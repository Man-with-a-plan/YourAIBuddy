using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class GrabbingState : BaseState<GrabbingStateMachine.EGrabbingState>
{
    protected GrabbingContext Context;

    public GrabbingState(GrabbingContext context, GrabbingStateMachine.EGrabbingState stateKey) : base(stateKey)
    {
        Context = context;

    }

    //Point list manipulation
    protected void AddGrabbablePoints(GameObject grabbable, RigCollisionHandler.BodySide limb)
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

    protected void DeleteGrabbablePoints(GameObject grabbable, RigCollisionHandler.BodySide limb)
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


    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 posToCheck)
    {
        return intersectingCollider.ClosestPoint(posToCheck);
    }
    protected async Task StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 ClosestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);

        Debug.Log(intersectingCollider);
       await SetIkTargetPosition();
    }
    protected async void UpdateIkTargetPositionTracking()
    {
        await SetIkTargetPosition();
    }
    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {

    }
    protected void SetPointsForEachLimb()
    {
        Context.SetClosestPoint(Context.GrabPointsLeftArm, RigCollisionHandler.BodySide.LeftArm);
        Context.SetClosestPoint(Context.GrabPointsRightArm, RigCollisionHandler.BodySide.RightArm);
        Context.SetClosestPoint(Context.GrabPointsLeftLeg, RigCollisionHandler.BodySide.LeftLeg);
        Context.SetClosestPoint(Context.GrabPointsRightLeg, RigCollisionHandler.BodySide.RightLeg);

    }
     private async Task SetIkTargetPosition()
    {
        float speed = 4f; // Adjust this value to control the speed of the transition
       await SmoothTransitionToTargetPosition(Context.LeftIkConstraint.data.target.transform, Context.FurthestGrabPointFromLeftShoulder, speed);
        await SmoothTransitionToTargetPosition(Context.RightIkConstraint.data.target.transform, Context.FurthestGrabPointFromRightShoulder, speed);
        await SmoothTransitionToTargetPosition(Context.LeftLegIkConstraint.data.target.transform, Context.FurthestGrabPointFromLeftHip, speed);
        await SmoothTransitionToTargetPosition(Context.RightLegIkConstraint.data.target.transform, Context.FurthestGrabPointFromRightHip, speed);

        //Context.RightIkConstraint.data.target.transform.position = Vector3.MoveTowards(
        //                  Context.RightIkConstraint.data.target.transform.position,
        //                  Context.FurthestGrabPointFromRightShoulder,
        //                  Time.deltaTime
        //              );

        //Context.LeftLegIkConstraint.data.target.transform.position = Vector3.MoveTowards(
        //    Context.LeftLegIkConstraint.data.target.transform.position,
        //    Context.FurthestGrabPointFromLeftHip,
        //    Time.deltaTime
        //    );
        //Context.RightLegIkConstraint.data.target.transform.position = Vector3.MoveTowards(
        //    Context.RightLegIkConstraint.data.target.transform.position,
        //    Context.FurthestGrabPointFromRightHip,
        //    Time.deltaTime
        //);


    }
     private  Task SmoothTransitionToTargetPosition(Transform target, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = target.position;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            target.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
             // Yield control back to the caller until the next frame
        }
        target.position = targetPosition; // Ensure it ends at the exact target position
        return Task.CompletedTask;
    }


}