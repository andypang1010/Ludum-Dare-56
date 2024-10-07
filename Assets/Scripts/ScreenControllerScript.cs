using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScreenController : MonoBehaviour
{
    public enum ScreenState
    {
        NORMAL,
        PAUSE,
        HELP,
        WIN,
        LOSE
    }
    public ScreenState currentState;
    // Start is called before the first frame update
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject helpMenuUI;
    public GameObject winMenuUI;
    public GameObject loseMenuUI;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == ScreenState.WIN || currentState == ScreenState.LOSE)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == ScreenState.NORMAL)
            {
                SetState(ScreenState.PAUSE);
            }
            else
            {
                SetState(ScreenState.NORMAL);
            }
        }

        updateMenu();
    }
    public ScreenState getState()
    {
        return currentState;
    }
    public bool IsNormal()
    {
        return currentState == ScreenState.NORMAL;
    }

    public void Resume()
    {
        SetState(ScreenState.NORMAL);
    }

    public void Pause()
    {
        SetState(ScreenState.PAUSE);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void getHelp()
    {
        SetState(ScreenState.HELP);
    }

    public void Win()
    {
        SetState(ScreenState.WIN);
    }
    public void Lose()
    {
        SetState(ScreenState.LOSE);
    }

    public void SetState(ScreenState state)
    {
        currentState = state;
        updateMenu();
    }
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void updateMenu()
    {
        if (currentState == ScreenState.NORMAL)
        {
            helpMenuUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
        else if (currentState == ScreenState.PAUSE)
        {
            helpMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        else if (currentState == ScreenState.HELP)
        {
            helpMenuUI.SetActive(true);
            pauseMenuUI.SetActive(false);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        else if (currentState == ScreenState.WIN)
        {
            winMenuUI.SetActive(true);
            helpMenuUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        else if (currentState == ScreenState.LOSE)
        {
            loseMenuUI.SetActive(true);
            helpMenuUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }
}

