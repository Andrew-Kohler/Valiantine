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
    [SerializeField] FadeUI openingLogo;

    //bool menuOpen;              // Whether or not the interaction sub-menu is open
    //bool activeCoroutine;       // Whether or not a coroutine is active
    public bool textReadout;    // Whether or not TextRevealer has finished reading the current text block
    bool textAdvance;           // Whether or not the player has given the go-ahead to advance to the next text block/close the window

    public delegate void OnInteractionEnd();
    public static event OnInteractionEnd onInteractionEnd;  

    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        //menuOpen = false;
        //activeCoroutine = false;
        textReadout = false;
        textAdvance = false;
        //lines.Add(line1);
        //lines.Add(line2);
    }

    private void OnEnable()
    {
        PlayerMovement.onInteractButton += AdvanceText;
        GateInteractable.onCastleEnter += showLogo;
    }

    private void OnDisable()
    {
        PlayerMovement.onInteractButton -= AdvanceText;
        GateInteractable.onCastleEnter -= showLogo;
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
        /*else if (GameManager.Instance.isInteraction())
        {
            if (!menuOpen && !activeCoroutine)
            {
                menuOpen = true;
                StartCoroutine(interactionText(lines));
            }
            
        }*/
    }

    public void startInteractionText(List<string> lines)
    {
        StartCoroutine(interactionText(lines));
    }

    private void AdvanceText()
    {
        if (textReadout)    // We can only advance to the next bit if it's all read out
        {
            textAdvance = true;
        }
        
    }

    private void showLogo()
    {
        StartCoroutine(DoTitleCard());
    }

    IEnumerator interactionText(List<string> lines) // Fades in the text box and reads out each line of text given
    {
        //activeCoroutine = true;
        interactionMenu.GetComponent<FadeUI>().UIFadeIn();
        yield return new WaitForSeconds(.5f);

        foreach (string line in lines)
        {
            textReadout = false;
            textAdvance = false;
            interactionMenu.GetComponent<TextRevealer>().ReadOutText(line);
            yield return new WaitUntil(() => textReadout);  // Wait for the text to be done reading out
            yield return new WaitUntil(() => textAdvance);  // Wait for the player to give the go-ahead to advance
        }

        StartCoroutine(interactionEnd());
        yield return null;
    }

    IEnumerator interactionEnd()    // Fades out the text box
    {
        GameManager.Instance.Interaction(false);
        //activeCoroutine = true;
        interactionMenu.GetComponent<FadeUI>().UIFadeOut();
        yield return new WaitForSeconds(.5f);
        interactionMenu.GetComponent<TextRevealer>().ReadOutText("");

        //menuOpen = false;
        //activeCoroutine = false;
        onInteractionEnd?.Invoke();
        GameManager.Instance.CanInteract(false);
    }

    private IEnumerator DoTitleCard()
    {
        yield return new WaitForSeconds(1f);
        openingLogo.UIFadeIn();
    }

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
//  When we CAN tell, how do we advance to the next one when the player hits E?
