using System.Collections;
using UnityEngine;

public class SearchState:GrabbingState
{
   private float elapsedTime = 0f;
    private float resetDuration = 2f;
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

        



        elapsedTime += Time.deltaTime;
     
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
            if (Context.CurrentlyGrabbing.Count > 0)
            {
                Debug.Log(Context.CurrentlyGrabbing.Count + " <--- Grabbing");
           
           
            return GrabbingStateMachine.EGrabbingState.Approach;
            }
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
