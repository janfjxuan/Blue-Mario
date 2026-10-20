using UnityEngine;

public class CoinPowerup : BasePowerup
{
    private Animator coinAnimator;
    public IntVariable gameCoins;
    public SimpleGameEvent onCoinCollected;

    private void Awake()
    {
        coinAnimator = GetComponent<Animator>();
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
        if (!spawned)
        {
            spawned = true;
            SetActive();
            gameCoins.SetValue(gameCoins.Value + 1);
            onCoinCollected.Raise();
        }
        coinAnimator.SetTrigger("spawn");
    }

    public override void DestroyPowerup()
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
        gameObject.SetActive(true);
        coinAnimator.ResetTrigger("spawn");
        coinAnimator.Rebind();
        coinAnimator.Update(0f);
        SetActive();
    }
}