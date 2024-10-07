using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelScript : MonoBehaviour
{

    [Header("Settings")]
    public int currentLevel = 1;
    public int maxLevel = 5;
    public float currentPoly = 0f;
    public float polyRequiredToNextLevel = 100f;


    [Header("Meshes")]
    public GameObject[] bodies;
    public RuntimeAnimatorController[] animatorControllers;

    [Header("UI")]
    public Slider LevelSlider;
    public RectTransform progressBarFillRect;
    public RectTransform progressBarSliderRect;
    public ScreenController sc;

    void Start()
    {
        UpdateProgressBar();
        LevelSlider.maxValue = polyRequiredToNextLevel;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GainPoly(20);
        }

        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     LosePoly(20);
        // }

        // UpdateProgressBar();
    }

    public void GainPoly(int polyNumber)
    {
        currentPoly += polyNumber;
        if (currentPoly >= polyRequiredToNextLevel)
        {
            LevelUp();
        }
        UpdateProgressBar();
    }

    public void LosePoly(int polyNumber)
    {
        currentPoly -= polyNumber;
        if (currentPoly < 0)
        {
            if (currentLevel == 1)
            {
                currentPoly = 0;
                return;
            }
            LevelDown();
        }
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        LevelSlider.value = currentPoly;
    }

    private void LevelUp()
    {
        if (currentLevel + 1 > maxLevel)
        {
            ShowWin();
            return;
        }

        currentLevel++;
        UpdateMeshAndRig();

        currentPoly -= polyRequiredToNextLevel;
        polyRequiredToNextLevel *= 1.5f;
        Debug.Log("Level Up! Now at level " + currentLevel);
        LevelSlider.maxValue = polyRequiredToNextLevel;
    }

    private void LevelDown()
    {
        if (currentLevel - 1 < 1)
        {
            ShowLose();
            return;
        }
        currentLevel--;

        UpdateMeshAndRig();

        Instantiate(bodies[currentLevel - 1].transform.GetChild(0), transform);
        Instantiate(bodies[currentLevel - 1].transform.GetChild(1), transform);

        polyRequiredToNextLevel /= 1.5f;
        currentPoly = polyRequiredToNextLevel;
        Debug.Log("Level Down! Now at level " + currentLevel);
        LevelSlider.maxValue = polyRequiredToNextLevel;
    }
    public void ShowWin()
    {
        sc.Win();
    }
    public void ShowLose()
    {
        sc.Lose();
    }

    private void UpdateMeshAndRig() {
        transform.Find("Model").SetParent(transform.Find("Level " + (currentLevel - 1) + " Body"));
        transform.Find("mixamorig:Hips").SetParent(transform.Find("Level " + (currentLevel - 1) + " Body"));

        bodies[currentLevel - 1].transform.Find("Model").SetParent(transform);
        bodies[currentLevel - 1].transform.Find("mixamorig:Hips").SetParent(transform);

        GetComponent<Animator>().runtimeAnimatorController = animatorControllers[currentLevel - 1];
    }
}
