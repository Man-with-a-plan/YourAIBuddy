using UnityEngine;

public class AiAgent : MonoBehaviour
{
    [SerializeField] private BehaviourTree behaviourTree;

    private BehaviourTreeState behaviourTreeState = null;

    private void Start()
    {
        if(behaviourTree != null)
        {
            behaviourTreeState = new BehaviourTreeState(behaviourTree);
            behaviourTreeState.Owner = this;
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
