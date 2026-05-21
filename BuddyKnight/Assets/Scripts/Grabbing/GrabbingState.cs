using System.Collections;
using System.Collections.Generic;
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

    protected void UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide limb)
    {
        SetIkTargetPosition(limb);
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

    private void SetIkTargetPosition(RigCollisionHandler.BodySide limb)
    {
        // Already running
        if (ikSequenceRoutine != null)
            return;
        
        ikSequenceRoutine = Context.StateMachine.RunCoroutine(
            MoveLimbsSequentially(limb)
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
       float speed)
    {
        // Determine which limbs to move based on bodySide
        Transform movingArm;
        Transform movingLeg;
        Transform stillArm;
        Transform stillLeg;

        if (bodySide == RigCollisionHandler.BodySide.LeftArm)
        {
            movingArm = leftArmTransform;
            movingLeg = rightLegTransform;
            stillArm = rightArmTransform;
            stillLeg = leftLegTransform;
        }
        else // RightArm
        {
            movingArm = rightArmTransform;
            movingLeg = leftLegTransform;
            stillArm = leftArmTransform;
            stillLeg = rightLegTransform;
        }

        if (destinationArm == Vector3.zero || destinationLeg == Vector3.zero)
        {
            Debug.LogWarning($"Destination is zero for arm or leg, skipping movement.");
            yield break;
        }

        Vector3 startPositionArm = movingArm.position;
        Vector3 startPositionLeg = movingLeg.position;

        float journeyLengthArm = Vector3.Distance(startPositionArm, destinationArm);
        float journeyLengthLeg = Vector3.Distance(startPositionLeg, destinationLeg);

        if (journeyLengthArm < 0.001f && journeyLengthLeg < 0.001f)
            yield break;

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            float armSpeed = journeyLengthArm > 0.001f ? (speed / journeyLengthArm) * Time.deltaTime : 0f;
            float legSpeed = journeyLengthLeg > 0.001f ? (speed / journeyLengthLeg) * Time.deltaTime : 0f;

            elapsed += Mathf.Max(armSpeed, legSpeed);

            // Move arm
            if (journeyLengthArm > 0.001f)
            {
                float armElapsed = Mathf.Min(elapsed * journeyLengthArm / Mathf.Max(journeyLengthArm, journeyLengthLeg), 1f);
                Vector3 armPosition = Vector3.Lerp(startPositionArm, destinationArm, armElapsed);
                movingArm.position = armPosition;
            }

            // Move leg
            if (journeyLengthLeg > 0.001f)
            {
                float legElapsed = Mathf.Min(elapsed * journeyLengthLeg / Mathf.Max(journeyLengthArm, journeyLengthLeg), 1f);
                Vector3 legPosition = Vector3.Lerp(startPositionLeg, destinationLeg, legElapsed);
                movingLeg.position = legPosition;
            }

            yield return null;
        }

        movingArm.position = destinationArm;
        movingLeg.position = destinationLeg;
        stillArm.position = stillArm.position;
        stillLeg.position = stillLeg.position;
    }

    private IEnumerator MoveLimbsSequentially(RigCollisionHandler.BodySide limb)
    {
        float speed = 1.5f;

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