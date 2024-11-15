using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEnemy : MonoBehaviour
{
    //float fadeSpeed = 2f;

    private void OnEnable()
    {
        BattleManager.battleHideEnemies += FadeOutIfPassive;
        BattleManager.battleShowEnemies += FadeInIfPassive;
    }

    private void OnDisable()
    {
        BattleManager.battleHideEnemies -= FadeOutIfPassive;
        BattleManager.battleShowEnemies -= FadeInIfPassive;
    }

    private void OnDestroy()
    {
        BattleManager.battleHideEnemies -= FadeOutIfPassive;
        BattleManager.battleShowEnemies -= FadeInIfPassive;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutEnemy());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInEnemy());  
    }

    public void FadeOutIfPassive()   // These work with the more decentralized battle manager
    {
        if (!this.gameObject.GetComponent<EnemyStats>().isBattling)
        {
            StartCoroutine(FadeOutEnemy());
        }
            
    }

    public void FadeInIfPassive()
    {
        if (!this.gameObject.GetComponent<EnemyStats>().isBattling)
        {
            StartCoroutine(FadeInEnemy());
        }
    }

    /*public void FadeOutDestroy()
    {
        StartCoroutine(DoFadeOutDestroy());
    }*/

    IEnumerator FadeOutEnemy()
    {
        /*while(this.GetComponentInChildren<Renderer>().material.GetFloat("_Alpha_Value") < .9f)
        {
            float objColor = this.GetComponentInChildren<Renderer>().material.GetFloat("_Alpha_Value");  // Get the current value
            float fadeAmt = objColor + (fadeSpeed * Time.deltaTime);                                    // Add an increment to it
            this.GetComponentInChildren<Renderer>().material.SetFloat("_Alpha_Value", fadeAmt);         // Increment the real thing
            yield return null;
        }*/
        this.gameObject.SetActive(false);
        yield return null;
    }

   /* IEnumerator DoFadeOutDestroy()
    {
        while (this.GetComponent<Renderer>().material.color.a > 0.01f)
        {
            Color objColor = this.GetComponent<Renderer>().material.color;
            float fadeAmt = objColor.a - (fadeSpeed * Time.deltaTime);

            objColor = new Color(objColor.r, objColor.g, objColor.b, fadeAmt);
            this.GetComponent<Renderer>().material.color = objColor;
            yield return null;
        }
        Destroy(this.gameObject);
        yield return null;
    }*/

    IEnumerator FadeInEnemy()
    {
        /*while (this.GetComponentInChildren<Renderer>().material.GetFloat("_Alpha_Value") > .01f)
        {
            float objColor = this.GetComponentInChildren<Renderer>().material.GetFloat("_Alpha_Value");  // Get the current value
            float fadeAmt = objColor + (fadeSpeed * Time.deltaTime);                                    // Subtract an increment from it
            this.GetComponentInChildren<Renderer>().material.SetFloat("_Alpha_Value", fadeAmt);         // Increment the real thing
            yield return null;
        }*/
        
        yield return null;
    }
}

