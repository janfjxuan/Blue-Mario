using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        GameManager.instance.GoToMainMenu();
    }
}