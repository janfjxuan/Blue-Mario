using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinPowerup : BasePowerup
{
    // start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.Coin;
    }

    public override void SpawnPowerup()
    {
        spawned = true;
        // play the sound
        AudioSource source = this.GetComponent<AudioSource>();
        source.PlayOneShot(source.clip);
        // invoke PowerupCollectedEvent
        // PowerUpManager.instance.powerupCollected.Invoke(this);
    }

    public new void DestroyPowerup()
    {

    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        GameManager manager;
        bool result = i.TryGetComponent<GameManager>(out manager);

        if (result)
        {
            // manager.IncreaseScore(i);
        }
    }
}