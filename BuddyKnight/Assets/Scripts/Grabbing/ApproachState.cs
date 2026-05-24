using UnityEngine;

public class ApproachState:GrabbingState
{
   private float elapsedTime = 0f;
    private float approachDuration = 3f;
    private float approachWeight = 0.5f;
    public ApproachState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        GrabbingContext Context = context;

    }
    public override void EnterState()
    {
        RigCollisionHandler.NewPointEntered += AddGrabbablePoints;
        RigCollisionHandler.NewPointExited += DeleteGrabbablePoints;
 
        Debug.Log("ApproachStateEntered");
    }
    public override void ExitState() {
        //RigCollisionHandler.NewPointEntered -= AddGrabbablePoints;
        //RigCollisionHandler.NewPointExited -= DeleteGrabbablePoints;
        Debug.Log("ApproachStateExited"); 
    }
    public override void UpdateState()
    {
       
        elapsedTime += Time.deltaTime;

        Context.LeftIkConstraint.weight = Mathf.Lerp(Context.LeftIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
        Context.RightIkConstraint.weight = Mathf.Lerp(Context.RightIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
       // Context.LeftLegIkConstraint.weight = Mathf.Lerp(Context.LeftLegIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
       // Context.RightLegIkConstraint.weight = Mathf.Lerp(Context.RightLegIkConstraint.weight, approachWeight, elapsedTime / approachDuration);
        Debug.Log("ApproachStateIsUpdating");
        SetPointsForEachLimb();

        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.LeftArm, 1.5f);
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        if (Context.CurrentlyGrabbing.Count == 0)
        {
            return GrabbingStateMachine.EGrabbingState.Reset;
        }
        if (Context.ThirdPersonController.isClimbingLadder)
        {
            return GrabbingStateMachine.EGrabbingState.StartClimbing;
        }
        return StateKey;
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
