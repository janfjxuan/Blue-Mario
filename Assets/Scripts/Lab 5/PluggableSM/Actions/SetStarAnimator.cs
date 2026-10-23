using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetStarAnimator")]
public class SetStarAnimator : Action
{
    public bool starOn;

    public override void Act(StateController controller)
    {
        controller.GetComponent<PlayerController>().SetStarAnimator(starOn);
    }

}
