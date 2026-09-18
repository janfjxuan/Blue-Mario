using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void Show(int score)
    {
        finalScoreText.text = "Score: " + score;
        gameOverPanel.SetActive(true);
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
    }
}