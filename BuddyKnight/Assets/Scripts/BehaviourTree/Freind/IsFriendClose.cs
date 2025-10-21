using UnityEngine;

public class IsFriendClose : ActionNode
{
    [SerializeField] private float distanceThresh = 3f;

    protected override void OnStart(BehaviourTreeState state)
    {
        // You could optionally assign the playerTransform here,
        // but only if it hasn't been set already (i.e., first time).
        if (state.PlayerTransform == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null)
            {
                state.PlayerTransform = target.transform;
            }
        }
    }

    protected override void OnStop(BehaviourTreeState state)
    {
    }

    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (state.PlayerTransform == null)
        {
            return State.Failure;
        }

        float dist = Vector3.Distance(state.Owner.transform.position, state.PlayerTransform.position);

        if (dist <= distanceThresh)
        {
            state.Owner.NavMeshAgent.ResetPath();
            return State.Success;
        }

        return State.Failure;
    }
}
