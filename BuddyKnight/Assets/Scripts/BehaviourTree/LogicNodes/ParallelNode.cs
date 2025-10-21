using UnityEngine;
using System.Globalization;

public class ParallelNode: CompositeNode
{
    public enum CompletionPolicy { AnySuccess, AllSuccess }
    [SerializeField] private CompletionPolicy policy = CompletionPolicy.AllSuccess;

    public override NodeData CreateNodeData() => new ParallelNodeData();
    protected override void OnStart(BehaviourTreeState state)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnStop(BehaviourTreeState state)
    {
        throw new System.NotImplementedException();
    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        bool anyRunning = false;
        int successCount = 0;

        foreach (var child in Children)
        {
            var result = child.Run(state);

            if (result == State.Running)
                anyRunning = true;
            else if (result == State.Success)
                successCount++;
            else if (result == State.Failure && policy == CompletionPolicy.AllSuccess)
                return State.Failure;
        }

        if (policy == CompletionPolicy.AnySuccess && successCount > 0)
            return State.Success;

        return anyRunning ? State.Running : State.Success;
    }
}

