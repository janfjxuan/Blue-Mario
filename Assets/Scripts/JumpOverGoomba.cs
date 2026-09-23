using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JumpOverGoomba : MonoBehaviour
{
    // public Transform enemyLocation;
    public GameObject enemies;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    private bool onGroundState;

    [System.NonSerialized]
    public int score = 0; // we don't want this to show up in the inspector

    [System.NonSerialized]
    public float timer = 10f;

    private bool countScoreState = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public AudioSource audioSource;
    public AudioClip bgMusic;
    public AudioClip castleMusic;
    public AudioClip winError;
    public Image blueScreen;
    public PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = "Timer: " + Mathf.Round(timer).ToString();
        } 
        else
        {
            timer = 0;
            timerText.text = "Timer: " + timer.ToString();
            playerMovement.KillMario();
        }

    }

    void FixedUpdate()
    {
        // mario jumps
        if (Input.GetKeyDown("space") && OnGroundCheck())
        {
            onGroundState = false;
            countScoreState = true;
        }

        // when jumping, and Goomba is near Mario and we haven't registered our score
        if (!onGroundState && countScoreState)
        {
            foreach (Transform child in enemies.transform)
            {
                if (Mathf.Abs(transform.position.x - child.position.x) < 0.5f && child.gameObject.GetComponent<SpriteRenderer>().enabled)
                {
                    countScoreState = false;
                    score++;
                    timer = 10f;
                    scoreText.text = "Score: " + score.ToString();
                    timerText.text = "Timer: " + timer.ToString();
                    if (score < enemies.transform.childCount)
                    {
                        enemies.transform.GetChild(score).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        enemies.transform.GetChild(score).gameObject.GetComponent<Collider2D>().enabled = true;
                    }
                    if (score == 2)
                    {
                        StartCoroutine(DamageEffect());
                    }
                    if (score == 5)
                    {
                        StartCoroutine(DamageEffect());
                        audioSource.clip = castleMusic;
                        audioSource.Play();
                    }
                    if (score > 5)
                    {
                        blueScreen.color = new Color(255, 255, 255, blueScreen.color.a + score * 0.02f);
                    }
                    if (score > 10)
                    {
                        audioSource.clip = winError;
                        audioSource.Play();
                        StartCoroutine(DelayedKillMario());
                    }
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;

    }

    private bool OnGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator DamageEffect()
    {
        blueScreen.color = new Color(255, 255, 255, 1f);
        yield return new WaitForSeconds(0.02f);
        blueScreen.color = new Color(255, 255, 255, 0f);
    }

    private IEnumerator DelayedKillMario()
    {
        yield return new WaitForSeconds(2f);
        playerMovement.KillMario();
    }
}
