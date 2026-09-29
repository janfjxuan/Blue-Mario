using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using TMPro;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent<float> timerChange;
    public UnityEvent gameOver;
    public UnityEvent levelComplete;

    private bool levelCompleted = false;
    public int score = 0;
    public float timer = 60f;

    public GameObject questionBoxes;
    public AudioSource musicSource;
    public HUDManager hudManager;
    public CameraController gameCameraController;

    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot gameOverSnapshot;

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
        if (Time.timeScale > 0 && timer > 0)
        {
            timer -= Time.deltaTime;
            timerChange.Invoke(Mathf.Max(timer, 0));
            if (timer <= 0)
            {
                timer = 0;
                timerChange.Invoke(0);
                GameOver();
            }
        }
    }

    public void GameRestart()
    {
        // reset score
        score = 0;
        SetScore(score);

        timer = 60f;
        timerChange.Invoke(timer);

        gameRestart.Invoke();
        Time.timeScale = 1.0f;

        levelCompleted = false;

        defaultSnapshot.TransitionTo(0.1f);

        // restart mario music from the beginning
        musicSource.Stop();
        musicSource.time = 0f;
        musicSource.Play();

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

        // reset camera 
        gameCameraController.ResetCamera();
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
        timer = 60f;
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }

    public void GameOver()
    {
        gameOverSnapshot.TransitionTo(0f);
        gameOver.Invoke();
        Time.timeScale = 0.0f;
    }

    public void LevelComplete()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        levelComplete.Invoke();
        Time.timeScale = 0.0f;
    }
}
