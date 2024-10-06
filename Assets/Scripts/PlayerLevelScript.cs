using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelScript : MonoBehaviour
{
    public int currentLevel = 1;
    //current Poly(XP value)
    public static int maxLevel = 4;
    public float currentPoly = 0f;
    public float polyRequiredToNextLevel = 100f;
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

        currentPoly -= polyRequiredToNextLevel;
        polyRequiredToNextLevel *= 1.5f;

        Debug.Log("Level Up! Now at level " + currentLevel);
        LevelSlider.maxValue = polyRequiredToNextLevel;
        IncreaseBarSize();
    }

    private void LevelDown()
    {
        if (currentLevel - 1 < 1)
        {
            ShowLose();
            return;
        }
        currentLevel--;

        polyRequiredToNextLevel /= 1.5f;
        currentPoly = polyRequiredToNextLevel;

        Debug.Log("Level Down! Now at level " + currentLevel);
        LevelSlider.maxValue = polyRequiredToNextLevel;
        DecreaseBarSize();
    }

    private void IncreaseBarSize()
    {
        float oldWidth = progressBarFillRect.sizeDelta.x;
        float newWidth = oldWidth * 1.5f;
        progressBarFillRect.sizeDelta = new Vector2(newWidth, progressBarFillRect.sizeDelta.y);
        float widthIncrease = newWidth - oldWidth;
        Vector3 currentPos = progressBarSliderRect.anchoredPosition;
        progressBarSliderRect.anchoredPosition = new Vector3(currentPos.x + widthIncrease, currentPos.y, currentPos.z);
    }

    private void DecreaseBarSize()
    {
        float oldWidth = progressBarFillRect.sizeDelta.x;
        float newWidth = oldWidth / 1.5f;
        progressBarFillRect.sizeDelta = new Vector2(newWidth, progressBarFillRect.sizeDelta.y);
        float widthIncrease = newWidth - oldWidth;
        Vector3 currentPos = progressBarSliderRect.anchoredPosition;
        progressBarSliderRect.anchoredPosition = new Vector3(currentPos.x + widthIncrease, currentPos.y, currentPos.z);
    }
    public void ShowWin()
    {
        sc.Win();
    }
    public void ShowLose()
    {
        sc.Lose();
    }
}
