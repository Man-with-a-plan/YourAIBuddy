using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class GrabbingState : BaseState<GrabbingStateMachine.EGrabbingState >
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
    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 ClosestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
       
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
    protected void SetPointsForEachLimb()
    {
        Context.SetClosestPoint(Context.GrabPointsLeftArm, RigCollisionHandler.BodySide.LeftArm);
        Context.SetClosestPoint(Context.GrabPointsRightArm, RigCollisionHandler.BodySide.RightArm);
        Context.SetClosestPoint(Context.GrabPointsLeftLeg, RigCollisionHandler.BodySide.LeftLeg);
        Context.SetClosestPoint(Context.GrabPointsRightLeg, RigCollisionHandler.BodySide.RightLeg);
        Debug.Log("Set");
    }
    private void SetIkTargetPosition()
    {
      
        Context.LeftIkConstraint.data.target.transform.position = Vector3.MoveTowards(
                          Context.LeftIkConstraint.data.target.transform.position,
                          Context.FurthestGrabPointFromLeftShoulder,
                          Time.deltaTime
                      );

        Context.RightIkConstraint.data.target.transform.position = Vector3.MoveTowards(
                          Context.RightIkConstraint.data.target.transform.position,
                          Context.FurthestGrabPointFromRightShoulder,
                          Time.deltaTime
                      );
        
        Context.LeftLegIkConstraint.data.target.transform.position = Vector3.MoveTowards(
            Context.LeftLegIkConstraint.data.target.transform.position,
            Context.FurthestGrabPointFromLeftHip,
            Time.deltaTime
            );
        Context.RightLegIkConstraint.data.target.transform.position = Vector3.MoveTowards(
            Context.RightLegIkConstraint.data.target.transform.position,
            Context.FurthestGrabPointFromRightHip,
            Time.deltaTime
        );


    }
  


}
