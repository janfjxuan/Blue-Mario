using UnityEngine;

public class PauseOverlayController : MonoBehaviour
{
    public GameObject overlay;

    void Start()
    {
        overlay.SetActive(false);
    }

    public void Show() { overlay.SetActive(true); }
    public void Hide() { overlay.SetActive(false); }
}