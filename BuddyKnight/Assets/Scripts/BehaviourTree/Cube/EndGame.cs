using UnityEngine;

public class EndGameNode : ActionNode
{
    [SerializeField] private string LogDisplay = "You Loose"; // Child name
    protected override void OnStart(BehaviourTreeState state) { }
    protected override void OnStop(BehaviourTreeState state) { }
    protected override State OnUpdate(BehaviourTreeState state)
    {
        Debug.Log(LogDisplay);
        // Insert your game manager call here
        // GameManager.Instance.EndGame();
        return State.Running;
    }
}