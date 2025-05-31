using UnityEngine;

public abstract class Node : ScriptableObject
{
    [HideInInspector] public string GenUniqueId;
    [HideInInspector] public Vector2 Position;

    protected abstract void OnStart(BehaviourTreeState state);
    protected abstract void OnStop(BehaviourTreeState state);
    protected abstract State OnUpdate(BehaviourTreeState state);

    public virtual NodeData CreateNodeData()
    {
        return new NodeData();
    }

    public State Run(BehaviourTreeState state) 
    {
        var data = state.GetNodeData(this);

        if(data.Stopped)
        {
            OnStart(state);
            data.Stopped = false;
        }

        data.State = OnUpdate(state);

        if(data.State != State.Running)
        {
            OnStop(state);
            data.Stopped = true;
        }

        return data.State;
    }
}