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

    private bool levelCompleted = false;
    private bool timerRunning = true;
    public int score = 0;
    public float timer = 60f;

    public GameObject obstacles;
    public AudioSource musicSource;
    public HUDManager hudManager;
    public CameraController gameCameraController;

    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot gameOverSnapshot;

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;
        // subscribe to scene manager scene change
        SceneManager.activeSceneChanged += SceneSetup;
    }

    public void SceneSetup(Scene current, Scene next)
    {
        // Setup code for the new scene
        gameStart.Invoke();
        SetScore(score);
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
        score = 0;
        SetScore(score);

        timerRunning = true;
        timer = 0f;
        timerChange.Invoke(timer);

        Time.timeScale = 1.0f;

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
                CoinBox coinBox = questionBoxTransform.GetComponent<CoinBox>();
                if (coinBox != null)
                    coinBox.ResetQuestionBox();
                ShoeBox shoeBox = questionBoxTransform.GetComponent<ShoeBox>();
                if (shoeBox != null)
                    shoeBox.ResetQuestionBox();
                MagicMushroomBox magicMushroomBox = questionBoxTransform.GetComponent<MagicMushroomBox>();
                if (magicMushroomBox != null)
                    magicMushroomBox.ResetQuestionBox();
            }
            Transform brickTransform = transform.Find("Brick-Coin");
            if (brickTransform != null)
            {
                BrickCoin brickCoin = brickTransform.GetComponent<BrickCoin>();
                if (brickCoin != null)
                    brickCoin.ResetBrick();
            }
        }

        // reset camera 
        gameCameraController.ResetCamera();
        gameRestart.Invoke();
    }

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
}
