using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class GrabbingState : BaseState<GrabbingStateMachine.EGrabbingState >
{
    protected GrabbingContext Context;

    public GrabbingState(GrabbingContext context, GrabbingStateMachine.EGrabbingState stateKey) : base(stateKey)
    {
        Context = context;
         
    }

   //Point list manipulation
    protected bool UpdateGrabbablePoints(GameObject grabPoint, GrabbingContext.EBodySide side, bool shouldAdd)
    {
        if (shouldAdd & !Context.GrabPointsTop.Contains(grabPoint) & !Context.GrabPointsBottom.Contains(grabPoint) )
        {

            AddToList(grabPoint, side);
         
            Debug.Log(Context.GrabPointsBottom.Count +"Bottom");
            return true;

        }
        else if(!shouldAdd)
        {
            DeleteFromList(grabPoint, side);
            Debug.Log("after deleted"+Context.GrabPointsTop.Count);
           return true;

        }
        Context.SetTwoClosestPoints(side == GrabbingContext.EBodySide.Top ? Context.GrabPointsTop : Context.GrabPointsBottom);
      
        //Context.SetPointsToGrabForBothHands();
        return false;
    }
    
    protected void DeleteFromList(GameObject grabPoint, GrabbingContext.EBodySide side)
    {
        if (side == GrabbingContext.EBodySide.Top)
        {
            Context.GrabPointsTop.Remove(grabPoint);
        }
        else
        {
            Context.GrabPointsBottom.Remove(grabPoint);
        }
    }
    protected void AddToList(GameObject grabPoint, GrabbingContext.EBodySide side)
    {
        if (side == GrabbingContext.EBodySide.Top)
        {
            Context.GrabPointsTop.Add(grabPoint);
        }
        else
        {
            Context.GrabPointsBottom.Add(grabPoint);
        }
    }

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 posToCheck)
    {
        return intersectingCollider.ClosestPoint(posToCheck);
    }
    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 ClosestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
        Context.SetCurrentSide(ClosestPointFromRoot);
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
