using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePrompt : MonoBehaviour
{
    // So, we basically just need a "Show" function and a "Hide" function for different stuff to call
    [SerializeField] GameObject text;
    [SerializeField] GameObject backing;

    float fadeSpeed = 3f;

    private void Start()
    {
        Color textColor = text.GetComponent<SpriteRenderer>().material.color;
        Color backingColor = backing.GetComponent<SpriteRenderer>().material.color;

        textColor = new Color(textColor.r, textColor.g, textColor.b, 0);
        text.GetComponent<SpriteRenderer>().material.color = textColor;
        backingColor = new Color(backingColor.r, backingColor.g, backingColor.b, 0);
        backing.GetComponent<SpriteRenderer>().material.color = backingColor;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutPrompt());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInPrompt());
    }

    IEnumerator FadeOutPrompt()
    {
        while (text.GetComponent<SpriteRenderer>().material.color.a > 0.01f)
        {
            Color textColor = text.GetComponent<SpriteRenderer>().material.color;
            Color backingColor = backing.GetComponent<SpriteRenderer>().material.color;
            float fadeAmt = textColor.a - (fadeSpeed * Time.deltaTime);

            textColor = new Color(textColor.r, textColor.g, textColor.b, fadeAmt);
            text.GetComponent<SpriteRenderer>().material.color = textColor;
            backingColor = new Color(backingColor.r, backingColor.g, backingColor.b, fadeAmt);
            backing.GetComponent<SpriteRenderer>().material.color = backingColor;

            yield return null;
        }
        //this.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator FadeInPrompt()
    {
        while (text.GetComponent<SpriteRenderer>().material.color.a <= 1f)
        {
            Color textColor = text.GetComponent<SpriteRenderer>().material.color;
            Color backingColor = backing.GetComponent<SpriteRenderer>().material.color;
            float fadeAmt = textColor.a + (fadeSpeed * Time.deltaTime);

            textColor = new Color(textColor.r, textColor.g, textColor.b, fadeAmt);
            text.GetComponent<SpriteRenderer>().material.color = textColor;
            backingColor = new Color(backingColor.r, backingColor.g, backingColor.b, fadeAmt);
            backing.GetComponent<SpriteRenderer>().material.color = backingColor;

            yield return null;
        }
        //this.gameObject.SetActive(false);
        yield return null;
    }

    // Ok, so we have this, and we show it when the player enters the trigger, and hide it when the player
    // (a) leaves da trigger
    // (b) actually interacts with the thing
    // Let's start with (a)
    // Ok, (a) works GREAT
    // Now b...
}
