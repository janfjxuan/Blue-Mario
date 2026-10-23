using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/StarStart")]
public class StarStartAction : Action
{
    public AudioClip starClip;

    public override void Act(StateController controller)
    {
        if (starClip != null)
            controller.GetComponent<PlayerController>().marioAudio.PlayOneShot(starClip);
    }
}