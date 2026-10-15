using UnityEngine;
using UnityEngine.Events;

public class PowerupManager : Singleton<PowerupManager>
{
    public UnityEvent<IPowerup> powerupCollected = new UnityEvent<IPowerup>();
    public UnityEvent<IPowerup> powerupAffectsPlayer = new UnityEvent<IPowerup>();
    public UnityEvent<IPowerup> powerupAffectsManager = new UnityEvent<IPowerup>();

    public override void Awake()
    {
        base.Awake();
        powerupCollected.AddListener(FilterAndCastPowerup);
    }

    void FilterAndCastPowerup(IPowerup i)
    {
        switch (i.powerupType)
        {
            case PowerupType.MagicMushroom:
                powerupAffectsPlayer.Invoke(i);
                break;
            // later: coin -> powerupAffectsManager, starman -> player, etc.
            default:
                Debug.LogWarning("No handler for powerup type: " + i.powerupType);
                break;
        }
    }
}