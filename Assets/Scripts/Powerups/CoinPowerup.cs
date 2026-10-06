using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinPowerup : BasePowerup
{
    private Animator coinAnimator;
    private void Awake()
    {
        coinAnimator = GetComponent<Animator>();
    }
    // start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.Coin;
    }

    public override void SpawnPowerup()
    {
        spawned = true;
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

    public override void ResetPowerup()
    {
        spawned = false;
        coinAnimator.ResetTrigger("spawn");
        coinAnimator.Rebind();
        coinAnimator.Update(0f);
    }
}