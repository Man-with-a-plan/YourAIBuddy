using System.Globalization;
using Unity.VisualScripting.Antlr3.Runtime;

using UnityEngine.AI;
using UnityEngine;

public class GoTo : ActionNode
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float maxError = 1f;
    [SerializeField] private float distanceThresh = 3f;

    private Transform playerTransform;
    protected override void OnStart(BehaviourTreeState state)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
        {
            state.PlayerTransform = playerObj.transform;
        }
    }

    protected override void OnStop(BehaviourTreeState state)
    {

    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (state.Owner.NavMeshAgent == null || !state.Owner.NavMeshAgent.enabled || state.PlayerTransform == null)
        {
            return State.Failure;
        }

        Vector3 targetPosition = state.PlayerTransform.position;



        if (NavMesh.SamplePosition(targetPosition, out var hit, maxError, NavMesh.AllAreas))
        {

            float dist = Vector3.Distance(state.Owner.transform.position, targetPosition);

          
              state.Owner.NavMeshAgent.SetDestination(hit.position);

             return State.Success;

            //if (dist > distanceThresh)
            //{
            //    state.Owner.NavMeshAgent.speed = state.MovementSpeed;
            //    state.Owner.NavMeshAgent.SetDestination(hit.position);

            //   return State.Running;
            //}

            ////else
            ////{
            ////    state.Owner.NavMeshAgent.ResetPath(); // Optional: stops the agent immediately
            ////    return State.Success;
            ////}


            //return State.Success;
        }
        return State.Failure;
    }
}
