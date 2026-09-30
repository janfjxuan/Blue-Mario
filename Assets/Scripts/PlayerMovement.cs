using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D marioBody;
    public SpriteRenderer marioSprite;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;
    public Animator marioAnimator;
    public Collider2D marioCollider;
    public Vector3 initialPosition;

    private float moveHorizontal;
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    public bool onGroundState = true;
    public bool faceRightState = true;
    public float deathImpulse = 5;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    // state
    [System.NonSerialized]
    public bool alive = true;
    private bool moving = false;
    private bool jumpedState = false;

    // Start is called before the first frame update
    void Start()
    {
        marioCollider.enabled = true;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        initialPosition = marioBody.transform.position;
    }
    void PlayDeathImpulse()
    {
        marioBody.linearVelocity = new Vector2(0f, 0f);
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    // FixedUpdate is called 50 times a second
    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) & !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            KillMario();
        }
        else if (other.gameObject.CompareTag("EndLimit"))
        {
            gameManager.LevelComplete();
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
        marioBody.transform.position = initialPosition;
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
        gameManager.GameOver();
    }
}