using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimator : MonoBehaviour
{

    public enum AnimationAxis { Rows, Columns }
    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private int animationIndex = 0;
    private float frame;
    private int frameLoop = 2;  // The frame value the animation resets on
    private int frameReset = 0; // The frame value the animation resets to

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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


        frame += (Time.deltaTime * animationSpeed);
        if (frame > frameLoop)
        {
            
            frame = frameReset;
        }
        meshRenderer.material.SetFloat(clipKey, animationIndex);
        meshRenderer.material.SetFloat(frameKey, (int)frame);
        Debug.Log(frame);

    }
}
