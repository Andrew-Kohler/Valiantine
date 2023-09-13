using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSubAnimatorS : MonoBehaviour
{
    GolemAnimatorS parentAnimator;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        parentAnimator = GetComponentInParent<GolemAnimatorS>();
    }

    // Update is called once per frame
    void Update()
    {
        // The Golem sub-animator plays whatever frame and animation the body is playing
        string clipKey, frameKey;
        clipKey = "_CurrRow";
        frameKey = "_CurrCol";

        // Animate
        int frame = parentAnimator.getGolemFrame();

        meshRenderer.material.SetFloat(clipKey, parentAnimator.getGolemAnimationIndex());
        meshRenderer.material.SetFloat(frameKey, frame);
    }
}
