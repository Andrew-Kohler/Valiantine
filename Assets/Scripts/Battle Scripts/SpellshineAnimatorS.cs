using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellshineAnimatorS : MonoBehaviour
{
    public enum AnimationAxis { Rows, Columns }

    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 5.4f;
    public int animationIndex = 0;

    private float deltaT;

    private int frame;
    [SerializeField] private int frameLoop = 4;  // A value to hold the number of the frame that the current animation loops on (e.g. after frame 13, loop it)
    //private int frameReset = 0; // A value to hold the number of the frame that the current animation loops back to (e.g. the loop starts on frame 0)
    public bool activeCoroutine = false;    // The classic boolean to use when Update() needs to be quiet during a coroutine

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        deltaT = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isSettings())
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

            // Animate
            int frame = (int)(deltaT * animationSpeed);

            deltaT += Time.deltaTime;
            if (frame > frameLoop)
            {
                Destroy(gameObject);
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
    }
}
