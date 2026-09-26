using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject mario;
    // state
    [System.NonSerialized]
    public bool alive = true;
    public GameObject enemies;
    public Transform gameCamera;
    public PlayerMovement playerMovement;
    public JumpOverGoomba jumpOverGoomba;
    public GameObject questionBoxes;
    public AudioSource musicSource;
    public GameOverUI gameOverUI;
    public AudioClip marioDeath;


    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        for (int i = 1; i < enemies.transform.childCount; i++)
        {
            enemies.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            enemies.transform.GetChild(i).gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
    public void ResetGame()
    {
        // reset position
        playerMovement.marioBody.transform.position = new Vector3(5.0f, 2.5f, 0.0f);
        playerMovement.marioBody.transform.rotation = Quaternion.identity;
        playerMovement.marioBody.linearVelocity = Vector2.zero;
        playerMovement.marioBody.angularVelocity = 0f;
        // reset sprite direction
        playerMovement.faceRightState = true;
        playerMovement.marioSprite.flipX = false;
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
        jumpOverGoomba.scoreText.text = "Score: 0";
        jumpOverGoomba.timer = 10f;
        jumpOverGoomba.timerText.text = "Timer: 10";
        jumpOverGoomba.blueScreen.color = new Color(255, 255, 255, 0);
        // reset question box
        foreach (Transform transform in questionBoxes.transform)
        {
            Transform questionBoxTransform = transform.Find("Question-Box");
            if (questionBoxTransform != null)
            {
                QuestionBox questionBox = questionBoxTransform.GetComponent<QuestionBox>();
                questionBox.currentHitCount = questionBox.hitCount;
                questionBox.questionBoxAnimator.SetBool("isEmpty", false);
                questionBox.questionBoxAnimator.Rebind();
                questionBox.questionBoxAnimator.Update(0f);
                questionBox.ceiling.SetActive(false);
            }
            Transform brickTransform = transform.Find("Brick-Coin");
            if (brickTransform != null)
            {
                BrickCoin brickCoin = brickTransform.GetComponent<BrickCoin>();
                brickCoin.currentHitCount = brickCoin.hitCount;
                // brickCoin.brickAnimator.Rebind();
                // brickCoin.brickAnimator.Update(0f);
            }
        }

        // reset animation
        playerMovement.marioCollider.enabled = true;
        playerMovement.marioAnimator.SetTrigger("gameRestart");
        alive = true;
        // reset music
        musicSource.clip = jumpOverGoomba.bgMusic;
        musicSource.Play();
        // reset camera position
        gameCamera.position = new Vector3(8.89f, 5, -10);
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

    public void KillMario()
    {
        if(alive)
        {
            playerMovement.marioCollider.enabled = false;
            // play death animation
            playerMovement.marioAnimator.Play("mario-die");
            playerMovement.marioAudio.PlayOneShot(marioDeath);
            alive = false; 
        }
    }
}
