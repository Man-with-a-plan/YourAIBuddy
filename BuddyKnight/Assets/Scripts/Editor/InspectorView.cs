using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using Unity.UI.Builder; // Optional, for UI Builder support

[UxmlElement]
public partial class InspectorView : VisualElement
{
   

    public InspectorView()
    {
      
    }

    private UnityEditor.Editor editor;

    internal void UpdateSelection(NodeView nodeView)
    {
        Debug.Log("UpdateSelection called with: " + nodeView?.Node?.name);
        Clear();
        if (editor != null)UnityEngine.Object.DestroyImmediate(editor);

        if (nodeView?.Node == null)
        {
            Debug.LogWarning("Node is null");
            return;
        }
        UnityEngine.Object.DestroyImmediate(editor);

        editor = Editor.CreateEditor(nodeView.Node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {

            if (editor.target != null)
            {
                editor.OnInspectorGUI();
            }
        });
        
        Add(container);
    }
}