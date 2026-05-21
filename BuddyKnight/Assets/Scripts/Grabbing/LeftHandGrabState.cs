using UnityEngine;

public class LeftHandGrabState : GrabbingState
{
  
    public LeftHandGrabState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        //GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        SetPointsForEachLimb();
       
        Debug.Log("GrabStateEntered");
    }
    public override void ExitState() { 
        Debug.Log("GrabStateIsExited");
    }
    public override void UpdateState()
    {
        
        Debug.Log("GrabStateIsUpdating");
        SetPointsForEachLimb();

        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.LeftArm);
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        if (Context.CurrentlyGrabbing.Count == 0)
        {
            return GrabbingStateMachine.EGrabbingState.Reset;
        }

        return StateKey;
        return GrabbingStateMachine.EGrabbingState.Search;
    }
    public override void OnTriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }
    public override void OnTriggerExit(Collider other)
    {
        throw new System.NotImplementedException();
    }
    public override void OnTriggerStay(Collider other)
    {
        throw new System.NotImplementedException();

    }
       
}
