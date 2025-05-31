using System.Globalization;
using UnityEngine;

public class Spin : DecoratorNode
{
    protected override void OnStart(BehaviourTreeState state)
    {

    }

    protected override void OnStop(BehaviourTreeState state)
    {

    }

    protected override State OnUpdate(BehaviourTreeState state1)
    {
        if(Child == null)
        {
            return State.Failure;
        }

        return Child.Run(state1);
    }
}
