using UnityEngine;

public abstract class DecoratorNode : Node
{
    [HideInInspector] public Node Child; 
}
