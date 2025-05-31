using UnityEngine;

public class WaitUntilFarEnough : ActionNode
{
    [SerializeField] private float minDistanceToResume = 8f;
    private Transform playerTransform;

    protected override void OnStart(BehaviourTreeState state)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (playerTransform == null)
            return State.Failure;

        float dist = Vector3.Distance(state.Owner.transform.position, playerTransform.position);

        if (dist >= minDistanceToResume)
        {
            return State.Success; // Player is far enough — resume following
        }

        return State.Running; // Keep waiting
    }

    protected override void OnStop(BehaviourTreeState state) { }
}
