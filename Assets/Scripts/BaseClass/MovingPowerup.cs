using System.Collections;
using UnityEngine;

public abstract class MovingPowerup : BasePowerup
{
    public PowerupGameEvent powerupCollected;

    [Header("Movement")]
    public float horizontalSpeed;
    private float riseHeight = 1f;
    private float riseDuration = 1f;

    [Header("Sorting")]
    private int orderRise = -1;
    private int orderFree = 3;

    protected Rigidbody2D powerupBody;
    protected Collider2D powerupCollider;
    protected SpriteRenderer powerupSprite;
    protected float defaultGravity;
    protected Vector2 startPosition;
    protected int direction = 1; // 1 = right, -1 = left
    protected bool moving = false; // false while rising, true after

    protected virtual void Awake()
    {
        powerupBody = GetComponent<Rigidbody2D>();
        powerupCollider = GetComponent<Collider2D>();
        powerupSprite = GetComponent<SpriteRenderer>();
        defaultGravity = powerupBody.gravityScale;
        startPosition = powerupBody.transform.position;
        gameObject.SetActive(false);
    }

    protected virtual void FixedUpdate()
    {
        if (!moving) return;
        powerupBody.linearVelocity = new Vector2(direction * horizontalSpeed, powerupBody.linearVelocity.y);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (spawned) powerupCollected.Raise(this);
            gameObject.SetActive(false);
            return;
        }
    }
    public override void SpawnPowerup()
    {
        spawned = true;
        moving = false;
        direction = 1;
        StartCoroutine(RiseRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        SetSortingOrder(orderRise);
        powerupCollider.enabled = false;
        powerupBody.gravityScale = 0f;
        powerupBody.linearVelocity = Vector2.zero;

        Vector2 start = powerupBody.position;
        Vector2 end = start + Vector2.up * riseHeight;
        float t = 0f;
        while (t < riseDuration)
        {
            t += Time.fixedDeltaTime;
            powerupBody.MovePosition(Vector2.Lerp(start, end, t / riseDuration));
            yield return new WaitForFixedUpdate();
        }

        SetSortingOrder(orderFree);
        powerupBody.gravityScale = defaultGravity;
        powerupCollider.enabled = true;
        moving = true;
    }

    public override void ResetPowerup()
    {
        StopAllCoroutines();
        spawned = false;
        moving = false;
        direction = 1;
        powerupBody.gravityScale = defaultGravity;
        powerupBody.linearVelocity = Vector2.zero;
        powerupCollider.enabled = false;
        SetSortingOrder(orderRise);
        powerupBody.transform.position = startPosition;
        gameObject.SetActive(false);
    }

    public override void DestroyPowerup() { }

    private void SetSortingOrder(int order)
    {
        if (powerupSprite != null) powerupSprite.sortingOrder = order;
    }
}