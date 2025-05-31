using System.Globalization;
using Unity.VisualScripting.Antlr3.Runtime;

using UnityEngine.AI;
using UnityEngine;

public class GoTo : ActionNode
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float maxError = 1f;

    private Transform playerTransform;
    protected override void OnStart(BehaviourTreeState state)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        
    }

    protected override void OnStop(BehaviourTreeState state)
    {
       
    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (state.Owner.NavMeshAgent == null || !state.Owner.NavMeshAgent.enabled || playerTransform == null)
        {
            return State.Failure;
        }

        Vector3 targetPosition = playerTransform.position;
       
            Vector3 targetPos = playerTransform.position;

            if (NavMesh.SamplePosition(targetPos, out var hit, maxError, NavMesh.AllAreas))
            {
                state.Owner.NavMeshAgent.SetDestination(hit.position);
            Debug.Log("GoTo: Moving to player.");
            return State.Success;
           
            }
            else
            {
                Debug.Log("GoTo: Failed to find NavMesh position.");
                return State.Failure;
            }
        

        return State.Running;
    }
}
