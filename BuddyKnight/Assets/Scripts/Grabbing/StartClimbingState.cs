using UnityEngine;

public class StartClimbingState : GrabbingState
{
    private float elapsedTime = 0f;
    private float approachDuration = 1f;
    private float approachWeight = 1f;
    public StartClimbingState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        //GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        SetPointsForEachLimb();

        Debug.Log("GrabStateEntered");
    }
    public override void ExitState()
    {
        Debug.Log("GrabStateIsExited");
    }
    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;
        Context.LeftIkConstraint.weight = Mathf.Lerp(Context.LeftIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
        Context.RightIkConstraint.weight = Mathf.Lerp(Context.RightIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
        Context.LeftLegIkConstraint.weight = Mathf.Lerp(Context.LeftLegIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
        Context.RightLegIkConstraint.weight = Mathf.Lerp(Context.RightLegIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
        Debug.Log("GrabStateIsUpdating");
        SetPointsForEachLimb();

        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.LeftArm, approachDuration);
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        if (elapsedTime/approachDuration >= 1)
        {
            return GrabbingStateMachine.EGrabbingState.LeftGrab;
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
