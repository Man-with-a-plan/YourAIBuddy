using UnityEngine;

public class LeftHandGrabState : GrabbingState
{
    private float elapsedTime = 0f;
    private float approachDuration = 0.5f;
    Vector3 stillArmWorldPos;
    Quaternion stillArmWorldRot;
    Vector3 stillLegWorldPos;
    Quaternion stillLegWorldRot;
    public LeftHandGrabState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        //GrabbingContext Context = context;
      
    }
    public override void EnterState()
    {
        elapsedTime = 0f;
        SetPointsForEachLimb();
        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.LeftArm, approachDuration);
        Debug.Log("LeftGrabStateEntered");
         stillArmWorldPos = Context.RightIkConstraint.data.target.position;
         stillArmWorldRot = Context.RightIkConstraint.data.target.rotation;
         stillLegWorldPos = Context.LeftLegIkConstraint.data.target.position;
         stillLegWorldRot = Context.LeftLegIkConstraint.data.target.rotation;
    }
    public override void ExitState() { 
        Debug.Log("LeftGrabStateIsExited");
    }
    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("LeftGrabStateIsUpdating");
        SetPointsForEachLimb();
        KeepStillLimbsInPlace(stillArmWorldPos, stillArmWorldRot, stillLegWorldPos, stillLegWorldRot, RigCollisionHandler.BodySide.LeftArm);

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
