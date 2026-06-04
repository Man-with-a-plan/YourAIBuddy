using System.Collections.Generic;
using NUnit.Framework;
using StarterAssets;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabbingContext
{
    private TwoBoneIKConstraint _leftIkConstraint;
    private TwoBoneIKConstraint _rightIkConstraint;
    private MultiRotationConstraint _leftMultiRotConstraint;
    private MultiRotationConstraint _rightMultiRotConstraint;
    private CharacterController _characterController;
    private ThirdPersonController _thirdPersonController;
    private Transform _rootTransform;
    private ChainIKConstraint _leftIkConstraintChain;
    private ChainIKConstraint _rightIkConstraintChain;

    public GrabbingContext(ChainIKConstraint leftIkConstraint, ChainIKConstraint rightIkConstraint, TwoBoneIKConstraint leftLegIkContstaint,
        TwoBoneIKConstraint rightLegIkConstraint,
        MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotConstraint,
        CharacterController characterController, ThirdPersonController thirdPersonController, Transform rootTransform)
    {
        _leftIkConstraintChain = leftIkConstraint;
        _rightIkConstraintChain = rightIkConstraint;
        _leftIkConstraint = leftLegIkContstaint;
        _rightIkConstraint = rightLegIkConstraint;
        _leftMultiRotConstraint = leftMultiRotationConstraint;
        _rightMultiRotConstraint = rightMultiRotConstraint;
        _characterController = characterController;
        _rootTransform = rootTransform;
        _thirdPersonController = thirdPersonController;
    }

    public ChainIKConstraint LeftIkConstraint => _leftIkConstraintChain;
    public ChainIKConstraint RightIkConstraint => _rightIkConstraintChain;
    public TwoBoneIKConstraint LeftLegIkConstraint => _leftIkConstraint;
    public TwoBoneIKConstraint RightLegIkConstraint => _rightIkConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotConstraint;

    public CharacterController CharacterController => _characterController;
    public ThirdPersonController ThirdPersonController => _thirdPersonController;
    public Transform RootTransform => _rootTransform;

    public ChainIKConstraint CurrentIkConstraint;
    public MultiRotationConstraint CurrentMultiRotationConstraint;

    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }

    public List<GameObject> GrabPointsLeftLeg { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsRightLeg { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsRightArm { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsLeftArm { get; private set; } = new List<GameObject>();

    public Dictionary<string, GameObject> CurrentlyGrabbing = new Dictionary<string, GameObject>();

    public Vector3 FurthestGrabPointFromLeftShoulder { get; private set; }
    public Vector3 FurthestGrabPointFromRightShoulder { get; private set; }
    public Vector3 FurthestGrabPointFromLeftHip { get; private set; }
    public Vector3 FurthestGrabPointFromRightHip { get; private set; }
    public Vector3 OriginalLeftShoulder { get; private set; }
    public Vector3 OriginalRightShoulder { get; private set; }
    public Vector3 OriginalLeftHip { get; private set; }
    public Vector3 OriginalRightHip { get; private set; }

    protected GameObject leftHandGrab, rightHandGrab;
    public Vector3 Normal;

    public MonoBehaviour Owner;
    public GrabbingStateMachine StateMachine;

    /// <summary>
    /// Called when a grab point enters a limb's trigger collider
    /// </summary>
    public void OnPointEntered(GameObject grabbable, RigCollisionHandler.BodySide limb)
    {
        switch (limb)
        {
            case RigCollisionHandler.BodySide.RightArm:
                if (!GrabPointsRightArm.Contains(grabbable))
                    GrabPointsRightArm.Add(grabbable);
                break;
            case RigCollisionHandler.BodySide.LeftArm:
                if (!GrabPointsLeftArm.Contains(grabbable))
                    GrabPointsLeftArm.Add(grabbable);
                break;
            case RigCollisionHandler.BodySide.RightLeg:
                if (!GrabPointsRightLeg.Contains(grabbable))
                    GrabPointsRightLeg.Add(grabbable);
                break;
            case RigCollisionHandler.BodySide.LeftLeg:
                if (!GrabPointsLeftLeg.Contains(grabbable))
                    GrabPointsLeftLeg.Add(grabbable);
                break;
        }
    }

    /// <summary>
    /// Called when a grab point exits a limb's trigger collider
    /// </summary>
    public void OnPointExited(GameObject grabbable, RigCollisionHandler.BodySide limb)
    {
        switch (limb)
        {
            case RigCollisionHandler.BodySide.RightArm:
                GrabPointsRightArm.Remove(grabbable);
                break;
            case RigCollisionHandler.BodySide.LeftArm:
                GrabPointsLeftArm.Remove(grabbable);
                break;
            case RigCollisionHandler.BodySide.RightLeg:
                GrabPointsRightLeg.Remove(grabbable);
                break;
            case RigCollisionHandler.BodySide.LeftLeg:
                GrabPointsLeftLeg.Remove(grabbable);
                break;
        }
    }

    public void SetClosestPoint(List<GameObject> ListOfClosestGrabPoints, RigCollisionHandler.BodySide limb)
    {
        // Clean up the list BEFORE processing
        ListCleanup(ListOfClosestGrabPoints);

        if (ListOfClosestGrabPoints.Count > 0)
        {
            Vector3 Center = RightIkConstraint.data.root.transform.position;

            float longestLeftDistance = Vector3.Distance(Center, ListOfClosestGrabPoints[0].transform.position);
            leftHandGrab = ListOfClosestGrabPoints[0];

            float distanceToCheck = float.MaxValue;

            for (int i = 0; i < ListOfClosestGrabPoints.Count; i++)
            {
                distanceToCheck = Vector3.Distance(Center, ListOfClosestGrabPoints[i].transform.position);
                if (distanceToCheck > longestLeftDistance)
                {
                    longestLeftDistance = distanceToCheck;
                    leftHandGrab = ListOfClosestGrabPoints[i];
                }
            }

            switch (limb)
            {
                case RigCollisionHandler.BodySide.LeftArm:
                    FurthestGrabPointFromLeftShoulder = leftHandGrab.transform.position;
                    CurrentlyGrabbing["LeftHand"] = leftHandGrab;
                    Debug.Log("Left hand grab point: " + FurthestGrabPointFromLeftShoulder);
                    break;
                case RigCollisionHandler.BodySide.RightArm:
                    FurthestGrabPointFromRightShoulder = leftHandGrab.transform.position;
                    CurrentlyGrabbing["RightHand"] = leftHandGrab;
                    Debug.Log("Right hand grab point: " + FurthestGrabPointFromRightShoulder);
                    break;
                case RigCollisionHandler.BodySide.RightLeg:
                    FurthestGrabPointFromRightHip = leftHandGrab.transform.position;
                    CurrentlyGrabbing["RightLeg"] = leftHandGrab;
                    Debug.Log("Right leg grab point: " + FurthestGrabPointFromRightHip);
                    break;
                case RigCollisionHandler.BodySide.LeftLeg:
                    FurthestGrabPointFromLeftHip = leftHandGrab.transform.position;
                    CurrentlyGrabbing["LeftLeg"] = leftHandGrab;
                    Debug.Log("Left leg grab point: " + FurthestGrabPointFromLeftHip);
                    break;
            }
        }
        else
        {
            Debug.Log("No grab points in range for " + limb);

            switch (limb)
            {
                case RigCollisionHandler.BodySide.LeftArm:
                    FurthestGrabPointFromLeftShoulder = RootTransform.position + Vector3.up;
                    CurrentlyGrabbing.Remove("LeftHand");
                    break;
                case RigCollisionHandler.BodySide.RightArm:
                    FurthestGrabPointFromRightShoulder = RootTransform.position + Vector3.up;
                    CurrentlyGrabbing.Remove("RightHand");
                    break;
                case RigCollisionHandler.BodySide.RightLeg:
                    FurthestGrabPointFromRightHip = RootTransform.position;
                    CurrentlyGrabbing.Remove("RightLeg");
                    break;
                case RigCollisionHandler.BodySide.LeftLeg:
                    FurthestGrabPointFromLeftHip = RootTransform.position;
                    CurrentlyGrabbing.Remove("LeftLeg");
                    break;
            }
        }
    }

    private void ListCleanup(List<GameObject> ListToClean)
    {
        float maxDistance = _characterController.height / 1.5f;
        Vector3 controllerPosition = _characterController.transform.position;
        Vector3 center = controllerPosition + _characterController.center;

        for (int i = ListToClean.Count - 1; i >= 0; i--)
        {
            float distanceToGrabPoint = Vector3.Distance(center, ListToClean[i].transform.position);

            if (distanceToGrabPoint > maxDistance)
            {
                Debug.Log("Grab point too far: distance " + distanceToGrabPoint + " > max " + maxDistance);
                ListToClean.RemoveAt(i);
            }
        }
    }

    public Vector3 GetPlaneNormal()
    {
        try
        {
            Vector3 leftHandPos = CurrentlyGrabbing["LeftHand"].transform.position;
            Vector3 rightHandPos = CurrentlyGrabbing["RightHand"].transform.position;
            Vector3 leftLegPos = CurrentlyGrabbing["LeftLeg"].transform.position;
            Vector3 rightLegPos = CurrentlyGrabbing["RightLeg"].transform.position;
            Vector3 betweenArms = (rightHandPos + leftHandPos) / 2;

            Vector3 edge1 = rightLegPos - betweenArms;
            Vector3 edge2 = leftLegPos - betweenArms;

            Vector3 normalVector = Vector3.Cross(edge2, edge1).normalized;

            return normalVector;
        }
        catch (System.Exception e)
        {
            return Vector3.zero;
        }
    }

    public Vector3 GetCenter()
    {
        bool hasLeftHand = TryGetGrabPosition("LeftHand", out Vector3 leftHandPos);
        bool hasRightHand = TryGetGrabPosition("RightHand", out Vector3 rightHandPos);
        bool hasLeftLeg = TryGetGrabPosition("LeftLeg", out Vector3 leftLegPos);
        bool hasRightLeg = TryGetGrabPosition("RightLeg", out Vector3 rightLegPos);

        // Diagonals span the body, so their midpoint is the best center estimate.
        bool diagA = hasLeftHand && hasRightLeg;   // left hand <-> right leg
        bool diagB = hasRightHand && hasLeftLeg;   // right hand <-> left leg

        // Both diagonals present == averaging all four limbs.
        if (diagA && diagB)
            return (leftHandPos + rightHandPos + leftLegPos + rightLegPos) / 4f;

        // One limb dropped: use whichever opposite pair is still complete.
        if (diagA) return (leftHandPos + rightLegPos) / 2f;
        if (diagB) return (rightHandPos + leftLegPos) / 2f;

        // No complete diagonal: average whatever limbs remain.
        Vector3 sum = Vector3.zero;
        int count = 0;
        if (hasLeftHand) { sum += leftHandPos; count++; }
        if (hasRightHand) { sum += rightHandPos; count++; }
        if (hasLeftLeg) { sum += leftLegPos; count++; }
        if (hasRightLeg) { sum += rightLegPos; count++; }
        if (count > 0) return sum / count;

        // Nothing connected at all — fall back so callers never get garbage.
        Debug.LogWarning("GetCenter: no grab points connected, returning transform.position");
        return RootTransform.position;
    }

    private bool TryGetGrabPosition(string limb, out Vector3 position)
    {
        position = Vector3.zero;
        if (CurrentlyGrabbing == null) return false;
        if (!CurrentlyGrabbing.TryGetValue(limb, out var grab)) return false;
        if (grab == null) return false;          // Unity == covers destroyed objects too
        position = grab.transform.position;
        return true;
    }

    public void SetOriginalLimbPositions()
    {
        OriginalLeftShoulder = LeftIkConstraint.data.target.transform.localPosition;
        OriginalRightShoulder = RightIkConstraint.data.target.transform.localPosition;
        OriginalLeftHip = LeftLegIkConstraint.data.target.transform.localPosition;
        OriginalRightHip = RightLegIkConstraint.data.target.transform.localPosition;
        Debug.Log("Grab points are set to original positions.");
    }

    public void ResetGrabPoints()
    {
        LeftIkConstraint.data.target.transform.localPosition = OriginalLeftShoulder;
        RightIkConstraint.data.target.transform.localPosition = OriginalRightShoulder;
        LeftLegIkConstraint.data.target.transform.localPosition = OriginalLeftHip;
        RightLegIkConstraint.data.target.transform.localPosition = OriginalRightHip;
        CurrentlyGrabbing.Clear();
    }
}