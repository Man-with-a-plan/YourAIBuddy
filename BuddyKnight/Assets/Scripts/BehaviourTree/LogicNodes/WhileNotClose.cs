using UnityEngine;


    public class WhileNotClose : DecoratorNode
    {
    [SerializeField] private float distanceThresh = 3f;
    protected override void OnStart(BehaviourTreeState state)
    {
        
    }
    protected override void OnStop(BehaviourTreeState state)
    {
    }
    protected override State OnUpdate(BehaviourTreeState state)
    {
        
        if (state.PlayerTransform == null)
        {
            return State.Failure;
        }

        float dist = Vector3.Distance(state.Owner.transform.position, state.PlayerTransform.position);

        if (dist <= distanceThresh)
        {
            state.Owner.NavMeshAgent.ResetPath();
       
            return State.Success;
        }
        else
        {
            
            if (Child == null)
            {
              
                return State.Failure;
            }

          
             Child.Run(state);
            state.Owner.NavMeshAgent.speed = state.MovementSpeed;
            return State.Failure;

        }

     
    }
}

