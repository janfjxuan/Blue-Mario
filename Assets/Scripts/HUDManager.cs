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
    public Transform restartButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    // Start is called before the first frame update
    // void Start()
    // {
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    // public void GameStart()
    // {
    //     // hide gameover panel
    //     gameOverPanel.SetActive(false);
    //     scoreText.transform.localPosition = scoreTextPosition[0];
    //     restartButton.localPosition = restartButtonPosition[0];
    // }

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    // public void Show(int score)
    // {
    //     finalScoreText.text = "Score: " + score;
    //     gameOverPanel.SetActive(true);
    // }
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = scoreText.GetComponent<TextMeshProUGUI>().text;
    }

    // public void GameOver()
    // {
    //     gameOverPanel.SetActive(true);
    //     scoreText.transform.localPosition = scoreTextPosition[1];
    //     restartButton.localPosition = restartButtonPosition[1];
    // }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
    }
}