using System.Collections;
using UnityEngine;
public abstract class BasePowerup : MonoBehaviour, IPowerup
{
    public PowerupType type;
    public bool spawned = false;
    protected bool consumed = false;

    // base methods
    protected virtual void Start()
    {

    }

    // interface methods
    // 1. concrete methods
    public PowerupType powerupType
    {
        get // getter
        {
            return type;
        }
    }

    public bool hasSpawned
    {
        get // getter
        {
            return spawned;
        }
    }

    public void SetActive()
    {
        gameObject.SetActive(spawned);
    }

    // 2. abstract methods, must be implemented by derived classes
    public abstract void DestroyPowerup();
    public abstract void SpawnPowerup();
    public abstract void ApplyPowerup(MonoBehaviour i);
    public abstract void ResetPowerup();
}
