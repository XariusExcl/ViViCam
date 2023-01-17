using UnityEngine;
using System.Collections;

public class InitialSettings : MonoBehaviour
{
    int displaysConnectedCount;
    void Start()
    {
        // Activate displays
        displaysConnectedCount = Display.displays.Length;
        Debug.Log ("displays connected: " + displaysConnectedCount);
            // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
            // Check if additional displays are available and activate each.
    
        for (int i = 1; i < displaysConnectedCount; i++)
        {
            Display.displays[i].Activate();
        }

        // Set Vsync
        QualitySettings.vSyncCount = 1;
        // Application.targetFrameRate = 60;
    }
}