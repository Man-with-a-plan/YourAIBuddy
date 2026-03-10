using UnityEngine;
using UnityEngine.Rendering;

public abstract class GrabbingState : BaseState<GrabbingStateMachine.EGrabbingState >
{
    protected GrabbingContext Context;

    public GrabbingState(GrabbingContext context, GrabbingStateMachine.EGrabbingState stateKey) : base(stateKey)
    {
        Context = context;
         
    }

   
    protected void UpdateGrabbablePoints(GameObject grabPoint, bool shouldAdd)
    {
        if (shouldAdd)
        {
            Context.GrabPoints.Add(grabPoint);
            Debug.Log(Context.GrabPoints.Count);
         
        }
        else
        {
            Context.GrabPoints.Remove(grabPoint);
            Debug.Log("after deleted"+Context.GrabPoints.Count);

        }
        Context.SetPointsToGrabForBothHands();
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
                          Context.ClosestGrabPointFromLeftShoulder,
                          Time.deltaTime
                      );

        Context.RightIkConstraint.data.target.transform.position = Vector3.MoveTowards(
                          Context.RightIkConstraint.data.target.transform.position,
                          Context.ClosestGrabPointFromRightShoulder,
                          Time.deltaTime
                      );


        Debug.Log("Set");
    }

   
}
