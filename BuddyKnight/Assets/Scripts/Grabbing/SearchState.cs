using System.Collections;
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
        Context.CurrenBodySide = GrabbingContext.EBodySide.Top;
        Context.LegCollider.enabled = false;
    }

    public override void ExitState()
    {
        Debug.Log("SearchIsExited");
    }
    public override void UpdateState()
    {
        

        UpdateIkTargetPositionTracking();

    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
       return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {
        Debug.Log("SearchState: TriggerEnter" + other.name + Context.CurrenBodySide);
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
           
            ColliderSwitcheroo(other, true);

        }
       
    }
    public override void OnTriggerStay(Collider other)
    {
        
    }
    public override void OnTriggerExit(Collider other)
    {
       Debug.Log("SearchState: TriggerExit");
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            
           ColliderSwitcheroo(other, false);
        }
    }
    private void ColliderSwitcheroo(Collider other, bool shouldAdd)
    {
      if(  UpdateGrabbablePoints(other.gameObject, Context.CurrenBodySide, shouldAdd))
        {
            if (Context.CurrenBodySide == GrabbingContext.EBodySide.Top)
            {
                Context.ArmCollider.enabled = false;
                Context.LegCollider.enabled = true;
                Context.CurrenBodySide = GrabbingContext.EBodySide.Bottom;
            }
            else
            {
                Context.LegCollider.enabled = false;
                Context.ArmCollider.enabled = true;
            
                Context.CurrenBodySide = GrabbingContext.EBodySide.Top;
            }
        }
       

        
    }
 
}
