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
    public int polyRequiredToNextLevel = 100;


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
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         GainPoly(20);
    //     }

    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         LosePoly(20);
    //     }
    // }

    public void GainPoly(int polyNumber)
    {
        if (currentPoly + polyNumber > polyRequiredToNextLevel)
        {
            LevelUp(polyNumber);
        }

        else {
            currentPoly += polyNumber;
        }

        UpdateProgressBar();
    }

    public void LosePoly(int polyNumber)
    {
        if (currentPoly - polyNumber < 0)
        {
            LevelDown(polyNumber);
        }

        else {
            currentPoly -= polyNumber;
        }

        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        // print("Updated progress bar");
        LevelSlider.value = currentPoly;
        LevelSlider.maxValue = polyRequiredToNextLevel;
    }

    private void LevelUp(int polyNumber)
    {
        if (currentLevel + 1 > maxLevel)
        {
            ShowWin();
            return;
        }

        currentLevel++;
        UpgradeModelAndRig();


        currentPoly += polyNumber - polyRequiredToNextLevel;
        polyRequiredToNextLevel = (int) (polyRequiredToNextLevel * 1.5f);
    }

    private void LevelDown(int polyNumber)
    {
        if (currentLevel - 1 < 1) return;

        currentLevel--;
        DowngradeModelAndRig();


        polyRequiredToNextLevel = (int) (polyRequiredToNextLevel / 1.5f);
        currentPoly += polyRequiredToNextLevel - polyNumber;
    }

    public void ShowWin()
    {
        sc.Win();
    }

    private void UpgradeModelAndRig()
    {
        // print("Storing old model to -> LEVEL " + (currentLevel - 1) + " BODY");
        transform.Find("Model").SetParent(transform.Find("Level " + (currentLevel - 1) + " Body"));
        transform.Find("mixamorig:Hips").SetParent(transform.Find("Level " + (currentLevel - 1) + " Body"));

        int currentIndex = currentLevel - 1;

        bodies[currentIndex].transform.Find("Model").SetParent(transform);
        bodies[currentIndex].transform.Find("mixamorig:Hips").SetParent(transform);

        GetComponent<Animator>().runtimeAnimatorController = animatorControllers[currentIndex];
    }

    private void DowngradeModelAndRig()
    {
        // print("Storing old model to -> LEVEL " + (currentLevel + 1) + " BODY");
        transform.Find("Model").SetParent(transform.Find("Level " + (currentLevel + 1) + " Body"));
        transform.Find("mixamorig:Hips").SetParent(transform.Find("Level " + (currentLevel + 1) + " Body"));

        int currentIndex = currentLevel - 1;

        bodies[currentIndex].transform.Find("Model").SetParent(transform);
        bodies[currentIndex].transform.Find("mixamorig:Hips").SetParent(transform);

        GetComponent<Animator>().runtimeAnimatorController = animatorControllers[currentIndex];
    }
}
