using UnityEngine;

public class CoinPowerup : BasePowerup
{
    private Animator coinAnimator;
    public SimpleGameEvent onCoinCollected;
    public int coinsInBlock = 1;
    private int coinsLeft;

    private void Awake()
    {
        coinAnimator = GetComponent<Animator>();
        coinsLeft = coinsInBlock;
    }
    // start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        type = PowerupType.Coin;
    }

    public void HideCoin()
    {
        gameObject.SetActive(false);
    }

    public override void SpawnPowerup()
    {
        if (spawned) return;
        gameObject.SetActive(true);
        if (coinsLeft <= 0) return;

        coinsLeft--;
        if (coinsLeft == 0) spawned = true;
        
        onCoinCollected.Raise();
        coinAnimator.Rebind();
        coinAnimator.Update(0f);
        coinAnimator.SetTrigger("spawn");
    }

    public override void DestroyPowerup()
    {

    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        
    }

    public override void ResetPowerup()
    {
        spawned = false;
        coinsLeft = coinsInBlock;
        gameObject.SetActive(true);
        coinAnimator.ResetTrigger("spawn");
        coinAnimator.Rebind();
        coinAnimator.Update(0f);
        SetActive();
    }
}