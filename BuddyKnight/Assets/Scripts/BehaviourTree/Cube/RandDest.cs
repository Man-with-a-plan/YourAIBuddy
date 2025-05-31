using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AI; 

public class RandDest : ActionNode
{
    [SerializeField] private float radius;
    [SerializeField] private float maxError;

    protected override void OnStart(BehaviourTreeState state)
    {
       // throw new System.NotImplementedException();
    }
    protected override void OnStop(BehaviourTreeState state)
    {
        //throw new System.NotImplementedException();
    }
    protected override State OnUpdate(BehaviourTreeState state)
    {
      
        if (state.Owner.NavMeshAgent == null || !state.Owner.NavMeshAgent.enabled)
        {
            return State.Failure;
        }

        var randomDirection = Random.insideUnitSphere * radius;

        randomDirection.y = 1f;

        var position = state.Owner.transform.position + randomDirection;

        var destination = state.Owner.NavMeshAgent.destination;
        var distance = Vector3.Distance(position, destination);
        if (NavMesh.SamplePosition(position, out var hit, maxError, NavMesh.AllAreas))
        {
            position = hit.position;
            state.Owner.NavMeshAgent.SetDestination(hit.position);
        
            return State.Success;

        }

        else
        {
           
            return State.Failure;
        }

    }


}

