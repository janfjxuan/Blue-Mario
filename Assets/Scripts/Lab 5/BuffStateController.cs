using UnityEngine;

public class BuffStateController : StateController
{
    public PowerupType currentPowerupType = PowerupType.Default;

    public bool IsStarred => currentState != null && currentState.name == "StarBuff";

    public override void Start()
    {
        base.Start();
        GameRestart();
    }

    public void GameRestart()
    {
        currentPowerupType = PowerupType.Default;
        TransitionToState(startState);
    }

    public void SetPowerup(PowerupType p)
    {
        currentPowerupType = p;
    }
}