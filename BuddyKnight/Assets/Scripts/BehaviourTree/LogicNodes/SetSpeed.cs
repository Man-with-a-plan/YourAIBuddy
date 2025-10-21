using UnityEngine;



    public class SetSpeed:ActionNode
    {
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 7f;

    protected override void OnStart(BehaviourTreeState state) { }
    protected override void OnStop(BehaviourTreeState state) { }
    protected override State OnUpdate(BehaviourTreeState state)
    {
       
        if (Input.GetKey(KeyCode.Q))
        {
            state.MovementSpeed = runSpeed;
        }
        else

        {
           
            state.MovementSpeed = walkSpeed;
        }

        return State.Running;
    }
}


