using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class CompositeNode : Node
{
    [SerializeField]
     public List<Node> Children = new List<Node>(); 


}
