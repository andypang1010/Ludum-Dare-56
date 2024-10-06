using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject helpMenuUI;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
                Debug.Log("Game Resume");
            }

            else
            {
                Pause();
                Debug.Log("Game Paused");
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void ShowHelp()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        helpMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }
    public void HideHelp()
    {
        Time.timeScale = 0f;
        helpMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}

