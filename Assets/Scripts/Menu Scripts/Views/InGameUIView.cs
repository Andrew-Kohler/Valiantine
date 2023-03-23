/*
In-Game UI View
Used on:    GameObject
For:    Marks a game object as the in-game UI that's up while the player is moving around
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIView : View
{
    [SerializeField] GameObject interactionMenu;

    bool menuOpen;
    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        menuOpen = false;
    }

    void Update()
    {
        if (GameManager.Instance.isInventory())
        {
            ViewManager.ShowFade<InventoryMenuView>(true);
        }
        else if (GameManager.Instance.isSettings())
        {
            ViewManager.Show<SettingsMenuView>(true);
        }
        else if (GameManager.Instance.isInteraction())
        {
            if (!menuOpen)
            {
                menuOpen = true;
                StartCoroutine(interactionStart("When the text gets read out it should go beepeepeepedeepeep or boodooloodooldooloo like all the best RPGSs"));
            }
            
        }

        // If this gets an event that something is interacted with
            // Fade in the text box
            // Depending on the event, reveal a different bit of text / make the window behave differently

        // The player pressing a key during this should send out an event from the controller that triggers the text advancing
        // Really, we should re-evaluate a lot of the player menu-ing to perhaps be more event based
    }

    IEnumerator interactionStart(string textToRead)
    {
        interactionMenu.GetComponent<FadeUI>().UIFadeIn();
        yield return new WaitForSeconds(.5f);
        interactionMenu.GetComponent<TextRevealer>().ReadOutText(textToRead);
        yield return null;
    }

    // We also need an interaction escape, AND some coe in update for what is actually done during an interaction
}

// Ok, so I think what I actually want is for this to have a submenu, NOT a whole view switch
