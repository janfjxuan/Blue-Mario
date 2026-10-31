using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseButtonController : MonoBehaviour, InteractiveButton
{
    public UnityEvent onGameOver;
    public SimpleGameEvent onGamePaused;
    public SimpleGameEvent onGameResumed;
    public Sprite pauseIcon;
    public Sprite playIcon;
    private Image image;
    private bool isPaused = false;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ButtonClick()
    {
        if (isPaused)
            onGameResumed.Raise();
        else
            onGamePaused.Raise();
    }

    public void OnPaused()
    {
        isPaused = true;
        image.sprite = playIcon;
    }

    public void OnResumed()
    {
        isPaused = false;
        image.sprite = pauseIcon;
    }
}
