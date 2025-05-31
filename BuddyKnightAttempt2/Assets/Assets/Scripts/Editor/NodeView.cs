using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected { get; set; }
    public Node Node { get; set; }
    public Port Input { get; set; }
    public Port Output { get; set; }

    public NodeView(Node node) : base($"{BehaviourTreeEditor.StyleSheetsLocation}/NodeView.uxml")
    {
        this.Node = node;
        this.title = node.name;
        this.viewDataKey = node.GenUniqueId;

        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInputPort();
        CreateOutputPort();
    }

    private void CreateInputPort()
    {
        if(Node is not RootNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }

        if(Input != null)
        {
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(Input);
        }
    }

    private void CreateOutputPort()
    {
        if(Node is CompositeNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if(Node is DecoratorNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }


        if(Output != null)
        {
            Output.portName = "";
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(Output);
        }
    }


    public override void OnSelected()
    {
        base.OnSelected();
        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.Position = new Vector2(newPos.xMin, newPos.yMin);
        EditorUtility.SetDirty(Node);
    }

    public void SortChildren()
    {
        if(Node is CompositeNode compositeNode)
        {
            compositeNode.Children.Sort(SortByHorizontal);
        }
    }

    private int SortByHorizontal(Node left, Node right)
    {
        return left.Position.x < right.Position.x ? -1 : 1;
    }
}