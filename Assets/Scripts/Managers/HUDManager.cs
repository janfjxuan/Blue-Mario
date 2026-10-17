using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject timerText;
    public Transform restartButton;
    public IntVariable gameScore;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverTimeText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteScoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteTimeText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteHighScoreText;

    void OnEnable()
    {
        SetScore();
    }
    void Start()
    {
        SetScore();
    }

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        SetScore();
    }

    public void SetScore()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + gameScore.Value.ToString();
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
        return "TOP- " + gameScore.previousHighestValue.ToString("D6");
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }
}