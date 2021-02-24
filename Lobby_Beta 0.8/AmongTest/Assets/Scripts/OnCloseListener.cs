﻿using UnityEngine;
using UnityEngine.UI;

public class OnCloseListener : MonoBehaviour
{
    public Image backgroundImage;

    /// <summary>
    /// Called when the Browser is closed.
    /// </summary>
    public void OnClose()
    {
        // Uncomment this if you set up Interop
        //BrowserJS.Warn("This warning was called from Unity!");

        // Randomize the background image color
        this.backgroundImage.color = new Color(Random.value, Random.value, Random.value);
    }
}