using UnityEngine;

public class ShoePowerup : BasePowerup
{
    private Animator shoeAnimator;
    private void Awake()
    {
        shoeAnimator = GetComponent<Animator>();
    }
    // start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.Shoe;
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
        shoeAnimator.ResetTrigger("spawn");
        shoeAnimator.Rebind();
        shoeAnimator.Update(0f);
    }
}
