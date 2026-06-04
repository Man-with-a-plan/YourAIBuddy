using UnityEngine;

public class RightHandGrabState : GrabbingState
{
    private float elapsedTime = 0f;
    private float approachDuration = 0.5f;
    Vector3 stillArmWorldPos;
    Quaternion stillArmWorldRot;
    Vector3 stillLegWorldPos;
    Quaternion stillLegWorldRot;
    public RightHandGrabState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        
    }
    public override void EnterState()
    {
                elapsedTime = 0f;
        Debug.Log("RightGrabStateEntered");
        UpdateIkTargetPositionTracking(RigCollisionHandler.BodySide.RightArm, approachDuration);
        stillArmWorldPos = Context.LeftIkConstraint.data.target.position;
        stillArmWorldRot = Context.LeftIkConstraint.data.target.rotation;
        stillLegWorldPos = Context.RightLegIkConstraint.data.target.position;
        stillLegWorldRot = Context.RightLegIkConstraint.data.target.rotation;
    }
    public override void ExitState() { Debug.Log("RightGrabStateIsExited"); }
    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("RightGrabStateIsUpdating");
        SetPointsForEachLimb();
        KeepStillLimbsInPlace(stillArmWorldPos, stillArmWorldRot, stillLegWorldPos, stillLegWorldRot, RigCollisionHandler.BodySide.RightArm);

    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        if (Context.CurrentlyGrabbing.Count == 0)
        {
            return GrabbingStateMachine.EGrabbingState.Reset;
        }
        if (elapsedTime >= approachDuration)
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
