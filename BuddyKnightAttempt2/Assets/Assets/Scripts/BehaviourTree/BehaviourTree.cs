using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu()]
public sealed class BehaviourTree : ScriptableObject
{
    public Node RootNode;
    public State TreeState = State.Running;
    public List<Node> Nodes = new List<Node>();

    public State Run(BehaviourTreeState state)
    {
        if(RootNode == null)
        {
            return State.Failure;
        }

        var rootNodeData = state.GetNodeData(RootNode);
        if (rootNodeData.State == State.Running)
        {
            return RootNode.Run(state);
        }

        return rootNodeData.State;
    }

    public Node CreateNode(System.Type type)
    {
        var node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.GenUniqueId = GUID.Generate().ToString();

        Nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        AssetDatabase.SaveAssets();
        return node;
    }

    public void RemoveNode(Node node)
    {
        Nodes.Remove(node); 
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        if (parent is DecoratorNode decoratorNode)
        {
            decoratorNode.Child = child;
            EditorUtility.SetDirty(decoratorNode);
        }
        else if (parent is CompositeNode compositeNode)
        {
            compositeNode.Children.Add(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        if (parent is DecoratorNode decoratorNode)
        {
            decoratorNode.Child = null;
            EditorUtility.SetDirty(decoratorNode);
        }
        else if (parent is CompositeNode compositeNode)
        {
            compositeNode.Children.Remove(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();

        if (parent is DecoratorNode decoratorNode)
        {
            if(decoratorNode.Child != null)
                children.Add(decoratorNode.Child);
        }
        else if (parent is CompositeNode compositeNode)
        {
            children = compositeNode.Children;
        }

        return children;
    }

}
