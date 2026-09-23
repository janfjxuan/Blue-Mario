using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse = 5;
    public Transform gameCamera;
    public GameOverUI gameOverUI;
    public AudioSource musicSource;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    // state
    [System.NonSerialized]
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        this.GetComponent<Collider2D>().enabled = true;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        for (int i = 1; i < enemies.transform.childCount; i++)
        {
            enemies.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            enemies.transform.GetChild(i).gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
    void PlayDeathImpulse()
    {
        marioBody.linearVelocity = new Vector2(0f, 0f);
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
    void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;
        // stop mario music
        musicSource.Pause();
        // set gameover scene
        gameOverUI.Show(jumpOverGoomba.score);
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
        if (col.gameObject.CompareTag("QuestionBox"))
        {
            ContactPoint2D contact = col.GetContact(0);
            if (contact.normal.y < -0.5f)
            {
                Transform coinTransform = col.transform.parent.Find("Coin");
                if (coinTransform != null)
                {
                    coinTransform.gameObject.SetActive(true);
                    Animator coinAnimator = coinTransform.GetComponent<Animator>();
                    coinAnimator.SetTrigger("popUp");
                    AudioSource coinAudio = coinTransform.GetComponent<AudioSource>();
                    if (coinAudio != null)
                    {
                        coinAudio.Play();
                    }
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            this.GetComponent<Collider2D>().enabled = false;
            // play death animation
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }
    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement = new Vector2(moveHorizontal, 0);
                // check if it doesn't go beyond maxSpeed
                if (marioBody.linearVelocity.magnitude < maxSpeed)
                    marioBody.AddForce(movement * speed);
            }

            // stop
            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                // stop
                marioBody.linearVelocityX = 0;
            }

            if (Input.GetKeyDown("space") && onGroundState)
            {
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0f);
                marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
                onGroundState = false;
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
            }
        }
    }

    public void RestartButtonCallback(int input)
    {
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
        gameOverUI.Hide();
        // restart mario music from the beginning
        musicSource.Stop();
        musicSource.time = 0f;
        musicSource.Play();
    }

    public void ResetGame()
    {
        // reset position
        marioBody.transform.position = new Vector3(5.0f, 2.5f, 0.0f);
        marioBody.transform.rotation = Quaternion.identity;
        marioBody.linearVelocity = Vector2.zero;
        marioBody.angularVelocity = 0f;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        for (int i = 0; i < enemies.transform.childCount; i++)
        {
            // enemies.transform.GetChild(i).gameObject.GetComponent<EnemyMovement>().moveRight = -1;
            enemies.transform.GetChild(i).localPosition = enemies.transform.GetChild(i).GetComponent<EnemyMovement>().startPosition;
            if (i > 0)
            {
                enemies.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                enemies.transform.GetChild(i).gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
        // reset score
        jumpOverGoomba.score = 0;
        jumpOverGoomba.blueScreen.color = new Color(255, 255, 255, 0);
        // reset animation
        this.GetComponent<Collider2D>().enabled = true;
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
        // reset camera position
        gameCamera.position = new Vector3(8.89f, 5, -10);
    }

    // for audio
    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }
}