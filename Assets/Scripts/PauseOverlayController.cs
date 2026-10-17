using UnityEngine;

public class PauseOverlayController : MonoBehaviour
{
    public GameObject overlay;

    void Start()
    {
        overlay.SetActive(false);
        GameManager.instance.pauseChange.AddListener(SetOverlay);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.pauseChange.RemoveListener(SetOverlay);
    }

    void SetOverlay(bool paused)
    {
        overlay.SetActive(paused);
    }
}