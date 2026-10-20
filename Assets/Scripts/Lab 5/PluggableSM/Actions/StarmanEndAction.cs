using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/EndStarman")]
public class StarmanEndAction : Action
{
    public override void Act(StateController controller)
    {
        ((MarioStateController)controller).EndStar();
    }
}