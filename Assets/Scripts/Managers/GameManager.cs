using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent<float> timerChange;
    public UnityEvent gameOver;
    public UnityEvent levelComplete;
    public UnityEvent<bool> pauseChange;

    private bool levelCompleted = false;
    private bool timerRunning = true;
    private bool isPaused = false;
    public IntVariable gameScore;
    public float timer = 0f;

    public GameObject obstacles;
    public AudioSource musicSource;
    public CameraController gameCameraController;
    
    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot gameOverSnapshot;

    void Start()
    {
        gameScore.Value = 0;
        gameStart.Invoke();
        Time.timeScale = 1.0f;
        // subscribe to scene manager scene change
        SceneManager.activeSceneChanged += SceneSetup;
    }

    public void SceneSetup(Scene current, Scene next)
    {
        // Setup code for the new scene
        gameStart.Invoke();
        SetScore(gameScore.Value);
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
            timerChange.Invoke(timer);
        }
    }

    public void GameRestart()
    {
        // reset score
        gameScore.Value = 0;
        SetScore(gameScore.Value);

        timerRunning = true;
        timer = 0f;
        timerChange.Invoke(timer);

        Time.timeScale = 1.0f;

        isPaused = false;
        AudioListener.pause = false;
        pauseChange.Invoke(false);

        levelCompleted = false;

        defaultSnapshot.TransitionTo(0.1f);

        // restart mario music from the beginning
        musicSource.Stop();
        musicSource.time = 0f;
        musicSource.Play();

        // reset question box
        foreach (Transform transform in obstacles.transform)
        {
            Transform questionBoxTransform = transform.Find("Question-Box");
            if (questionBoxTransform != null)
            {
                QuestionBoxPowerupController questionBox = questionBoxTransform.GetComponent<QuestionBoxPowerupController>();
                if (questionBox != null)
                    questionBox.ResetQuestionBox();
            }
            Transform brickTransform = transform.Find("Brick");
            if (brickTransform != null)
            {
                BrickPowerupController brick = brickTransform.GetComponent<BrickPowerupController>();
                if (brick != null)
                    brick.ResetBrick();
            }
        }

        // reset camera 
        gameCameraController.ResetCamera();
        gameRestart.Invoke();
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }

    public void GameOver()
    {
        timerRunning = false;
        gameOverSnapshot.TransitionTo(0f);
        gameOver.Invoke();
        Time.timeScale = 0.0f;
    }

    public void LevelComplete()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        timerRunning = false;
        levelComplete.Invoke();
        Time.timeScale = 0.0f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        AudioListener.pause = isPaused;
        pauseChange.Invoke(isPaused);
    }
}
