using UnityEngine;


public class IsFriendClose : ActionNode
{
    [SerializeField] private float distanceThresh = 3f;
    private Transform playerTransform;

    protected override void OnStart(BehaviourTreeState state)
    {
        // Find target by tag once at start
        if (playerTransform == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null)
            {
                playerTransform = target.transform;
            }
        }
    }
    protected override void OnStop(BehaviourTreeState state)
    {
    }
    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (playerTransform == null)
        {
            return State.Failure;
        }

        float dist = Vector3.Distance(state.Owner.transform.position, playerTransform.position);

        if (dist <= distanceThresh)
        {
            Debug.Log("stop, oaaaaaaaaa");
            return State.Success;
        }

        return State.Running;

    }

       


}