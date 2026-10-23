using System.Collections;
using UnityEngine;

public class FireFlowerPowerup : BasePowerup
{
    public PowerupGameEvent powerupCollected;
    // setup this object's type
    // instantiate variables
    private Rigidbody2D fireFlowerBody;
    private Collider2D fireFlowerCollider;
    private Vector2 startPosition;
    private float defaultGravity;
    private void Awake()
    {
        fireFlowerBody = GetComponent<Rigidbody2D>();
        startPosition = fireFlowerBody.transform.position;
        defaultGravity = fireFlowerBody.gravityScale;
        fireFlowerCollider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.FireFlower;
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
        StartCoroutine(RiseRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        fireFlowerCollider.enabled = false;
        fireFlowerBody.gravityScale = 0f;

        Vector2 start = fireFlowerBody.position;
        Vector2 end = start + Vector2.up * 1f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime;
            fireFlowerBody.MovePosition(Vector2.Lerp(start, end, t));
            yield return new WaitForFixedUpdate();
        }

        // hand over to physics
        fireFlowerBody.gravityScale = defaultGravity;
        fireFlowerCollider.enabled = true;
    }

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
        StopAllCoroutines();
        spawned = false;
        fireFlowerCollider.enabled = false;
        fireFlowerBody.transform.position = startPosition;
        fireFlowerBody.gravityScale = defaultGravity;
        gameObject.SetActive(false);
    }
}