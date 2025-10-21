using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    [SerializeField] private BehaviourTree behaviourTree;
    [SerializeField] private NavMeshAgent navMeshAgent;
    private BehaviourTreeState behaviourTreeState = null;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    private void Start()
    {
        if(behaviourTree != null)
        {
            behaviourTreeState = new BehaviourTreeState(behaviourTree);
            behaviourTreeState.Owner = this;
        }
        if(navMeshAgent == null)
        {
            navMeshAgent.Warp(transform.position);
        }
    }

    void Update()
    {
        if(behaviourTree != null)
        {
            behaviourTree.Run(behaviourTreeState);
        }
    }
}
