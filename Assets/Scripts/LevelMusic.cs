using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource starTheme;
    private bool starActive = false;

    public void StarStarted()
    {
        starActive = true;
        bgm.Pause();
        starTheme.Play();
    }

    public void StarEnded()
    {
        if (!starActive) return;
        starActive = false;
        starTheme.Stop();
        bgm.UnPause();
    }

}
