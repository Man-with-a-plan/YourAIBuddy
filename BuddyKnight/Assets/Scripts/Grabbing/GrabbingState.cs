using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class GrabbingState : BaseState<GrabbingStateMachine.EGrabbingState >
{
    protected GrabbingContext Context;
    private bool _isMovingLimbs = false;

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

    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 ClosestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
        Debug.Log(intersectingCollider);
       // UpdateIkTargetPositionTracking();
    }

    protected async void UpdateIkTargetPositionTracking()
    {
        if (_isMovingLimbs)
            return;

        await MoveLimbsSequentially();
        SetIkTargetRotation();
    }

    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {

    }

    protected void SetPointsForEachLimb()
    {
        Debug.Log("Setting points for each limb");
        Context.Normal = Context.GetPlaneNormal();
        Context.SetClosestPoint(Context.GrabPointsLeftArm, RigCollisionHandler.BodySide.LeftArm);
        Context.SetClosestPoint(Context.GrabPointsRightArm, RigCollisionHandler.BodySide.RightArm);
        Context.SetClosestPoint(Context.GrabPointsLeftLeg, RigCollisionHandler.BodySide.LeftLeg);
        Context.SetClosestPoint(Context.GrabPointsRightLeg, RigCollisionHandler.BodySide.RightLeg);
        // Debug: Check if grab points were found
        if (Context.FurthestGrabPointFromLeftShoulder == Vector3.positiveInfinity)
            Debug.LogWarning("No grab point found for Left Shoulder");
        if (Context.FurthestGrabPointFromRightShoulder == Vector3.positiveInfinity)
            Debug.LogWarning("No grab point found for Right Shoulder");
        if (Context.FurthestGrabPointFromLeftHip == Vector3.positiveInfinity)
            Debug.LogWarning("No grab point found for Left Hip");
        if (Context.FurthestGrabPointFromRightHip == Vector3.positiveInfinity)
            Debug.LogWarning("No grab point found for Right Hip");
       
    }

    private async Task MoveLimbsSequentially()
    {
        _isMovingLimbs = true;

        // Move each limb one at a time
        await MoveLimbToTarget(
            Context.LeftIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromLeftShoulder,
            "LeftHand"
        );

        await MoveLimbToTarget(
            Context.RightIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromRightShoulder,
            "RightHand"
        );

        await MoveLimbToTarget(
            Context.LeftLegIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromLeftHip,
            "LeftLeg"
        );

        await MoveLimbToTarget(
            Context.RightLegIkConstraint.data.target.transform,
            Context.FurthestGrabPointFromRightHip,
            "RightLeg"
        );

        _isMovingLimbs = false;
    }

    private async Task MoveLimbToTarget(Transform limbTarget, Vector3 targetPosition, string limbName)
    {
        // Validate target position before moving
        if (!IsValidPosition(targetPosition))
        {
            Debug.LogWarning($"Invalid target position for {limbName}: {targetPosition}. Skipping movement.");
            return;
        }

        float moveSpeed = 2f; // Adjust this to control limb movement speed
        float stopDistance = 0.01f; // Distance threshold to consider limb "reached" target

        while (Vector3.Distance(limbTarget.position, targetPosition) > stopDistance)
        {
            limbTarget.position = Vector3.MoveTowards(
                limbTarget.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            await Task.Yield(); // Wait for next frame
        }

        Debug.Log($"{limbName} reached target");
    }

    private bool IsValidPosition(Vector3 position)
    {
        return !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z)
               && position != Vector3.positiveInfinity && position != Vector3.negativeInfinity;
    }

    private void SetIkTargetRotation()
    {
        if (Context.Normal == Vector3.zero)
        {
            Debug.LogWarning("Plane normal is zero, cannot set IK target rotation");
            return;
        }

        // Calculate up vector by projecting world up onto the plane
        Vector3 upVector = Vector3.ProjectOnPlane(Vector3.up, Context.Normal).normalized;

        if (upVector == Vector3.zero)
        {
            // If plane normal is parallel to world up, use an alternative up
            upVector = Vector3.ProjectOnPlane(Vector3.right, Context.Normal).normalized;
        }

        // Create target rotation aligned with plane normal
        Quaternion targetRotation = Quaternion.LookRotation(Context.Normal, upVector);

        // Apply rotation to IK constraint targets
        Context.LeftIkConstraint.data.target.transform.rotation = targetRotation;
        Context.RightIkConstraint.data.target.transform.rotation = targetRotation;
        Context.LeftLegIkConstraint.data.target.transform.rotation = targetRotation;
        Context.RightLegIkConstraint.data.target.transform.rotation = targetRotation;
    }
}