/*
Fade UI
Used on:    Menus and menu elements
For:    Holds methods for fading and unfading UI elements (battle UI)
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public void UIFadeIn()  // Methods used for the battle UI
    {
        StartCoroutine(DoFadeInUI());
    }

    public void UIFadeOut()
    {
        StartCoroutine(DoFadeOutUI());
    }

    IEnumerator DoFadeInUI()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1f)
        { 
            canvasGroup.alpha += Time.deltaTime * 3f;
            yield return null;
        }
        
    }
    IEnumerator DoFadeOutUI() // So, for fading in, it's better to switch using the view manager and then call the coroutine
    {                           // But for fading out, we don't want to deactivate until this is done, so it's better to do the call in here
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 3f;
            yield return null;
        }
        ViewManager.ShowLast();
        yield return null;    
    }
}


