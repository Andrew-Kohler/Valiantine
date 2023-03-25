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

// Ok, so what we need is this:
    // When the player is within the "interact zone," they should be able to hit a button and open a menu related to what
    // they're interacting with
    // So, for each of these different types of interactables, perhaps it makes sense to include a progammer's toggle
    // for what they actually are.
    // Then I can check for "Statue" or "Chest," and gather the appropriate info

// Ok
    // It looks like, the way I have this structured, it would be best to have a "Can Interact" bool in GM
    // Then, if we press E and we Can Interact, we can start the process of interacting
    // So the question now is how do we have each of the interactables communicate to this menu their essential data

// Except to keep the code nice, I don't want this menu to be adding things to the player's inventory or healing them,
// this is just text

// So, player components and currentView components are both publicly interactable for anywhere, very clever past me
// But how do we set off the menu and the deets of the interaction itself?
// Ahh, we have pressing E at ANY time in the world broadcast an "Interact" event
// We have entering a trigger activate the "Can Interact" bool in GM
// With these two things combined...we would activate everything in the scene.
// Since the trigger will always be the child of the object of interest, we could beam a reference of that object to the GM
// This feels like I'm overcomplicating things

// Chain of events:
    // Player enters trigger, we tell GameManager Player is allowed to interact with [object]. Simple, done, step 1.
    // Player presses E. This should be managed in PlayerMenuToggle. Them pressing E does a thing:
        // We call Interact() on the object in question.
        // Interact() is a function that, no matter the interactable, sets isInteraction to true,
        // sends desired text inputs to the in-game UI, and starts 3 coroutines:
            // A coroutine relating to the interactable itself, which does things like animating the object and
            // checking the player's pockets.
            // A coroutine in the UI, which reads out the text and opens menus at appropriate times.
            // A coroutine in the player, which animates them correctly.
        // For each type of interactable, we'll essentially have to choreograph the interaction, so that nothing
        // moves forward without the consent of whatever our linchpin is at the moment (waiting on an animation,
        // a text advance, etc.)
        // I'll be darned - you know you're in the weeds when you HAVE to start scripting sequences.

// Before all of that, I do still need to figure out how to have text advance to the next "slide" based on a button press.
