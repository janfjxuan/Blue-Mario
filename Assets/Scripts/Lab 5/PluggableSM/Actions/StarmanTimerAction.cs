using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/StarmanTimer")]
public class StarmanTimerAction : Action
{
    public float duration = 8f;

    public override void Act(StateController controller)
    {
        if (controller.CheckIfCountDownElapsed(duration) && controller.previousState != null)
            controller.TransitionToState(controller.previousState);
    }
}