/*
Unfade
Used on:    Black panel in in-game UI
For:    Fades from black to current scene, used in scene transitions
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unfade : MonoBehaviour
{
    CanvasGroup canvasGroup;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(DoFadeOut());
    }
    IEnumerator DoFadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / 2f;
            yield return null;
        }
        GameManager.Instance.Transition(false); // We are no longer in a transition state
    }

}
