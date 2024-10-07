using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject helpScene;
    public GameObject mainScene;
    public void PlayGame()
    {
        SceneManager.LoadScene("CG");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings menu opened!");
    }

    // Function to quit the game
    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
    public void GetHelp()
    {
        helpScene.SetActive(true);
        mainScene.SetActive(false);
    }
    public void GetMain()
    {
        helpScene.SetActive(false);
        mainScene.SetActive(true);
    }
}