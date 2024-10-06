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
        UpdateProgressBar();
    }
    public void UpdateProgressBar()
    {
        LevelSlider.value = currentPoly;
    }
    public void GainPoly(int polyNumber)
    {
        currentPoly += polyNumber;
        if (currentPoly >= polyRequiredToNextLevel)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        if (currentLevel + 1 > maxLevel) { return; }
        currentPoly -= polyRequiredToNextLevel;
        currentLevel++;
        polyRequiredToNextLevel = 1.5f * polyRequiredToNextLevel;
        Debug.Log("Level Up! Now at level " + currentLevel);
        LevelSlider.maxValue = polyRequiredToNextLevel;
        AdjustProgressBarSize();
    }
    
    public void AdjustProgressBarSize()
    {
        float oldWidth = progressBarFillRect.sizeDelta.x;
        float newWidth = oldWidth * 1.5f;
        progressBarFillRect.sizeDelta = new Vector2(newWidth, progressBarFillRect.sizeDelta.y);
        float widthIncrease = newWidth - oldWidth;
        Vector3 currentPos = progressBarSliderRect.anchoredPosition;
        progressBarSliderRect.anchoredPosition = new Vector3(currentPos.x + (widthIncrease), currentPos.y, currentPos.z);
    }
}
