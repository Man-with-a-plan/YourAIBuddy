using System.Collections.Generic;
using System;


public class BehaviourTreeState
{
    private Dictionary<string, NodeData> data = new Dictionary<string, NodeData>();

    public AiAgent Owner;

    public BehaviourTreeState(BehaviourTree behaviourTree)
    {
        foreach(var node in behaviourTree.Nodes)
        {
            data.Add(node.GenUniqueId, node.CreateNodeData());
        }
    }

    public NodeData GetNodeData(Node node)
    {
        if(data.TryGetValue(node.GenUniqueId, out var d))
        {
            return d;
        }

        return null;
    }
}