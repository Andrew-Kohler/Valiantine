using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEnemy : MonoBehaviour
{
    float fadeSpeed = 3f;

    public void FadeOut()
    {
        StartCoroutine(FadeOutEnemy());
    }
    IEnumerator FadeOutEnemy()
    {
        while(this.GetComponent<Renderer>().material.color.a > 0.01f)
        {
            Color objColor = this.GetComponent<Renderer>().material.color;
            float fadeAmt = objColor.a - (fadeSpeed * Time.deltaTime);

            objColor = new Color(objColor.r, objColor.g, objColor.b, fadeAmt);
            this.GetComponent<Renderer>().material.color = objColor;
            yield return null;
        }
        this.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator FadeInEnemy()
    {
        yield return null;
    }
}

