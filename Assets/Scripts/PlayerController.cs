using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IPowerupApplicable
{
    private MarioStateController marioStateController;
    private BuffStateController buffStateController;
    private SpriteRenderer marioSprite;
    public GameConstants gameConstants;
    private bool transforming = false;
    private bool inputLocked = false;
    public bool startWithCastleWalk = false;
    private BoxCollider2D box;
    private Vector2 smallSize;
    private Vector2 smallOffset;
    public AudioSource marioAudio;
    public Animator marioAnimator;
    public Collider2D marioCollider;
    public Rigidbody2D marioBody;
    public Vector3 marioStartingPosition;
    public UnityEvent onGameOver;

    private bool jumpHeld;
    public bool onGroundState = true;
    public bool faceRightState = true;
    public BoolVariable marioFaceRight;

    // state
    [System.NonSerialized]
    public bool alive = true;
    private bool moving = false;

    // pipe transition
    private int defaultSortingLayerID;
    private int defaultSortingOrder;
    public bool InPipe { get; set; }
    private float defaultGravity;

    // read only states
    public bool IsMoving => moving;
    public bool FacingRight => faceRightState;
    public bool IsGrounded => onGroundState;
    public bool IsTransforming => transforming;
    public float DefaultGravity => defaultGravity;
    public bool IsSuper => marioStateController.currentState.name == "SuperMario" || marioStateController.currentState.name == "FireMario";
    // to do clean up states name to enum

    // audio
    public AudioClip marioDeathClip;
    public AudioClip marioJumpClip;
    public AudioClip marioSuperJumpClip;

    // hash
    private static readonly int IsStarHash = Animator.StringToHash("isStar");
    private readonly HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();

    // level end
    public bool InLevelEnd { get; private set; }

    [SerializeField] LayerMask groundLayers = (1 << 3) | (1 << 7);

    void Awake()
    {
        box = marioCollider as BoxCollider2D;
        smallSize = box.size;
        smallOffset = box.offset;
        marioStateController = GetComponent<MarioStateController>();
        buffStateController = GetComponent<BuffStateController>();
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        defaultGravity = GetComponent<Rigidbody2D>().gravityScale;
        defaultSortingLayerID = marioSprite.sortingLayerID;
        defaultSortingOrder = marioSprite.sortingOrder;
    }
    // Start is called before the first frame update
    void Start()
    {
        marioCollider.enabled = true;
        marioAnimator.SetBool("onGround", onGroundState);
        marioStartingPosition = marioBody.transform.position;

        if (startWithCastleWalk) StartCoroutine(WalkOutOfCastle());

        // subscribe to scene manager scene change
        // SceneManager.activeSceneChanged += SetStartingPosition;
    }

    public void PlayDeathImpulse()
    {
        marioBody.gravityScale = gameConstants.deathGravityScale;
        marioBody.linearVelocity = new Vector2(0f, 0f);
        marioBody.AddForce(Vector2.up * gameConstants.deathImpulse, ForceMode2D.Impulse);
    }

    void CheckGrounded(Collision2D col)
    {
        if ((groundLayers.value & (1 << col.gameObject.layer)) == 0) return;
        if (marioBody.linearVelocity.y > 0.1f) return;

        foreach (ContactPoint2D c in col.contacts)
        {
            if (c.normal.y > 0.5f)
            {
                groundContacts.Add(col.collider);
                if (!onGroundState)
                {
                    onGroundState = true;
                    marioAnimator.SetBool("onGround", true);
                }
                return;
            }
        }
    }

    private bool IsStomp(Collision2D col)
    {
        Bounds mario = marioCollider.bounds;
        Bounds enemy = col.collider.bounds;

        // feet above this fraction of the enemy's height counts as landing on top
        float threshold = enemy.min.y + enemy.size.y * gameConstants.stompHeightFraction;
        if (mario.min.y > threshold) return true;

        foreach (ContactPoint2D c in col.contacts)
            if (c.normal.y > 0.5f) return true;

        return false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        CheckGrounded(col);
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyMovement enemy = col.gameObject.GetComponent<EnemyMovement>();

            if (IsStomp(col))
            {
                enemy.Stomp();
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocityX, 0f);
                marioBody.AddForce(Vector2.up * gameConstants.stompBounceVelocity * 0.6f, ForceMode2D.Impulse);
            }
            else if (buffStateController.IsStarred)
            {
                enemy.Yeet(col.transform.position.x > transform.position.x);
            }
            else
            {
                DamageMario();
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        CheckGrounded(col);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (groundContacts.Remove(col.collider) && groundContacts.Count == 0 && onGroundState)
        {
            onGroundState = false;
            // marioAnimator.SetBool("onGround", false);
        }
    }

    public void ClearGroundContacts()
    {
        groundContacts.Clear();
        onGroundState = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GapHole"))
        {
            marioStateController.SetPowerup(PowerupType.Kill);
        }
    }

    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            updateMarioShouldFaceRight(false);
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
        else if (value == 1 && !faceRightState)
        {
            updateMarioShouldFaceRight(true);
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    private void updateMarioShouldFaceRight(bool value)
    {
        faceRightState = value;
        marioFaceRight.SetValue(faceRightState);
    }

    public void FaceRight(bool right)
    {
        FlipMarioSprite(right ? 1 : -1);
    }

    public void SetSortingBehind(SpriteRenderer target)
    {
        marioSprite.sortingLayerID = target.sortingLayerID;
        marioSprite.sortingOrder = target.sortingOrder - 1;
    }

    public void RestoreSorting()
    {
        marioSprite.sortingLayerID = defaultSortingLayerID;
        marioSprite.sortingOrder = defaultSortingOrder;
    }

    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
        else if (alive)
        {
            float oldX = marioBody.linearVelocity.x;
            float newX = Mathf.MoveTowards(oldX, 0f, gameConstants.deceleration * Time.fixedDeltaTime);
            marioBody.linearVelocity = new Vector2(newX, marioBody.linearVelocity.y);
        }
        if (alive)
        {
            float y = marioBody.linearVelocity.y;

            if (y > 0f && jumpHeld)
                marioBody.gravityScale = gameConstants.lowGravityScale;
            else
                marioBody.gravityScale = gameConstants.highGravityScale;

            if (y < gameConstants.maxFallSpeed)
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, gameConstants.maxFallSpeed);
        }
    }

    void Move(int value)
    {
        float speedX = marioBody.linearVelocity.x;
        // allow force if under max speed, OR if pushing against current motion
        if (Mathf.Abs(speedX) < gameConstants.maxSpeed || Mathf.Sign(speedX) != value)
        {
            marioBody.AddForce(new Vector2(value, 0) * gameConstants.speed);
        }
    }

    public void MoveCheck(int value)
    {
        if (!alive || inputLocked) return;

        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState && !inputLocked)
        {
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, gameConstants.jumpVelocity);
            groundContacts.Clear();
            onGroundState = false;
            jumpHeld = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
            PlaySfx(IsSuper ? marioSuperJumpClip : marioJumpClip);
        }
    }

    public void JumpRelease()
    {
        jumpHeld = false;
    }

    public void SetColliderSmall()
    {
        box.size = smallSize;
        box.offset = smallOffset;
    }

    public void SetColliderSuper()
    {
        box.size = new Vector2(smallSize.x, smallSize.y * 2f);
        box.offset = new Vector2(smallOffset.x, smallOffset.y + smallSize.y / 2f);
    }

    public void DamageMario()
    {
        if (InLevelEnd) return;
        if (buffStateController.IsStarred) return;

        if (marioStateController.currentState.name == "InvincibleSmallMario") return;
        marioStateController.SetPowerup(PowerupType.Damage);
    }

    public void KillMario()
    {
        if (alive)
        {
            marioCollider.enabled = false;
            PlaySfx(marioDeathClip);
            alive = false;
        }
    }

    public void RequestPowerupEffect(IPowerup i)
    {
        i.ApplyPowerup(this);
    }

    public void PlayTransformAnimation(string stateName, float duration, AudioClip sfx)
    {
        if (sfx != null) marioAudio.PlayOneShot(sfx);
        StartCoroutine(TransformRoutine(stateName, duration));
    }

    public void SetStarAnimator(bool value)
    {
        foreach (var p in marioAnimator.parameters)
        {
            if (p.nameHash == IsStarHash)
            {
                marioAnimator.SetBool(IsStarHash, value);
                return;
            }
        }
    }

    private IEnumerator TransformRoutine(string stateName, float duration)
    {
        transforming = true;
        Time.timeScale = 0f;

        marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        marioAnimator.Play(stateName);

        yield return new WaitForSecondsRealtime(duration);

        marioAnimator.updateMode = AnimatorUpdateMode.Normal;
        RefreshAnimatorState();   // Rebind, back to idle, restore onGround
        Time.timeScale = 1f;
        transforming = false;
    }

    public void BeginLevelEnd()
    {
        if (InLevelEnd) return;
        InLevelEnd = true;
        SetInputLocked(true);
    }

    public void StartCastleWalk()
    {
        if (!InLevelEnd || !alive) return;
        StartCoroutine(CastleWalkRoutine());
    }

    private IEnumerator CastleWalkRoutine()
    {
        float timeout = 3f;
        while (!onGroundState && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        FlipMarioSprite(1); 
        moving = true; 

        float walkTimeout = 8f;
        while (moving && walkTimeout > 0f)
        {
            walkTimeout -= Time.unscaledDeltaTime;
            yield return null;
        }
        moving = false;
    }

    public void ArriveAtCastle()
    {
        moving = false;
        marioBody.linearVelocity = Vector2.zero;
    }

    private IEnumerator WalkOutOfCastle()
    {
        inputLocked = true;
        FlipMarioSprite(1);
        moving = true;

        float targetX = transform.position.x + 2f;
        float timeout = 3f;
        while (transform.position.x < targetX && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
        moving = false;
        inputLocked = false;
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
        if (locked) moving = false;
    }

    public void ResetMovementState()
    {
        moving = false;
        jumpHeld = false;
        onGroundState = true;
    }

    public void GameRestart()
    {
        StopAllCoroutines();
        inputLocked = false;
        Time.timeScale = 1f;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;
        InLevelEnd = false;

        // reset position
        InPipe = false;
        marioBody.gravityScale = defaultGravity;
        marioBody.transform.position = marioStartingPosition;
        marioBody.transform.rotation = Quaternion.identity;
        marioBody.linearVelocity = Vector2.zero;
        marioBody.angularVelocity = 0f;
        ResetMovementState();
        groundContacts.Clear();
        onGroundState = true;

        // reset sprite
        updateMarioShouldFaceRight(true);
        marioSprite.flipX = false;
        RestoreSorting();

        // reset animation
        marioCollider.enabled = true;
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        if (startWithCastleWalk) StartCoroutine(WalkOutOfCastle());
    }

    public void RefreshAnimatorState()
    {
        marioAnimator.Rebind();
        SetStarAnimator(buffStateController != null && buffStateController.IsStarred);
        marioAnimator.SetBool("onGround", onGroundState);
        marioAnimator.Update(0f);
    }

    public void GameOverScene()
    {
        onGameOver.Invoke();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip != null) marioAudio.PlayOneShot(clip);
    }
}