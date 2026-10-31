using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButtonController : MonoBehaviour, InteractiveButton
{
    public SimpleGameEvent onAfterLevelComplete;
    public void ButtonClick()
    {
        Time.timeScale = 1f;
        onAfterLevelComplete.Raise();
    }
}