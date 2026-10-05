using UnityEngine;

public interface Powerup
{
    void DestroyPowerup();
    void SpawnPowerup();
    void ApplyPowerup(MonoBehaviour i);

    PowerupType powerupType
    {
        get;
    }

    bool hasSpawned
    {
        get;
    }
}


public interface PowerupApplicable
{
    public void RequestPowerupEffect(Powerup i);
}