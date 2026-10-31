using UnityEngine;

public class ShoePowerup : BasePowerup
{
    public PowerupGameEvent powerupCollected;
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
        shoeAnimator.SetTrigger("spawn");
        powerupCollected.Raise(this);
    }

    public override void DestroyPowerup()
    {

    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        if (i.TryGetComponent<BuffStateController>(out var buff))
        {
            buff.SetPowerup(PowerupType.Shoe);
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
