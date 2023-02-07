/*
Indicator
Used on:    ---
For:    Parent class for indicator sets
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    protected Vector3 zeroPos;    // Position of the bottom indicator
    protected Vector3 onePos;     // Position of the left indicator
    protected Vector3 twoPos;     // Position of the top indicator
    protected Vector3 threePos;   // Position of the right indicator

    protected GameObject[] indicators;    // Array of the 4 indicators
    protected SpriteRenderer[] sr;        // Array of each indicator's sprite renderer

    [SerializeField] protected float xOffset1 = 1.5f; // x offset of the top and bottom indicators
    [SerializeField] protected float xOffset2 = 2.5f; // x offset of the left and right indicators
    [SerializeField] protected float yOffset1 = 4.75f;// y offset of the bottom indicator
    [SerializeField] protected float yOffset2 = 5.1f; // y offset of the left and right indicators
    [SerializeField] protected float yOffset3 = 5.5f; // y offset of the top indicator
    [SerializeField] protected float zOffset = 3f;    // z offset of the top and bottom indicators

    protected float alpha;                                // The alpha value of the indicators (0-1)
    [SerializeField] protected float alphaStep = .01f;                            // How fast the indicators fade in/out on battle entry/exit

    protected void Start()
    {
        indicators = new GameObject[4];
        sr = new SpriteRenderer[4];

        for (int i = 0; i < 4; i++)  // Get all of the indicator boxes within this parent and by default set them to be transparent
        {
            indicators[i] = this.gameObject.transform.GetChild(i).gameObject;
            sr[i] = indicators[i].GetComponent<SpriteRenderer>();
            sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, 0f);
        }

        SetPositions();
        enabled = false; // We want these scripts to begin un-enabled so they aren't running during exploration
    }

    protected void SetPositions() // Sets the coordinate positions of the indicators based on their array positions
    {
        zeroPos = new Vector3(this.gameObject.transform.parent.position.x + xOffset1, this.gameObject.transform.parent.position.y + yOffset1, this.gameObject.transform.parent.position.z - zOffset);
        onePos = new Vector3(this.gameObject.transform.parent.position.x - xOffset2, this.gameObject.transform.parent.position.y + yOffset2, this.gameObject.transform.parent.position.z);
        twoPos = new Vector3(this.gameObject.transform.parent.position.x - xOffset1, this.gameObject.transform.parent.position.y + yOffset3, this.gameObject.transform.parent.position.z + zOffset);
        threePos = new Vector3(this.gameObject.transform.parent.position.x + xOffset2, this.gameObject.transform.parent.position.y + yOffset2, this.gameObject.transform.parent.position.z);

        indicators[0].transform.position = zeroPos;
        indicators[1].transform.position = onePos;
        indicators[2].transform.position = twoPos;
        indicators[3].transform.position = threePos;
    }

}
