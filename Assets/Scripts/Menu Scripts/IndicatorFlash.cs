/*
Indicator Flash
Used on:    GameObject - Flash indicator group
For:    Control class for the flash effect that accompanies action indicators appearing
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorFlash : Indicator
{
    // No need for a start method, the parent class Indicator has everything we need

    void Update()
    {
        
    }

    public IEnumerator DoFlashOut() // Called by calling DoFlashIn
    {
        alpha = 1;
        while (alpha > 0)
        {
            for (int i = 0; i < 4; i++)  // Set these to 1f
            {
                sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, alpha);
            }
            alpha -= alphaStep;
            Debug.Log("Flash alpha:" + alpha);
            yield return null;
        }

        yield return null;
    }
}
