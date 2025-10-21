using UnityEngine;


public class NotNode : DecoratorNode
{
    

    protected override void OnStart(BehaviourTreeState state)
    {

    }
    protected override void OnStop(BehaviourTreeState state)
    {
        //  Debug.Log("SequenceStop");
        //throw new System.NotImplementedException();
    }
    protected override State OnUpdate(BehaviourTreeState state)

    {
        if (Child == null)
        {
           
            return State.Failure;
        }
        var child = Child;
         
        var childState = child.Run(state);

        switch (childState)
        {
            case State.Running:

                return State.Running;
            case State.Failure: return State.Success;
            case State.Success: return State.Failure;
        }


       
        return State.Success;

    }
}

