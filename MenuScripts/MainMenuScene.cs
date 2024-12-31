using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScene : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
