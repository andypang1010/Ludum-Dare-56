using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXButton : MonoBehaviour
{
    private AudioSource hoverFxSource;
    private AudioSource clickFxSource;

    private void Start() {
        hoverFxSource = GameObject.Find("Button Hover").GetComponent<AudioSource>();
        clickFxSource = GameObject.Find("Button Click").GetComponent<AudioSource>();
    }

    public void HoverSound()
    {
        if (hoverFxSource != null)
            hoverFxSource.Play(); // Play the hover sound
    }

    public void ClickSound()
    {
        if (clickFxSource != null)
            clickFxSource.Play(); // Play the click sound
    }
}
