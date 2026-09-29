using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JumpOverGoomba : MonoBehaviour
{
    // public Transform enemyLocation;
    // GameManager gameManager;
    // private bool onGroundState;

    // [System.NonSerialized]
    // public int score = 0; // we don't want this to show up in the inspector

    // [System.NonSerialized]
    // public float timer = 10f;

    // private bool countScoreState = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // if (timer > 0)
        // {
        //     timer -= Time.deltaTime;
        //     timerText.text = "Timer: " + Mathf.Round(timer).ToString();
        // } 
        // else
        // {
        //     timer = 0;
        //     timerText.text = "Timer: " + timer.ToString();
        //     gameManager.KillMario();
        // }
    }

    void FixedUpdate()
    {
        // // mario jumps
        // if (Input.GetKeyDown("space") && OnGroundCheck())
        // {
        //     onGroundState = false;
        //     countScoreState = true;
        // }

        // // when jumping, and Goomba is near Mario and we haven't registered our score
        // if (!onGroundState && countScoreState)
        // {
        //     foreach (Transform child in gameManager.enemies.transform)
        //     {
        //         if (Mathf.Abs(transform.position.x - child.position.x) < 0.5f && child.gameObject.GetComponent<SpriteRenderer>().enabled)
        //         {
        //             countScoreState = false;
        //             score++;
        //             timer = 10f;
        //             scoreText.text = "Score: " + score.ToString();
        //             timerText.text = "Timer: " + timer.ToString();
        //         }
        //     }
        // }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // if (col.gameObject.CompareTag("Ground")) onGroundState = true;

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

    // void GameOverScene()
    // {
    //     // stop time
    //     Time.timeScale = 0.0f;
    //     // stop mario music
    //     gameManager.musicSource.Pause();
    //     // set gameover scene
    //     gameManager.hudManager.GameOver();
    // }
}
