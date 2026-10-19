using UnityEngine;

public class FireFlowerPowerup : BasePowerup
{
    public PowerupGameEvent powerupCollected;
    // setup this object's type
    // instantiate variables
    private Collider2D fireFlowerCollider;
    private Animator fireFlowerAnimator;
    private void Awake()
    {
        fireFlowerAnimator = GetComponent<Animator>();
        fireFlowerCollider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.FireFlower;
        fireFlowerAnimator.enabled = true;
        fireFlowerCollider.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (spawned)
                powerupCollected.Raise(this);
            gameObject.SetActive(false);
        }
    }

    // interface implementation
    public override void DestroyPowerup()
    {
        
    }
    public override void SpawnPowerup()
    {
        spawned = true;
        fireFlowerAnimator.enabled = false;
        fireFlowerCollider.enabled = true;
    }


    // interface implementation
    // public override void ApplyPowerup(MonoBehaviour i)
    // {
    //     if (i.TryGetComponent<PlayerController>(out var mario))
    //         mario.MakeSuperMario();
    // }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        // base.ApplyPowerup(i);
        // try
        MarioStateController mario;
        bool result = i.TryGetComponent<MarioStateController>(out mario);
        if (result)
        {
            mario.SetPowerup(this.powerupType);
        }
    }

    public override void ResetPowerup()
    {
        spawned = false;
        fireFlowerAnimator.enabled = true;
        fireFlowerCollider.enabled = false;
        gameObject.SetActive(false);
    }
}