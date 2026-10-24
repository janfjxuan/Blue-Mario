using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IPowerupApplicable
{
    private MarioStateController marioStateController;
    private BuffStateController buffStateController;
    public GameConstants gameConstants;
    private bool transforming = false;
    public bool IsTransforming => transforming;
    // private bool isSuper = false;
    public bool IsSuper => marioStateController.currentState.name == "SuperMario" || marioStateController.currentState.name == "FireMario";
    // private bool isInvincible = false;
    private bool inputLocked = false;
    public bool startWithCastleWalk = false;
    private BoxCollider2D box;
    private Vector2 smallSize;
    private Vector2 smallOffset;
    public Rigidbody2D marioBody;
    public SpriteRenderer marioSprite;
    public AudioSource marioAudio;
    public Animator marioAnimator;
    public Collider2D marioCollider;
    public Vector3 marioStartingPosition;
    public UnityEvent onGameOver;

    float deathImpulse;
    float upSpeed;
    float maxSpeed;
    float speed;
    float deceleration;
    // public float invincibleDuration = 2f;   // or put it in GameConstants
    public float blinkInterval = 0.1f;

    public bool onGroundState = true;
    public bool faceRightState = true;
    public BoolVariable marioFaceRight;

    // state
    [System.NonSerialized]
    public bool alive = true;
    private bool moving = false;
    private bool jumpedState = false;

    // audio
    public AudioClip marioDeathClip;
    public AudioClip marioJumpClip;
    public AudioClip marioSuperJumpClip;

    // hash
    private static readonly int IsStarHash = Animator.StringToHash("isStar");

    void Awake()
    {
        box = marioCollider as BoxCollider2D;
        smallSize = box.size;
        smallOffset = box.offset;
        marioStateController = GetComponent<MarioStateController>();
        buffStateController = GetComponent<BuffStateController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // Set constants
        speed = gameConstants.speed;
        maxSpeed = gameConstants.maxSpeed;
        deathImpulse = gameConstants.deathImpulse;
        upSpeed = gameConstants.upSpeed;
        deceleration = gameConstants.deceleration;

        marioStateController = GetComponent<MarioStateController>();
        marioCollider.enabled = true;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        // smallmarioStateController = marioAnimator.runtimeAnimatorController;
        marioStartingPosition = marioBody.transform.position;

        box = marioCollider as BoxCollider2D;
        smallSize = box.size;
        smallOffset = box.offset;

        // subscribe to scene manager scene change
        // SceneManager.activeSceneChanged += SetStartingPosition;
    }

    public void PlayDeathImpulse()
    {
        marioBody.linearVelocity = new Vector2(0f, 0f);
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void CheckGrounded(Collision2D col)
    {
        if (onGroundState) return;
        if ((((1 << 3) | (1 << 7)) & (1 << col.gameObject.layer)) == 0) return;
        if (marioBody.linearVelocity.y > 0.1f) return;

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                onGroundState = true;
                marioAnimator.SetBool("onGround", onGroundState);
                return;
            }
        }
    }

    // FixedUpdate is called 50 times a second
    void OnCollisionEnter2D(Collision2D col)
    {
        CheckGrounded(col);
        if (col.gameObject.CompareTag("Enemy"))
        {
            ContactPoint2D contact = col.GetContact(0);
            EnemyMovement enemy = col.gameObject.GetComponent<EnemyMovement>();

            if (contact.normal.y > 0.5f)
            {
                enemy.Stomp();
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocityX, 0f);
                marioBody.AddForce(Vector2.up * upSpeed * 0.6f, ForceMode2D.Impulse);
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
            float newX = Mathf.MoveTowards(oldX, 0f, deceleration * Time.fixedDeltaTime);
            marioBody.linearVelocity = new Vector2(newX, marioBody.linearVelocity.y);
        }
    }

    void Move(int value)
    {
        float speedX = marioBody.linearVelocity.x;
        // allow force if under max speed, OR if pushing against current motion
        if (Mathf.Abs(speedX) < maxSpeed || Mathf.Sign(speedX) != value)
        {
            marioBody.AddForce(new Vector2(value, 0) * speed);
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
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0f);
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
            PlaySfx(IsSuper ? marioSuperJumpClip : marioJumpClip);
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState && !inputLocked)
        {
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;
        }
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

    public void ResetMovementState()
    {
        moving = false;
        jumpedState = false;
        onGroundState = true;
    }

    public void GameRestart()
    {
        StopAllCoroutines();
        inputLocked = false;
        // growing = false;
        Time.timeScale = 1f;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;

        // reset position
        marioBody.transform.position = marioStartingPosition;
        marioBody.transform.rotation = Quaternion.identity;
        marioBody.linearVelocity = Vector2.zero;
        marioBody.angularVelocity = 0f;
        ResetMovementState();

        // reset sprite direction
        updateMarioShouldFaceRight(true);
        marioSprite.flipX = false;

        // reset animation
        // isSuper = false;
        // box.size = smallSize;
        // box.offset = smallOffset;
        // marioAnimator.runtimeAnimatorController = smallmarioStateController;

        // marioAnimator.SetBool("onGround", onGroundState);

        marioCollider.enabled = true;
        marioAnimator.SetTrigger("gameRestart");

        // isInvincible = false;
        // SetSpriteAlpha(1f);
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