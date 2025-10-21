using UnityEngine;


    public class IsQPressed:DecoratorNode
    {
    protected override void OnStart(BehaviourTreeState state) { }
    protected override void OnStop(BehaviourTreeState state) { }
    protected override State OnUpdate(BehaviourTreeState state)
    {
        if (Input.GetKey(KeyCode.Q))
        {
            return State.Success; // If Q is pressed, don't do walk sequence
        }

        return State.Failure; // Otherwise run the child
    }
}


