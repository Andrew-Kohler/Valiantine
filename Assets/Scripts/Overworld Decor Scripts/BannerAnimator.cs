/*
Banner Animator
Used on:    Banner Decoration
For:    Allows banners to react to wind
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAnimator : MonoBehaviour
{
    private bool windStarted;
    private bool windEnded;

    public enum AnimationAxis { Rows, Columns }
    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private int animationIndex = 0;
    private float frame;
    private int frameLoop = 1;  // The frame value the animation resets on
    private int frameReset = 0; // The frame value the animation resets to

    // This is the one where I figured out I didn't need deltaT

    private int transitionOneStartIndex = 1;    // Start index for transiton from static to blowing
    private int blowingLoopStartIndex = 4;      // Start index for blowing loop
    private int blowingLoopEndIndex = 9;      // End index for blowing loop
    private int transitionTwoEndIndex = 14;   // End index for transition from blowing to static

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        windStarted = false;
        windEnded = true;
    }

    // Update is called once per frame
    void Update()
    {
        string clipKey, frameKey;
        if (axis == AnimationAxis.Rows)
        {
            clipKey = rowProperty;
            frameKey = colProperty;
        }
        else
        {
            clipKey = colProperty;
            frameKey = rowProperty;
        }

        if (GameManager.Instance.IsWindy()) // If the wind kicks up...
        {
            if (!windStarted)   // This lets us go from not blowing to blowing smoothly
            {
                windStarted = true;
                windEnded = false;
                frame = transitionOneStartIndex;
                frameReset = blowingLoopStartIndex;
                frameLoop = blowingLoopEndIndex;
            }

            frame += (Time.deltaTime * animationSpeed);
            if (frame >= frameLoop)
            {
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, (int)frame);
        } // end of top if statement
        else
        {
            if (!windEnded) // Do the cooldown routine
            {
                frameLoop = transitionTwoEndIndex;
                frame += (Time.deltaTime * animationSpeed);
                if (frame >= frameLoop)
                {
                    windEnded = true;
                    windStarted = false;
                }
                meshRenderer.material.SetFloat(clipKey, animationIndex);
                meshRenderer.material.SetFloat(frameKey, (int)frame);
            }
            else
            {
                meshRenderer.material.SetFloat(clipKey, animationIndex);
                meshRenderer.material.SetFloat(frameKey, 0);
            }
        }
    }
}
