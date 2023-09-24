using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimatorS : MonoBehaviour
{
    public enum AnimationAxis { Rows, Columns }

    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 5f;
    [SerializeField] private int animationIndex = 0; // The row of the spritesheet we're on (here, always 0)
    private int frameLoop = 4;  // The frame value the animation resets on
     //private int frameReset = 0; // The frame value the animation resets to

    private float deltaT;

    public delegate void OnChestOpen();
    public static event OnChestOpen onChestOpen;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void PlayOpenAnimation()
    {
        StartCoroutine(DoOpenAnimation());
    }

    private IEnumerator DoOpenAnimation()
    {
        int frame = 0;
        while (frameLoop > frame)
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
            frame = (int)(deltaT * animationSpeed);

            deltaT += Time.deltaTime;
            /*if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }*/
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            yield return null;
        }
        onChestOpen?.Invoke();  // Tell the player animator to do its little dance

    }
}
