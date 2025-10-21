using UnityEngine;


    public class MoveOnY:ActionNode
    {
    [SerializeField] private Vector3 localTargetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private string meshObjectName = "Cube";
    private Transform CubeTransform;
    protected override void OnStart(BehaviourTreeState state)
    {
        if (CubeTransform == null)
        {
            var child = state.Owner.transform.Find(meshObjectName);
            if (child != null)
                CubeTransform = child;
            else
                Debug.LogWarning($"Could not find child mesh '{meshObjectName}' on {state.Owner.name}");
        }

    }
    protected override void OnStop(BehaviourTreeState state) { }
    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (CubeTransform == null)
            return State.Failure;

        // Move the mesh toward the local target position
        CubeTransform.localPosition = Vector3.MoveTowards(
            CubeTransform.localPosition,
            localTargetPosition,
            moveSpeed * Time.deltaTime
        );
        if (Vector3.Distance(CubeTransform.localPosition, localTargetPosition) < 0.01f)
            return State.Success;

        return State.Running;
    }
    }

