using UnityEngine;

public class StarmanPowerup : BasePowerup
{
    public PowerupGameEvent powerupCollected;
    // setup this object's type
    // instantiate variables
    private Collider2D starmanCollider;
    private Animator starmanAnimator;
    private void Awake()
    {
        starmanAnimator = GetComponent<Animator>();
        starmanCollider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.StarMan;
        starmanAnimator.enabled = true;
        starmanCollider.enabled = false;
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
    public override void DestroyPowerup()
    {
        
    }
    public override void SpawnPowerup()
    {
        spawned = true;
        starmanAnimator.enabled = false;
        starmanCollider.enabled = true;
        rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse); // move to the right
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
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
        starmanAnimator.enabled = true;
        starmanCollider.enabled = false;
        gameObject.SetActive(false);
    }
}