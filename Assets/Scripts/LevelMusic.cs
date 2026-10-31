using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource starTheme;
    public AudioSource undergroundTheme;

    private bool starActive = false;
    private bool underground = false;

    public void StarStarted()
    {
        starActive = true;
        bgm.Pause();
        undergroundTheme.Pause();
        starTheme.Play();
    }

    public void StarEnded()
    {
        if (!starActive) return;
        starActive = false;
        starTheme.Stop();
        if (underground) undergroundTheme.UnPause();
        else bgm.UnPause();
    }

    public void UndergroundEntered()
    {
        underground = true;
        bgm.Pause();
        undergroundTheme.Play();
        if (starActive) undergroundTheme.Pause();
    }

    public void UndergroundExited()
    {
        underground = false;
        undergroundTheme.Stop();
        if (!starActive) bgm.UnPause();
    }

    public void GameRestart()
    {
        starActive = false;
        underground = false;
        starTheme.Stop();
        undergroundTheme.Stop();
    }
}