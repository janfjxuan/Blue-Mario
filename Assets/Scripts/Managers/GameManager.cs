using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // events
    public SimpleGameEvent gameStart;
    public SimpleGameEvent onUpdateScore;
    public FloatGameEvent timerChange;
    public SimpleGameEvent levelComplete;

    private bool levelCompleted = false;
    private bool timerRunning = true;
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
        gameStart.Raise();
        Time.timeScale = 1.0f;
        // subscribe to scene manager scene change
        SceneManager.activeSceneChanged += SceneSetup;
    }

    public void SceneSetup(Scene current, Scene next)
    {
        // Setup code for the new scene
        gameStart.Raise();
        SetScore(gameScore.Value);
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
            timerChange.Raise(timer);
        }
    }

    public void GameRestart()
    {
        // reset score
        gameScore.Value = 0;
        SetScore(gameScore.Value);

        timerRunning = true;
        timer = 0f;
        timerChange.Raise(timer);

        Time.timeScale = 1.0f;

        AudioListener.pause = false;

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
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        onUpdateScore.Raise();
    }

    public void GameOver()
    {
        timerRunning = false;
        gameOverSnapshot.TransitionTo(0f);
        Time.timeScale = 0.0f;
    }

    public void LevelComplete()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        timerRunning = false;
        levelComplete.Raise();
        Time.timeScale = 0.0f;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
    }
}
