/*
Fade UI
Used on:    Battle UI
For:    Holds methods for fading and unfading UI elements (battle UI)
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeUI : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public void BattleFadeIn()  // Methods used for the battle UI
    {
        StartCoroutine(DoFadeInUI());
    }

    public void BattleFadeOut()
    {
        StartCoroutine(DoFadeOutUI());
    }

    IEnumerator DoFadeInUI()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 3f;
            yield return null;
        }
        
    }
    IEnumerator DoFadeOutUI()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 3f;
            yield return null;
        }
    }
}


