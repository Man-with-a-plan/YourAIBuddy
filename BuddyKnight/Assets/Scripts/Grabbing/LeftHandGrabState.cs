using UnityEngine;

public class LeftHandGrabState : GrabbingState
{
    private float elapsedTime = 0f;
    private float approachDuration = 0.5f;
    public LeftHandGrabState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        //GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        elapsedTime = 0f;
        SetPointsForEachLimb();
       
        Debug.Log("LeftGrabStateEntered");
    }
    public override void ExitState() { 
        Debug.Log("LeftGrabStateIsExited");
    }
    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("LeftGrabStateIsUpdating");
        SetPointsForEachLimb();

        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.LeftArm, approachDuration);
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        if (Context.CurrentlyGrabbing.Count == 0)
        {
            return GrabbingStateMachine.EGrabbingState.Reset;
        }
        if(elapsedTime >= approachDuration)
        {
            return GrabbingStateMachine.EGrabbingState.RightGrab;
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
