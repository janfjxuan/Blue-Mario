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
    private bool onGroundState;

    [System.NonSerialized]
    public int score = 0; // we don't want this to show up in the inspector

    private bool countScoreState = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public Image blueScreen;
    // public GameOverUI gameOverUI;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


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
                    scoreText.text = "Score: " + score.ToString();
                    Debug.Log(score);
                    if(score < enemies.transform.childCount)
                    {
                        enemies.transform.GetChild(score).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        enemies.transform.GetChild(score).gameObject.GetComponent<Collider2D>().enabled = true;
                    }

                    blueScreen.color = new Color(255, 255, 255, blueScreen.color.a + score * 0.005f);
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
            Debug.Log("on ground");
            return true;
        }
        else
        {
            Debug.Log("not on ground");
            return false;
        }
    }
}
