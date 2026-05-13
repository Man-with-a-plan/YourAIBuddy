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

        SetIkTargetPosition();
    }

    protected void UpdateIkTargetPositionTracking()
    {
        SetIkTargetPosition();
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

    private void SetIkTargetPosition()
    {
        // Already running
        if (ikSequenceRoutine != null)
            return;

        ikSequenceRoutine = Context.StateMachine.RunCoroutine(
            MoveLimbsSequentially()
        );
    }

    private IEnumerator SmoothMoveCoroutine(
     Transform targetArm,
     Vector3 destinationArm,
     Transform targetLeg,
     Vector3 destinationLeg,
     float speed)
    {
        if (destinationArm == Vector3.zero)
        {
            Debug.LogWarning($"Destination is zero for {targetArm.name}, skipping movement.");
            yield break;
        }

        Vector3 startPosition = targetArm.position;

        float journeyLength = Vector3.Distance(startPosition, destinationArm);

        if (journeyLength < 0.001f)
            yield break;

        float elapsed = 0f;

        // Controls how high the arc goes
        float arcHeight = 0.25f;

        while (elapsed < 1f)
        {
            
            elapsed += (speed / journeyLength) * Time.deltaTime;

            // Base linear interpolation
            Vector3 position = Vector3.Lerp(
                startPosition,
                destinationArm,
                elapsed
            );

            // Arc motion (parabola)
            float heightOffset = 4f * arcHeight * elapsed * (1f - elapsed);

            position += Vector3.up * heightOffset;

            targetArm.position = position;

            yield return null;
        }

        targetLeg.position = destinationLeg;
    }

    private IEnumerator MoveLimbsSequentially()
    {
        float speed = 2f;

        yield return SmoothMoveCoroutine(
            Context.LeftIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromLeftShoulder,
            Context.RightLegIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromRightHip,
            speed
        );

        yield return SmoothMoveCoroutine(
            Context.RightIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromRightShoulder,
            Context.LeftLegIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromLeftHip,
            speed
        );

        //yield return SmoothMoveCoroutine(
        //    Context.LeftLegIkConstraint.data.target.transform,
        //    Context.FurthestGrabPointFromLeftHip,
        //    speed
        //);

        //yield return SmoothMoveCoroutine(
        //    Context.RightLegIkConstraint.data.target.transform,
        //    Context.FurthestGrabPointFromRightHip,
        //    speed
        //);

        ikSequenceRoutine = null;
    }


   
    #endregion
}