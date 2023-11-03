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
    [SerializeField] float modifier = 3f;

    public void UIFadeIn()  // Methods used for the battle UI
    {
        StopAllCoroutines();
        StartCoroutine(DoFadeInUI());
    }

    public void UIFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(DoFadeOutUI());
    }

    IEnumerator DoFadeInUI()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1f)
        {
          //  Debug.Log("Right foot in");
            canvasGroup.alpha += Time.deltaTime * modifier;
            yield return null;
        }
        
    }
    IEnumerator DoFadeOutUI() // So, for fading in, it's better to switch using the view manager and then call the coroutine
    {                           // But for fading out, we don't want to deactivate until this is done, so it's better to do the call in here
        canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * modifier;
            yield return null;
        }

        if (!GameManager.Instance.isBattle())
        {
            ViewManager.ShowLast();
        }
        else
        {
            // Listen, the ONLY time this clause is ever invoked is backing out of the inventory in battle. I caved and made it
            // a specific method. Let me have this.

            // Ok, left off here pondering the fact that if I fade out the BattleUI when I fade the inventory in, this will
            // also get invoked, and I don't want that
            if (!GameManager.Instance.isInventory())
            {
                ViewManager.ShowLastFade(); // Check ViewManager for explanation
            }
            else
            {
                ViewManager.ShowFade<InventoryMenuView>(true);
            }
            
        }
        yield return null;    
    }
}


