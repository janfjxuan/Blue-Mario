using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour, IPowerupApplicable
{
    public GameConstants gameConstants;
    private bool growing = false;
    private bool isSuper = false;
    public bool IsSuper => isSuper;
    private bool isInvincible = false;
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
    public AudioClip marioGrowClip;
    public AudioClip marioShrinkClip;
    public Animator marioAnimator;
    public RuntimeAnimatorController superMarioController;
    private RuntimeAnimatorController smallMarioController;
    public Collider2D marioCollider;
    public Vector3 marioStartingPosition;
    public UnityEvent onGameOver;

    float deathImpulse;
    float upSpeed;
    float maxSpeed;
    float speed;
    float deceleration;
    public float invincibleDuration = 2f;   // or put it in GameConstants
    public float blinkInterval = 0.1f;

    public bool onGroundState = true;
    public bool faceRightState = true;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    // state
    [System.NonSerialized]
    public bool alive = true;
    private bool moving = false;
    private bool jumpedState = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set constants
        speed = gameConstants.speed;
        maxSpeed = gameConstants.maxSpeed;
        deathImpulse = gameConstants.deathImpulse;
        upSpeed = gameConstants.upSpeed;
        deceleration = gameConstants.deceleration;

        marioCollider.enabled = true;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        smallMarioController = marioAnimator.runtimeAnimatorController;
        marioStartingPosition = marioBody.transform.position;

        box = marioCollider as BoxCollider2D;
        smallSize = box.size;
        smallOffset = box.offset;

        // subscribe to scene manager scene change
        // SceneManager.activeSceneChanged += SetStartingPosition;
    }

    // public void SetStartingPosition(Scene current, Scene next)
    // {
    //     if (next.name == "World-1-2")
    //     {
    //         // change the position accordingly in your World-1-2 case
    //         this.transform.position = new Vector3(-10.2399998f, -4.3499999f, 0.0f);
    //     }
    // }

    void PlayDeathImpulse()
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
            else if (!isInvincible)
            {
                if (isSuper)
                    UnmakeSuperMario();
                else
                    KillMario();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Mario trigger: " + other.name + " tag=" + other.tag, other);
        if (other.gameObject.CompareTag("GapHole"))
        {
            KillMario(); // can change to different event 
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
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
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
        if (alive && onGroundState && !growing && !inputLocked)
        {
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0f);
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
            if (!isSuper) PlayJumpSound();
            else PlaySuperJumpSound();
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState && !growing && !inputLocked)
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

    void PlayGrowSound()
    {
        marioAudio.PlayOneShot(marioGrowClip);
    }

    void PlayShrinkSound()
    {
        marioAudio.PlayOneShot(marioShrinkClip);
    }

    public void KillMario()
    {
        if (alive)
        {
            marioCollider.enabled = false;
            // play death animation
            marioAnimator.Play("mario-die");
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

    public void MakeSuperMario()
    {
        if (isSuper || growing) return;
        PlayGrowSound();
        StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        growing = true;
        Time.timeScale = 0f;

        marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        marioAnimator.Play("mario-grow");

        yield return new WaitForSecondsRealtime(1f);

        // swap to the super Mario setup
        isSuper = true;
        box.size = new Vector2(smallSize.x, smallSize.y * 2f);
        box.offset = new Vector2(smallOffset.x, smallOffset.y + smallSize.y / 2f);
        marioAnimator.runtimeAnimatorController = superMarioController;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;
        marioAnimator.Rebind();
        marioAnimator.Update(0f);
        marioAnimator.SetBool("onGround", onGroundState);

        Time.timeScale = 1f;
        growing = false;
    }

    public void UnmakeSuperMario()
    {
        if (!isSuper || growing) return;
        PlayShrinkSound();
        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator ShrinkRoutine()
    {
        growing = true;
        Time.timeScale = 0f;

        marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        marioAnimator.Play("mario-shrink");

        yield return new WaitForSecondsRealtime(1f);

        // swap to the small Mario setup
        isSuper = false;
        box.size = smallSize;
        box.offset = smallOffset;
        marioAnimator.runtimeAnimatorController = smallMarioController;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;
        marioAnimator.Rebind();
        marioAnimator.Update(0f);
        marioAnimator.SetBool("onGround", onGroundState);

        Time.timeScale = 1f;
        growing = false;
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleDuration)
        {
            SetSpriteAlpha(marioSprite.color.a < 1f ? 1f : 0.3f);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetSpriteAlpha(1f);
        isInvincible = false;
        StartCoroutine(InvincibilityRoutine());
    }

    private void SetSpriteAlpha(float a)
    {
        Color c = marioSprite.color;
        c.a = a;
        marioSprite.color = c;
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
        growing = false;
        Time.timeScale = 1f;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;

        // reset position
        marioBody.transform.position = marioStartingPosition;
        marioBody.transform.rotation = Quaternion.identity;
        marioBody.linearVelocity = Vector2.zero;
        marioBody.angularVelocity = 0f;
        ResetMovementState();

        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset animation
        isSuper = false;
        box.size = smallSize;
        box.offset = smallOffset;
        marioAnimator.runtimeAnimatorController = smallMarioController;

        marioAnimator.SetBool("onGround", onGroundState);

        marioCollider.enabled = true;
        marioAnimator.SetTrigger("gameRestart");

        isInvincible = false;
        SetSpriteAlpha(1f);
        alive = true;

        if (startWithCastleWalk) StartCoroutine(WalkOutOfCastle());
    }

    public void GameOverScene()
    {
        onGameOver.Invoke();
    }
}