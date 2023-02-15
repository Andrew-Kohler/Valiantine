using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour
{
    CanvasGroup canvasGroup;
    private void OnEnable()
    {
        StartCoroutine(DoFadeOut());
        // Explanation so I don't forget: ViewManager initializes and then hides every view on bootup.
        // This wasn't working in SceneLoader because the coroutine was getting started before the in game UI view
        // was ever actually shown. It got to run for a frame before getting turned off and on again.
    }
    public void SceneFadeIn(string levelToLoad)
    {
        StartCoroutine(DoFadeIn(levelToLoad));
    }

    public void SceneFadeOut()
    {
        StartCoroutine(DoFadeOut());
    }

    IEnumerator DoFadeIn(string levelToLoad)
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / 2f;
            yield return null;
        }
        SceneManager.LoadScene(levelToLoad);
        yield return null;
    }

    IEnumerator DoFadeOut()
    {
        GameManager.Instance.Transition(true);
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0f)
        {     
            canvasGroup.alpha -= Time.deltaTime / 2f;
            Debug.Log("Alpha:" + canvasGroup.alpha);
            yield return null;
        }
        GameManager.Instance.Transition(false); // We are no longer in a transition state
        yield return null;
    }

}
