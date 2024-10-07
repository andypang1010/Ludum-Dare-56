using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXButton : MonoBehaviour
{
    public AudioSource hoverFxSource; // AudioSource for hover sound
    public AudioSource clickFxSource; // AudioSource for click sound

    public void HoverSound()
    {
        hoverFxSource.Play(); // Play the hover sound
    }

    public void ClickSound()
    {
        clickFxSource.Play(); // Play the click sound
    }
}
