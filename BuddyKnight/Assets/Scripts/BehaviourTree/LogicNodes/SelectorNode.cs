using UnityEngine;

public class SelectorNode : CompositeNode
{
    public override NodeData CreateNodeData()
    {
        return new SelectorNodeData();
    }

    protected override void OnStart(BehaviourTreeState state)
    {
        var data = state.GetNodeData(this) as SelectorNodeData;
        data.CurrentChildIndex = 0;
    }

    protected override void OnStop(BehaviourTreeState state)
    {
        var data = state.GetNodeData(this) as SelectorNodeData;
        data.CurrentChildIndex = 0;
    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        var data = state.GetNodeData(this) as SelectorNodeData;

        while (data.CurrentChildIndex < Children.Count)
        {
            var child = Children[data.CurrentChildIndex];
            var childState = child.Run(state);

            switch (childState)
            {
                case State.Running:
                    return State.Running;

                case State.Success:
                    data.CurrentChildIndex = 0;
                    return State.Success;

                case State.Failure:
                    data.CurrentChildIndex++;
                    break;
            }
        }

        return State.Failure;
    }
}
