using System.Collections.Generic;
using System;
using UnityEngine;


public class BehaviourTreeState
{
    private Dictionary<string, NodeData> data = new Dictionary<string, NodeData>();

    public AiAgent Owner;
    public Transform PlayerTransform;
    public float MovementSpeed = 3.5f;

    public BehaviourTreeState(BehaviourTree behaviourTree)
    {
        foreach(var node in behaviourTree.Nodes)
        {
            Debug.Log("node");
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