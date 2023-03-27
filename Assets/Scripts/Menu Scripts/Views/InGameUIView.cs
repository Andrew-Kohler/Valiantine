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
    bool activeCoroutine;
    public bool textReadout;

    //Testing only
    string line1 = "This is smoke and mirrors; there's currently no relation between this readout and the statue.";
    string line2 = "However, it's still very cool, and the first step towards the looming task of systems integrations.";
    List<string> lines = new List<string>();
    

    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        menuOpen = false;
        activeCoroutine = false;
        textReadout = false;
        lines.Add(line1);
        lines.Add(line2);
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
                StartCoroutine(interactionStart(lines));
            }
            
        }

        // If this gets an event that something is interacted with
            // Fade in the text box
            // Depending on the event, reveal a different bit of text / make the window behave differently

        // The player pressing a key during this should send out an event from the controller that triggers the text advancing
        // Really, we should re-evaluate a lot of the player menu-ing to perhaps be more event based
    }

    IEnumerator interactionStart(List<string> lines)
    {
        activeCoroutine = true;
        interactionMenu.GetComponent<FadeUI>().UIFadeIn();
        yield return new WaitForSeconds(.5f);
        foreach (string line in lines)
        {
            textReadout = false;
            interactionMenu.GetComponent<TextRevealer>().ReadOutText(line);
            yield return new WaitUntil(() => textReadout);
            yield return new WaitForSeconds(1f);
        }

        activeCoroutine = false;
        yield return null;
    }

    // We also need an interaction escape, AND some code in update for what is actually done during an interaction
}

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
// Now the problem becomes:
//  When the player hits "E" and the text is reading, how do we advance it to the end?
//  When we CAN tell, how do we advance to the next one when the player hits E?

// The problem sort of extends from having no good place to put the "E" press. If ONLY there was a way to broadcast that...
// Yeah, events.

// Oh my god, past Andrew is a genius and so sexy and I would kiss him if I could.
// So, we make this event delegate, call it Press E or whatever. Heck, let's make it an action, why not.
// If we're within an interaction, we subscribe an "Advance Text" function to the E press event.
// Then, that function can carry out the possibilities 
