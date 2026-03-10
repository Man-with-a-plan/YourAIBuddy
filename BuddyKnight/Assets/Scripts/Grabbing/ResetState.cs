using UnityEngine;

public class ResetState:GrabbingState
{
    public ResetState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        Debug.Log("ResetIsentered");
    }

    public override void ExitState()
    {

    }
    public override void UpdateState()
    {

    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        return StateKey;
      
    }

    public override void OnTriggerEnter(Collider other)
    {

    }
    public override void OnTriggerStay(Collider other)
    {

    }
    public override void OnTriggerExit(Collider other)
    {

    }
}
