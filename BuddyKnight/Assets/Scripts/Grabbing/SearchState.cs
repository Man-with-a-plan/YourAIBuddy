using UnityEngine;

public class SearchState:GrabbingState
{
    public SearchState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        GrabbingContext Context = context;
    }
    public override void EnterState()
    {
        Debug.Log("SearchIsentered");
       
    }

    public override void ExitState()
    {
        Debug.Log("SearchIsExited");
    }
    public override void UpdateState()
    {
        UpdateIkTargetPositionTracking();
        Debug.Log("SearchIsUpdating");
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
       return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {
        
       
        if(other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
            {
            Debug.Log("maybe they just don't see?");
            UpdateGrabbablePoints(other.gameObject, true);
          
        }
    }
    public override void OnTriggerStay(Collider other)
    {
        
    }
    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            UpdateGrabbablePoints(other.gameObject, false);
        }
    }
}
