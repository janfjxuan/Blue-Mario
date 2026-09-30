using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    // private Vector3[] scoreTextPosition = {
    //     new Vector3(-820, 480, 0),
    //     new Vector3(0, 0, 0)
    //     };
    // private Vector3[] restartButtonPosition = {
    //     new Vector3(800, 480, 0),
    //     new Vector3(0, -150, 0)
    // };
    public GameObject scoreText;
    public GameObject timerText;
    public Transform restartButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverTimeText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteScoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteTimeText;

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
    }

    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteScoreText.text = scoreText.GetComponent<TextMeshProUGUI>().text;
        levelCompleteTimeText.text = timerText.GetComponent<TextMeshProUGUI>().text;
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }
}