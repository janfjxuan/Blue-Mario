using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject timerText;
    public Transform restartButton;
    public IntVariable gameScore;
    public IntVariable gameCoins;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI worldText;
    private float lastTime = 0f;
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
        gameCoins.SetValue(0);
        SetScore();
        SetCoins();
        if (worldText != null)
            worldText.text = "WORLD\n" + SceneManager.GetActiveScene().name.Replace("World-", "");
    }

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        gameCoins.SetValue(0);
        SetScore();
        SetCoins();
    }

    public void SetScore()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "MARIO\n" + gameScore.Value.ToString("D7");
    }

    public void SetCoins()
    {
        coinText.text = "\nx" + gameCoins.Value.ToString("D2");
    }

    public void SetTimer(float timer)
    {
        lastTime = timer;
        timerText.GetComponent<TextMeshProUGUI>().text = "TIME\n" + Mathf.FloorToInt(timer).ToString("D3");
    }

    private string PanelScore() { return "Score: " + gameScore.Value; }

    private string PanelTime()
    {
        int minutes = Mathf.FloorToInt(lastTime / 60f);
        int seconds = Mathf.FloorToInt(lastTime % 60f);
        return "Time : " + minutes + ":" + seconds.ToString("00");
    }


    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = PanelScore();
        gameOverTimeText.text = PanelTime();
        if (gameOverHighScoreText != null)
            gameOverHighScoreText.text = TopScoreString();

    }

    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteScoreText.text = PanelScore();
        levelCompleteTimeText.text = PanelTime();
        if (levelCompleteHighScoreText != null)
            levelCompleteHighScoreText.text = TopScoreString();
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