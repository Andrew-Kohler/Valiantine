using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardinalAnimatorS : MonoBehaviour
{
    public enum AnimationAxis { Rows, Columns }

    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private int animationIndex = 0;
    private int frameLoop = 1;  // The frame value the animation resets on
    private int frameReset = 0; // The frame value the animation resets to

    private float deltaT;
    public int dir;
    public bool flyingAway;

    private int rightRowIndex = 0;
    private int leftRowIndex = 1;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        deltaT = 0;
        dir = 0;
    }

    private void Update()
    {
        if (!GameManager.Instance.isSettings())
        {
            if (!flyingAway)
            {
                frameLoop = 1;
                frameReset = 0;
                if(dir == 0)
                {
                    animationIndex = rightRowIndex;
                }
                else
                {
                    animationIndex = leftRowIndex;
                }
            }
            else
            {
                frameLoop = 3;
                frameReset = 1;
                if (dir == 0)
                {
                    animationIndex = rightRowIndex;
                }
                else
                {
                    animationIndex = leftRowIndex;
                }
            }

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

            // Animate
            int frame = (int)(deltaT * animationSpeed);

            deltaT += Time.deltaTime;
            if (frame >= frameLoop) // Might be messing with this soon!
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
    }
}
