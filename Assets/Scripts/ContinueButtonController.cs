using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButtonController : MonoBehaviour, InteractiveButton
{
    public SimpleGameEvent onAfterLevelComplete;
    public void ButtonClick()
    {
        Time.timeScale = 1f;
        onAfterLevelComplete.Raise();

        // int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // if (nextIndex < SceneManager.sceneCountInBuildSettings)
        //     SceneManager.LoadScene(nextIndex);
        // else
        //     SceneManager.LoadScene(0);
    }
}