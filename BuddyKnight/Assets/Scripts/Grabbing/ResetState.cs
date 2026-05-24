using UnityEngine;

public class ResetState:GrabbingState
{
   private float _elapsedTime = 0f;
   private float _resetDuration = 0.5f;

    public ResetState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        // GrabbingContext Context = context;
    

    }
 

    public override void EnterState()
    {
       
        RigCollisionHandler.NewPointEntered -= AddGrabbablePoints;
         RigCollisionHandler.NewPointExited -= DeleteGrabbablePoints;
        _elapsedTime = 0;
        Debug.Log("ResetIsentered");
        
        Context.LeftLegIkConstraint.weight = 0f;
        Context.RightLegIkConstraint.weight = 0f;
        Context.LeftIkConstraint.weight = 0f;
        Context.RightIkConstraint.weight = 0f;
        Context.RightMultiRotationConstraint.weight = 0f;
        Context.LeftMultiRotationConstraint.weight = 0f;

        Context.ResetGrabPoints();
    }

    public override void ExitState()
    {
        Debug.Log("ResetIsExited");
    }
    public override void UpdateState()
    {
       
        _elapsedTime += Time.deltaTime;
        Debug.Log("ResetStateIsUpdating");
    }
    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        Debug.Log("Elapsed Time: " + _elapsedTime);
        Debug.Log("Character Velocity Magnitude: " + Context.CharacterController.velocity.magnitude);
        bool isMoving = Context.CharacterController.velocity.magnitude > 0.1f;
        
        Debug.Log("Is Moving: " + isMoving);

        if (_elapsedTime >= _resetDuration && !isMoving)
        {
            
            return GrabbingStateMachine.EGrabbingState.Search;
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
