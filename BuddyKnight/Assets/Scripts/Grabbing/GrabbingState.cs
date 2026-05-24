using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public abstract class GrabbingState : BaseState<GrabbingStateMachine.EGrabbingState>
{
    protected GrabbingContext Context;

    // Track active coroutines so they don't stack
    private Coroutine ikSequenceRoutine;

    public GrabbingState(
        GrabbingContext context,
        GrabbingStateMachine.EGrabbingState stateKey)
        : base(stateKey)
    {
        Context = context;
    }

    #region Point List Manipulation

    protected void AddGrabbablePoints(
        GameObject grabbable,
        RigCollisionHandler.BodySide limb)
    {
        switch (limb)
        {
            case RigCollisionHandler.BodySide.RightArm:
                Context.GrabPointsRightArm.Add(grabbable);
                break;

            case RigCollisionHandler.BodySide.LeftArm:
                Context.GrabPointsLeftArm.Add(grabbable);
                break;

            case RigCollisionHandler.BodySide.RightLeg:
                Context.GrabPointsRightLeg.Add(grabbable);
                break;

            case RigCollisionHandler.BodySide.LeftLeg:
                Context.GrabPointsLeftLeg.Add(grabbable);
                break;
        }
    }

    protected void DeleteGrabbablePoints(
        GameObject grabbable,
        RigCollisionHandler.BodySide limb)
    {
        switch (limb)
        {
            case RigCollisionHandler.BodySide.RightArm:
                Context.GrabPointsRightArm.Remove(grabbable);
                break;

            case RigCollisionHandler.BodySide.LeftArm:
                Context.GrabPointsLeftArm.Remove(grabbable);
                break;

            case RigCollisionHandler.BodySide.RightLeg:
                Context.GrabPointsRightLeg.Remove(grabbable);
                break;

            case RigCollisionHandler.BodySide.LeftLeg:
                Context.GrabPointsLeftLeg.Remove(grabbable);
                break;
        }
    }

    #endregion

    #region IK Tracking

    private Vector3 GetClosestPointOnCollider(
        Collider intersectingCollider,
        Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 closestPointFromRoot =
            GetClosestPointOnCollider(
                intersectingCollider,
                Context.RootTransform.position);

        Debug.Log(intersectingCollider);

       // SetIkTargetPosition();
    }

    protected void UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide limb, float speed)
    {
        SetIkTargetPosition(limb, speed);
    }

    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {

    }

    #endregion

    #region Grab Point Calculations

    protected void SetPointsForEachLimb()
    {
        Context.SetClosestPoint(
            Context.GrabPointsLeftArm,
            RigCollisionHandler.BodySide.LeftArm);

        Context.SetClosestPoint(
            Context.GrabPointsRightArm,
            RigCollisionHandler.BodySide.RightArm);

        Context.SetClosestPoint(
            Context.GrabPointsLeftLeg,
            RigCollisionHandler.BodySide.LeftLeg);

        Context.SetClosestPoint(
            Context.GrabPointsRightLeg,
            RigCollisionHandler.BodySide.RightLeg);
    }

    #endregion

    #region IK Movement

    private void SetIkTargetPosition(RigCollisionHandler.BodySide limb, float speed)
    {
        if (ikSequenceRoutine != null)
            return;

        ikSequenceRoutine = Context.StateMachine.RunCoroutine(
            MoveLimbsSequentially(limb, speed)
        );
    }

    private IEnumerator SmoothMoveCoroutine(
        Transform leftArmTransform,
        Transform rightArmTransform,
        Transform leftLegTransform,
        Transform rightLegTransform,
        Vector3 destinationArm,
        Vector3 destinationLeg,
        RigCollisionHandler.BodySide bodySide,
        float movementDuration)
    {
        // --- Validate inputs ---
        if (destinationArm == Vector3.zero || destinationLeg == Vector3.zero)
        {
            Debug.LogWarning("Destination is zero for arm or leg, skipping movement.");
            yield break;
        }

        // --- Assign moving vs still limbs ---
        Transform movingArm, movingLeg, stillArm, stillLeg;

        if (bodySide == RigCollisionHandler.BodySide.LeftArm)
        {
            movingArm = leftArmTransform;
            movingLeg = rightLegTransform;
            stillArm = rightArmTransform;
            stillLeg = leftLegTransform;
        }
        else
        {
            movingArm = rightArmTransform;
            movingLeg = leftLegTransform;
            stillArm = leftArmTransform;
            stillLeg = rightLegTransform;
        }

        // --- Early out if already at destination ---
        Vector3 startArm = movingArm.position;
        Vector3 startLeg = movingLeg.position;

        if (Vector3.Distance(startArm, destinationArm) < 0.001f &&
            Vector3.Distance(startLeg, destinationLeg) < 0.001f)
            yield break;

        // --- Anchor still limbs in world space ---
        // We track the CHARACTER root so we can compensate for body movement each frame.
        Transform root = Context.CharacterController.transform; // adjust if your root is a different reference
        Vector3 prevRootPos = root.position;
        Quaternion prevRootRot = root.rotation;

        Vector3 stillArmWorldPos = stillArm.position;
        Quaternion stillArmWorldRot = stillArm.rotation;
        Vector3 stillLegWorldPos = stillLeg.position;
        Quaternion stillLegWorldRot = stillLeg.rotation;

        // --- Move loop ---
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / movementDuration);

            // Optional: replace with a curve for snappier feel
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Move the active limbs toward their destinations
            movingArm.position = Vector3.Lerp(startArm, destinationArm, smoothT);
            movingLeg.position = Vector3.Lerp(startLeg, destinationLeg, smoothT);

            // --- Compensate still limbs for root movement ---
            // If the character moved or rotated this frame, offset the anchored
            // world positions by the same delta so they stay planted.
            Vector3 rootPosDelta = root.position - prevRootPos;
            Quaternion rootRotDelta = root.rotation * Quaternion.Inverse(prevRootRot);

            stillArmWorldPos = rootRotDelta * (stillArmWorldPos - prevRootPos) + root.position + rootPosDelta * 0f;
            stillLegWorldPos = rootRotDelta * (stillLegWorldPos - prevRootPos) + root.position + rootPosDelta * 0f;

            // Simpler version — use this if you only translate (no root rotation):
            // stillArmWorldPos += rootPosDelta;
            // stillLegWorldPos += rootPosDelta;

            stillArm.SetPositionAndRotation(stillArmWorldPos, stillArmWorldRot);
            stillLeg.SetPositionAndRotation(stillLegWorldPos, stillLegWorldRot);

            prevRootPos = root.position;
            prevRootRot = root.rotation;

            yield return null;
        }

        // --- Snap to exact final positions ---
        movingArm.position = destinationArm;
        movingLeg.position = destinationLeg;

        // Re-anchor still limbs one final time
        stillArm.SetPositionAndRotation(stillArmWorldPos, stillArmWorldRot);
        stillLeg.SetPositionAndRotation(stillLegWorldPos, stillLegWorldRot);
    }

    private IEnumerator MoveLimbsSequentially(RigCollisionHandler.BodySide limb, float speed)
    {
       

        // Move left arm and right leg simultaneously
        yield return SmoothMoveCoroutine(
            Context.LeftIkConstraint.data.target.transform,
            Context.RightIkConstraint.data.target.transform,
            Context.LeftLegIkConstraint.data.target.transform,
            Context.RightLegIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromLeftShoulder,
            Context.FurthestGrabPointFromRightHip,
            limb,
            speed
        );

        //// Move right arm and left leg simultaneously
        //yield return SmoothMoveCoroutine(
        //    Context.LeftIkConstraint.data.target.transform,
        //    Context.RightIkConstraint.data.target.transform,
        //    Context.LeftLegIkConstraint.data.target.transform,
        //    Context.RightLegIkConstraint.data.target.transform,
        //    Context.FurthestGrabPointFromRightShoulder,
        //    Context.FurthestGrabPointFromLeftHip,
        //    RigCollisionHandler.BodySide.RightArm,
        //    speed
        //);

        ikSequenceRoutine = null;
    }



    #endregion
}