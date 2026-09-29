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
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteScoreText;

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
        timerText.GetComponent<TextMeshProUGUI>().text = "Timer: " + Mathf.Round(timer).ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = scoreText.GetComponent<TextMeshProUGUI>().text;
    }

    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteScoreText.text = scoreText.GetComponent<TextMeshProUGUI>().text;
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }
}