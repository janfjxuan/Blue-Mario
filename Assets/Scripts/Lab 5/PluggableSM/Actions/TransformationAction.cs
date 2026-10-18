using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/TransformAnimation")]
public class TransformAnimationAction : Action
{
    public string animatorStateName;
    public float duration = 1f;
    public AudioClip audioClip;

    public override void Act(StateController controller)
    {
        controller.GetComponent<PlayerController>().PlayTransformAnimation(animatorStateName, duration, audioClip);
    }
}