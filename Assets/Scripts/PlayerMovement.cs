using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public GameConstants gameConstants;
    public UnityEvent stomp;
    // public GameManager gameManager;
    public Rigidbody2D marioBody;
    public SpriteRenderer marioSprite;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;
    public Animator marioAnimator;
    public Collider2D marioCollider;
    public Vector3 marioStartingPosition;

    float deathImpulse;
    float upSpeed;
    float maxSpeed;
    float speed;

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

        marioCollider.enabled = true;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        marioStartingPosition = marioBody.transform.position;

        // subscribe to scene manager scene change
        // SceneManager.activeSceneChanged += SetStartingPosition;
    }

    void Awake()
    {
        // subscribe to Game Restart event
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
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
                KillMario();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.gameObject.CompareTag("Enemy"))
        // {
        //     KillMario();
        // }
        // else if (other.gameObject.CompareTag("Goal"))
        // {
        //     gameManager.LevelComplete();
        // }
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
            Debug.Log("FixedUpdate: moving is true, calling Move");

            Move(faceRightState == true ? 1 : -1);
        }
    }

    void Move(int value)
    {
        Debug.Log($"Move() called with value {value}, current velocity: {marioBody.linearVelocity}");

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        Debug.Log($"MoveCheck called with value {value}, alive={alive}");

        if (!alive) return;

        if (value == 0)
        {
            moving = false;
            marioBody.linearVelocityX = 0;
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
        if (alive && onGroundState)
        {
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0f);
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
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

    public void ResetMovementState()
    {
        moving = false;
        jumpedState = false;
        onGroundState = true;
    }

    public void GameRestart()
    {
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
        marioCollider.enabled = true;
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
    }

    public void GameOverScene()
    {
        // gameManager.GameOver();
        GameManager.instance.GameOver();
    }
}