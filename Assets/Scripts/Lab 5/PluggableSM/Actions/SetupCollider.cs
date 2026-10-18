using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupCollider")]
public class SetupColliderAction : Action
{
    public bool superSize;

    public override void Act(StateController controller)
    {
        var playerController = controller.GetComponent<PlayerController>();
        if (superSize) playerController.SetColliderSuper();
        else playerController.SetColliderSmall();
    }
}