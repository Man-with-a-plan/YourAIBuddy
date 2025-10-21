using UnityEngine;


    public class IsClose:ActionNode
    {
    [SerializeField] private float distanceThresh = 1f;

    protected override void OnStart(BehaviourTreeState state)
    {
      
      //  throw new System.NotImplementedException();
    }
    protected override void OnStop(BehaviourTreeState state)
    {
      
        // throw new System.NotImplementedException();
    }
    protected override State OnUpdate(BehaviourTreeState state)
    {
      
        if(state.Owner.NavMeshAgent == null || !state.Owner.NavMeshAgent.enabled)
        {
            Debug.Log("fail");
            return State.Failure;
        }

        var position = state.Owner.transform.position;
        var destination = state.Owner.NavMeshAgent.destination;
        var distance = Vector3.Distance(position, destination);

        if(distance <= distanceThresh)
        {
            Debug.Log("he is close");
            // Debug.Log("Reach for dat peach");
            return State.Success;
           

        }

        else
        {
          // Debug.Log("Don't reach for dat peach");
            return State.Failure;
        }

    }


}

