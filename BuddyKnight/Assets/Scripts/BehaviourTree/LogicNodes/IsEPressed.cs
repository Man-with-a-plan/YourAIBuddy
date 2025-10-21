using UnityEngine;


    public class IsEPressed:ActionNode
    {
    [SerializeField] bool toRun;
    protected override void OnStart(BehaviourTreeState state) {
       
    }
    protected override void OnStop(BehaviourTreeState state) { }
    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (Input.GetKey(KeyCode.E))
        {
           
                return State.Success; // If Q is pressed, don't do walk sequence
         
            }

        if (!toRun)
        {
            return State.Failure;
        }

        else
        {
            return State.Running;
        }
    }
}

