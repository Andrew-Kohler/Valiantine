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
    float zOffsetExtra = .01f;
    private new void Start()
    {
        base.Start();
        this.SetPositions();
    }
    void Update()
    {
        
    }

    new void SetPositions()
    {
        zeroPos = new Vector3(this.gameObject.transform.parent.position.x + xOffset1, this.gameObject.transform.parent.position.y + yOffset1, this.gameObject.transform.parent.position.z - zOffset + zOffsetExtra);
        onePos = new Vector3(this.gameObject.transform.parent.position.x - xOffset2, this.gameObject.transform.parent.position.y + yOffset2, this.gameObject.transform.parent.position.z + zOffsetExtra);
        twoPos = new Vector3(this.gameObject.transform.parent.position.x - xOffset1, this.gameObject.transform.parent.position.y + yOffset3, this.gameObject.transform.parent.position.z + zOffset + zOffsetExtra);
        threePos = new Vector3(this.gameObject.transform.parent.position.x + xOffset2, this.gameObject.transform.parent.position.y + yOffset2, this.gameObject.transform.parent.position.z + zOffsetExtra);

        indicators[0].transform.position = zeroPos;
        indicators[1].transform.position = onePos;
        indicators[2].transform.position = twoPos;
        indicators[3].transform.position = threePos;
    }

    public IEnumerator DoFlashOut() // Called by calling DoFlashIn in IndicatorAction
    {
        Debug.Log("Flash Out Called");
        alpha = 1;
        while (alpha > 0)
        {
            for (int i = 0; i < 4; i++)  // Set these to 1f
            {
                sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, alpha);
            }
            alpha -= alphaStep * Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    public IEnumerator DoFlashOutFast() // Called by calling DoFlashOut in IndicatorAction
    {
        alpha = 1;
        while (alpha > 0)
        {
            for (int i = 0; i < 4; i++)  // Set these to 1f
            {
                sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, alpha);
            }
            alpha -= alphaStep * 1.25f * Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
