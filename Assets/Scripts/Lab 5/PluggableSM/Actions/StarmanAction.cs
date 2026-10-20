using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupStarman")]
public class StarmanAction : Action
{
    public AudioClip starmanStart;

    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        if (starmanStart != null)
            m.GetComponent<AudioSource>().PlayOneShot(starmanStart);
        m.SetPowerup(PowerupType.Default);

        m.StartStar();
    }
}