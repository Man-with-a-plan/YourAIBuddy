using UnityEngine;

public class IsUnder : ActionNode
{
    [SerializeField] private float distanceThresh = 1f;
    [SerializeField] private float heightThresh = 5f;
    [SerializeField] private bool isOver;
    protected override void OnStart(BehaviourTreeState state)
    { }
    protected override void OnStop(BehaviourTreeState state)
    { }

    protected override State OnUpdate(BehaviourTreeState state)
    {

        if (state.Owner.NavMeshAgent == null || !state.Owner.NavMeshAgent.enabled)
        {
            Debug.Log("fail");
            return State.Failure;
        }

        var position = state.Owner.transform.position;
        var playerPosition = state.Owner.NavMeshAgent.destination;
        var distance = Vector3.Distance(position, playerPosition);
        if (isOver)
        {
            if (distance <= distanceThresh && position.y < heightThresh)
            {
                Debug.Log("he is VERY close");

                return State.Success;


            }

            else
            {
                // Debug.Log("Don't reach for dat peach");
                return State.Failure;
            }
        }
        else
        {
            if (distance <= distanceThresh && position.y > heightThresh)
            {
                Debug.Log("he is on top and close");

                return State.Success;


            }

            else
            {
                // Debug.Log("Don't reach for dat peach");
                return State.Failure;
            }
        }
      
    }
}

