using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    public int score = 0;

    public GameObject questionBoxes;
    public AudioSource musicSource;
    public HUDManager hudManager;


    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;

        gameStart.Invoke();
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void GameRestart()
    {
        // reset score
        score = 0;
        SetScore(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
        
        // restart mario music from the beginning
        musicSource.Stop();
        musicSource.time = 0f;
        musicSource.Play();

        // jumpOverGoomba.score = 0;
        // jumpOverGoomba.scoreText.text = "Score: 0";
        // jumpOverGoomba.timer = 10f;
        // jumpOverGoomba.timerText.text = "Timer: 10";
        // jumpOverGoomba.blueScreen.color = new Color(255, 255, 255, 0);

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
    }

    // public void RestartButtonCallback(int input)
    // {
    //     // reset everything
    //     GameRestart();
    //     // resume time
    //     Time.timeScale = 1.0f;
    //     hudManager.Hide();
    //     // restart mario music from the beginning
    //     musicSource.Stop();
    //     musicSource.time = 0f;
    //     musicSource.Play();
    // }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        // stop mario music
        musicSource.Pause();
        // set gameover scene
        hudManager.GameOver();
        gameOver.Invoke();
    }
}
