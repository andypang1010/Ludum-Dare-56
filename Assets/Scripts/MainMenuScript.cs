using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}