using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField] private float rate = 2f;
    private void OnEnable()
    {
        if(SceneManager.GetActiveScene().name == "23_MainMenu")
        {
            StopAllCoroutines();
            StartCoroutine(DoFadeOut(7f));
        }
        else if(SceneManager.GetActiveScene().name != "0_Exterior" && SceneManager.GetActiveScene().name != "24_Credits")
        {
            StopAllCoroutines();
            StartCoroutine(DoFadeOut(0));
        }
        
        // Explanation so I don't forget: ViewManager initializes and then hides every view on bootup.
        // This wasn't working in SceneLoader because the coroutine was getting started before the in game UI view
        // was ever actually shown. It got to run for a frame before getting turned off and on again.
    }
    public void SceneFadeIn(string levelToLoad)
    {
        StopAllCoroutines();
        StartCoroutine(DoFadeIn(levelToLoad));
    }

    public void NoLoadSceneFadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(DoNoLoadFadeIn());
    }

    public void SceneFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(DoFadeOut(0));
    }

    IEnumerator DoFadeIn(string levelToLoad)
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / rate;
            yield return null;
        }
        SceneManager.LoadScene(levelToLoad);
        yield return null;
    }

    IEnumerator DoNoLoadFadeIn()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / rate;
            yield return null;
        }
    }

    IEnumerator DoFadeOut(float delay)
    {
        GameManager.Instance.Transition(true);
        canvasGroup = GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(delay);
        while (canvasGroup.alpha > 0f)
        {     
            canvasGroup.alpha -= Time.deltaTime / rate;
            //Debug.Log("Alpha:" + canvasGroup.alpha);
            yield return null;
        }
        GameManager.Instance.Transition(false); // We are no longer in a transition state
        yield return null;
    }

}
