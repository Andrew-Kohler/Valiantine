using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEnemy : MonoBehaviour
{
    float fadeSpeed = 3f;

    private void OnEnable()
    {
        BattleManager.battleHideEnemies += FadeOutIfPassive;
        BattleManager.battleShowEnemies += FadeInIfPassive;
    }

    private void OnDisable()
    {
        BattleManager.battleHideEnemies -= FadeOutIfPassive;
        BattleManager.battleHideEnemies -= FadeInIfPassive;
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
        while (this.GetComponent<Renderer>().material.color.a <= 1f)
        {
            Color objColor = this.GetComponent<Renderer>().material.color;
            float fadeAmt = objColor.a + (fadeSpeed * Time.deltaTime);

            objColor = new Color(objColor.r, objColor.g, objColor.b, fadeAmt);
            this.GetComponent<Renderer>().material.color = objColor;
            yield return null;
        }
        
        yield return null;
    }
}

