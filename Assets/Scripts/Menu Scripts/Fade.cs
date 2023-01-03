using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
   public void FadeIn(string levelToLoad)
    {
        StartCoroutine(DoFadeIn(levelToLoad));
    }

    IEnumerator DoFadeIn(string levelToLoad)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / 2f;
            yield return null;   
        }
        SceneManager.LoadScene(levelToLoad);
    }

}

// https://forum.unity.com/threads/fade-to-black.459450/
