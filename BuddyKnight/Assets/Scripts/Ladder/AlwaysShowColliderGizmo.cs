using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class AlwaysShowColliderGizmo:MonoBehaviour
{
    
}

// Draw gizmos for any object that has this component
[CustomEditor(typeof(AlwaysShowColliderGizmo))]
public class AlwaysShowColliderGizmoEditor : Editor
{
    void OnSceneGUI()
    {
        var t = (target as AlwaysShowColliderGizmo).transform;

        Collider col = t.GetComponent<Collider>();
        if (col == null) return;

        Handles.color = Color.green;

        if (col is BoxCollider box)
        {
            Handles.DrawWireCube(
                t.position + box.center,
                box.size
            );
        }
        else if (col is CapsuleCollider capsule)
        {
            // Basic visualization
            Handles.DrawWireDisc(
                t.position + capsule.center,
                t.up,
                capsule.radius
            );
        }
    }
}
