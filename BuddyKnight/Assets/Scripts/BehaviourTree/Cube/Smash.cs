using UnityEngine;

public class Smash : ActionNode
{

    [SerializeField] private Vector3 localTargetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private string meshObjectName = "SmashingCube"; // Child name

    private Transform meshTransform; 
    protected override void OnStart(BehaviourTreeState state)
    {
        if (meshTransform == null)
        {
            var child = state.Owner.transform.Find(meshObjectName);
            if (child != null)
                meshTransform = child;
            else
                Debug.LogWarning($"Could not find child mesh '{meshObjectName}' on {state.Owner.name}");
        }
      
    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        // Wait for the smash to complete

        if (meshTransform == null)
            return State.Failure;

        // Move the mesh toward the local target position
        meshTransform.localPosition = Vector3.MoveTowards(
            meshTransform.localPosition,
            localTargetPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if we reached target
        if (Vector3.Distance(meshTransform.localPosition, localTargetPosition) < 0.01f)
            return State.Success;

        return State.Running;
    }

    protected override void OnStop(BehaviourTreeState state)
    {
       
    }
}
