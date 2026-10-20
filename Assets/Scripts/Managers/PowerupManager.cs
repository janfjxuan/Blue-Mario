using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public PowerupGameEvent powerupAffectsPlayer;
    public PowerupGameEvent powerupAffectsManager;

    public void FilterAndCastPowerup(IPowerup i)
    {
        switch (i.powerupType)
        {
            case PowerupType.MagicMushroom:
                powerupAffectsPlayer.Raise(i);
                break;
            case PowerupType.FireFlower:
                powerupAffectsPlayer.Raise(i);
                break;
            case PowerupType.StarMan:
                powerupAffectsPlayer.Raise(i);
                break;
            default:
                break;
        }
    }
}