using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IPowerupApplicable
{
    MarioStateController marioStateController;
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
    public AudioSource marioDeathAudio;
    public AudioClip marioSuperJumpClip;
    // public AudioClip marioGrowClip;
    // public AudioClip marioShrinkClip;
    public Animator marioAnimator;
    public RuntimeAnimatorController supermarioStateController;
    private RuntimeAnimatorController smallmarioStateController;
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
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    // state
    [System.NonSerialized]
    public bool alive = true;
    private bool moving = false;
    private bool jumpedState = false;

    void Awake()
    {
        box = marioCollider as BoxCollider2D;
        smallSize = box.size;
        smallOffset = box.offset;
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

    // FixedUpdate is called 50 times a second
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Mario collision: " + col.gameObject.name + " tag=" + col.gameObject.tag, col.gameObject);

        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

        if (col.gameObject.CompareTag("Enemy"))
        {
            ContactPoint2D contact = col.GetContact(0);
            if (contact.normal.y > 0.5f)
            {
                col.gameObject.GetComponent<EnemyMovement>().Stomp();
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocityX, 0f);
                marioBody.AddForce(Vector2.up * upSpeed * 0.6f, ForceMode2D.Impulse);
            }
            else
            {
                // if (isSuper)
                //     UnmakeSuperMario();
                // else
                //     KillMario();
                DamageMario();
            }
        }
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
        // Vector2 movement = new Vector2(value, 0);
        // // check if it doesn't go beyond maxSpeed
        // if (marioBody.linearVelocity.magnitude < maxSpeed)
        //     marioBody.AddForce(movement * speed);

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
            if (!IsSuper) PlayJumpSound();
            else PlaySuperJumpSound();
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

    public void Stomp()
    {

    }

    // for audio
    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlaySuperJumpSound()
    {
        marioAudio.PlayOneShot(marioSuperJumpClip);
    }

    // void PlayGrowSound()
    // {
    //     marioAudio.PlayOneShot(marioGrowClip);
    // }

    // void PlayShrinkSound()
    // {
    //     marioAudio.PlayOneShot(marioShrinkClip);
    // }

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
        // GameOverAnimationStart(); // last time Mario dies right away

        // pass this to StateController to see if Mario should start game over
        // since both state StateController and MarioStateController are on the same gameobject, it's ok to cross-refer between scripts
        // GetComponent<MarioStateController>().SetPowerup(PowerupType.Damage);
        if (marioStateController.currentState.name == "InvincibleSmallMario") return; // ignore hits while invincible
        marioStateController.SetPowerup(PowerupType.Damage);
    }

    public void KillMario()
    {
        if (alive)
        {
            marioCollider.enabled = false;
            // play death animation
            // marioAnimator.Play("mario-die");
            marioDeathAudio.PlayOneShot(marioDeathAudio.clip);
            alive = false;
        }
    }

    private IEnumerator DelayedKillMario()
    {
        yield return new WaitForSeconds(2f);
        KillMario();
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

    // public void MakeSuperMario()
    // {
    //     if (isSuper || growing) return;
    //     PlayGrowSound();
    //     StartCoroutine(GrowRoutine());
    // }

    // private IEnumerator GrowRoutine()
    // {
    //     growing = true;
    //     Time.timeScale = 0f;

    //     marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    //     marioAnimator.Play("mario-grow");

    //     yield return new WaitForSecondsRealtime(1f);

    //     // swap to the super Mario setup
    //     isSuper = true;
    //     box.size = new Vector2(smallSize.x, smallSize.y * 2f);
    //     box.offset = new Vector2(smallOffset.x, smallOffset.y + smallSize.y / 2f);
    //     marioAnimator.runtimeAnimatorController = supermarioStateController;
    //     marioAnimator.updateMode = AnimatorUpdateMode.Normal;
    //     marioAnimator.Rebind();
    //     marioAnimator.Update(0f);
    //     marioAnimator.SetBool("onGround", onGroundState);

    //     Time.timeScale = 1f;
    //     growing = false;
    // }

    // public void UnmakeSuperMario()
    // {
    //     if (!isSuper || growing) return;
    //     PlayShrinkSound();
    //     StartCoroutine(ShrinkRoutine());
    // }

    // private IEnumerator ShrinkRoutine()
    // {
    //     growing = true;
    //     Time.timeScale = 0f;

    //     marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    //     marioAnimator.Play("mario-shrink");

    //     yield return new WaitForSecondsRealtime(1f);

    //     // swap to the small Mario setup
    //     isSuper = false;
    //     box.size = smallSize;
    //     box.offset = smallOffset;
    //     marioAnimator.runtimeAnimatorController = smallmarioStateController;
    //     marioAnimator.updateMode = AnimatorUpdateMode.Normal;
    //     marioAnimator.Rebind();
    //     marioAnimator.Update(0f);
    //     marioAnimator.SetBool("onGround", onGroundState);

    //     Time.timeScale = 1f;
    //     growing = false;
    // }

    // private IEnumerator InvincibilityRoutine()
    // {
    //     isInvincible = true;
    //     float elapsed = 0f;

    //     while (elapsed < invincibleDuration)
    //     {
    //         SetSpriteAlpha(marioSprite.color.a < 1f ? 1f : 0.3f);
    //         yield return new WaitForSeconds(blinkInterval);
    //         elapsed += blinkInterval;
    //     }

    //     SetSpriteAlpha(1f);
    //     isInvincible = false;
    //     StartCoroutine(InvincibilityRoutine());
    // }

    // private void SetSpriteAlpha(float a)
    // {
    //     Color c = marioSprite.color;
    //     c.a = a;
    //     marioSprite.color = c;
    // }

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
        marioAnimator.Update(0f);
        marioAnimator.SetBool("onGround", onGroundState);
    }

    public void GameOverScene()
    {
        onGameOver.Invoke();
    }
}