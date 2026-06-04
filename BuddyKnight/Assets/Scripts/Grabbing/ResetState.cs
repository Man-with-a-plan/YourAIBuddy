using UnityEngine;

public class ResetState : GrabbingState
{
    private float _elapsedTime = 0f;
    private float _resetDuration = 0.5f;
    
    public ResetState(GrabbingContext context, GrabbingStateMachine.EGrabbingState eGrabbingState) : base(context, eGrabbingState)
    {
        Context.SetOriginalLimbPositions();
    }

    public override void EnterState()
    {
      
        _elapsedTime = 0;
        Debug.Log("ResetIsentered");

        // Immediately disable all IK constraint weights
        Context.LeftLegIkConstraint.weight = 0f;
        Context.RightLegIkConstraint.weight = 0f;
        Context.LeftIkConstraint.weight = 0f;
        Context.RightIkConstraint.weight = 0f;
        Context.RightMultiRotationConstraint.weight = 0f;
        Context.LeftMultiRotationConstraint.weight = 0f;

        // Reset IK target positions to their original stored positions
      

        // Additionally, reset the actual target transform positions to avoid visual artifacts
      
    }

    public override void ExitState()
    {
        Debug.Log("ResetIsExited");
    }

    public override void UpdateState()
    {
        
        _elapsedTime += Time.deltaTime;
        
        Debug.Log("ResetStateIsUpdating");
        Context.ResetGrabPoints();
    }

    public override GrabbingStateMachine.EGrabbingState GetNextState()
    {
        Debug.Log("Elapsed Time: " + _elapsedTime);

        // Transition to Search after reset duration has elapsed
        if (_elapsedTime >= _resetDuration)
        {
            Debug.Log("Transitioning to Search");
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