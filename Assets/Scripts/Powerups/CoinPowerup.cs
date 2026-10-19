using UnityEngine;

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
        base.Start();
        type = PowerupType.Coin;
    }

    public void HideCoin()
    {
        gameObject.SetActive(false);
    }

    public override void SpawnPowerup()
    {
        if(!spawned)
        {
            spawned = true;
            SetActive();
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