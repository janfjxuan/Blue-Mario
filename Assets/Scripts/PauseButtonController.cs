using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonController : MonoBehaviour, InteractiveButton
{
    private bool isPaused = false;
    public Sprite pauseIcon;
    public Sprite playIcon;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        GameManager.instance.pauseChange.AddListener(SetIcon);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.pauseChange.RemoveListener(SetIcon);
        }
    }

    public void ButtonClick()
    {
        GameManager.instance.TogglePause();
    }

    void SetIcon(bool paused)
    {
        image.sprite = paused ? playIcon : pauseIcon;
    }
}
