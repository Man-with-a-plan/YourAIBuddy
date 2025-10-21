using UnityEngine;
using System.Globalization;

    public class SequenceNode: CompositeNode 
    {

    public override NodeData CreateNodeData()
    {
        return new SequenceNodeData();
    }
    
    protected override void OnStart(BehaviourTreeState state)
    {
        var data = state.GetNodeData(this) as SequenceNodeData;
        data.CurrentChildIndex = 0;
      //  Debug.Log("SequenceStart");
    }
    protected override void OnStop(BehaviourTreeState state)
    {
      //  Debug.Log("SequenceStop");
        //throw new System.NotImplementedException();
    }
    protected override State OnUpdate(BehaviourTreeState state)

    {
        var data = state.GetNodeData(this) as SequenceNodeData;
        while (data.CurrentChildIndex < Children.Count)
        {
            var child = Children[data.CurrentChildIndex];
            var childState = child.Run(state);
            switch (childState)
            {
                case State.Running:

                    return State.Running;
                case State.Failure: return State.Failure;
                case State.Success: data.CurrentChildIndex++; Debug.Log(data.CurrentChildIndex); break;
            }
           
        }
      
        return State.Success;
     }

    }

