using System.Collections;
using UnityEngine;

public class SearchState:GrabbingState
{
    public SearchState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
       // GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        Debug.Log("SearchIsentered");
     
        RigCollisionHandler.NewPointEntered += AddGrabbablePoints;
        RigCollisionHandler.NewPointExited += DeleteGrabbablePoints;
    }

    public override void ExitState()
    {
        RigCollisionHandler.NewPointEntered -= AddGrabbablePoints;
        RigCollisionHandler.NewPointExited -= DeleteGrabbablePoints;
        Debug.Log("SearchIsExited");
    }
    public override void UpdateState()
    {
        SetPointsForEachLimb();

        UpdateIkTargetPositionTracking();

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
