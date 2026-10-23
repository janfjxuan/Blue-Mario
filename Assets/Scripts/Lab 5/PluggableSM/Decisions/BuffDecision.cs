using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/BuffPowerup")]
public class BuffPowerupDecision : Decision
{
    public PowerupType buff = PowerupType.StarMan;

    public override bool Decide(StateController controller)
    {
        return ((BuffStateController)controller).currentPowerupType == buff;
    }
}