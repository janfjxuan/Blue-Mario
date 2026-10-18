using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    public PowerupGameEvent powerupCollected;
    // setup this object's type
    // instantiate variables
    private Collider2D magicMushroomCollider;
    private Animator magicMushroomAnimator;
    private void Awake()
    {
        magicMushroomAnimator = GetComponent<Animator>();
        magicMushroomCollider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.MagicMushroom;
        magicMushroomAnimator.enabled = true;
        magicMushroomCollider.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (spawned)
                powerupCollected.Raise(this);
            gameObject.SetActive(false);
        }
        else if (col.gameObject.layer == 10) // else if hitting Pipe, flip travel direction
        {
            if (spawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);

            }
        }
    }

    // interface implementation
    public override void SpawnPowerup()
    {
        spawned = true;
        magicMushroomAnimator.enabled = false;
        magicMushroomCollider.enabled = true;
        rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse); // move to the right
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
        magicMushroomAnimator.enabled = true;
        magicMushroomCollider.enabled = false;
        gameObject.SetActive(false);
    }
}