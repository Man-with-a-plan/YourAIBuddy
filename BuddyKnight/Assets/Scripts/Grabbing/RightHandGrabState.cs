using UnityEngine;

public class RightHandGrabState : GrabbingState
{
    public RightHandGrabState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        //GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        Debug.Log("GrabStateEntered");
    }
    public override void ExitState() { Debug.Log("GrabStateIsExited"); }
    public override void UpdateState()
    {
        Debug.Log("GrabStateIsUpdating");
        SetPointsForEachLimb();

        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.RightArm);
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
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
