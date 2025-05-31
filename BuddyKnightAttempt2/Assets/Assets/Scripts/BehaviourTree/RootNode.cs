using System.Globalization;
using UnityEngine;

public class RootNode : DecoratorNode
{
    protected override void OnStart(BehaviourTreeState state)
    {

    }

    protected override void OnStop(BehaviourTreeState state)
    {

    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        if(Child == null)
        {
            return State.Failure;
        }

        return Child.Run(state);
    }
}
