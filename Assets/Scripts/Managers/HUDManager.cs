using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject timerText;
    public Transform restartButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverTimeText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteScoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteTimeText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteHighScoreText;


    void Start()
    {
        SetScore(GameManager.instance.gameScore.Value);
    }
    void Awake()
    {
        // subscribe to events
        GameManager.instance.gameStart.AddListener(GameStart);
        GameManager.instance.gameOver.AddListener(GameOver);
        GameManager.instance.gameRestart.AddListener(GameStart);
        GameManager.instance.scoreChange.AddListener(SetScore);
        GameManager.instance.timerChange.AddListener(SetTimer);
        GameManager.instance.levelComplete.AddListener(LevelComplete);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.RemoveListener(GameStart);
            GameManager.instance.gameOver.RemoveListener(GameOver);
            GameManager.instance.gameRestart.RemoveListener(GameStart);
            GameManager.instance.scoreChange.RemoveListener(SetScore);
            GameManager.instance.timerChange.RemoveListener(SetTimer);
            GameManager.instance.levelComplete.RemoveListener(LevelComplete);
        }
    }

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }
    public void SetTimer(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.GetComponent<TextMeshProUGUI>().text = "Time : " + minutes + ":" + seconds.ToString("00");
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = scoreText.GetComponent<TextMeshProUGUI>().text;
        gameOverTimeText.text = timerText.GetComponent<TextMeshProUGUI>().text;
        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = TopScoreString();
        }
    }

    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteScoreText.text = scoreText.GetComponent<TextMeshProUGUI>().text;
        levelCompleteTimeText.text = timerText.GetComponent<TextMeshProUGUI>().text;
        if (levelCompleteHighScoreText != null)
        {
            levelCompleteHighScoreText.text = TopScoreString();
        }
    }

    public string TopScoreString()
    {
        return "TOP- " + GameManager.instance.gameScore.previousHighestValue.ToString("D6");
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }
}