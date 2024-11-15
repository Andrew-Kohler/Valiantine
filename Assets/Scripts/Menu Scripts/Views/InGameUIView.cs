/*
In-Game UI View
Used on:    GameObject
For:    Marks a game object as the in-game UI that's up while the player is moving around
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIView : View
{
    [SerializeField] GameObject interactionMenu;
    [SerializeField] Image advanceArrow;
    [SerializeField] TextMeshProUGUI advanceE;

    [SerializeField] FadeUI openingLogo;

    [SerializeField] FadeUI openingText1;
    [SerializeField] FadeUI openingText2;
    [SerializeField] FadeUI openingText3;
    [SerializeField] FadeUI openingText4;

    [SerializeField] FadeUI closingText1;
    [SerializeField] FadeUI closingText2;
    [SerializeField] FadeUI closingText3;
    [SerializeField] FadeUI closingText4;

    [SerializeField] private List<AudioClip> sounds;
    private AudioSource audioS;

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
        audioS = GetComponent<AudioSource>();
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

    public void playIntroConvo()
    {
        StartCoroutine(DoOpeningConvo());
    }

    public void playClosingConvo()
    {
        StartCoroutine(DoClosingConvo());
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
        advanceArrow.color = new Color(.6f, .6f, .6f);
        advanceE.color = new Color(.6f, .6f, .6f);
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
        audioS.PlayOneShot(sounds[0], GameManager.Instance.musicVolume * GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(9f);
        
        SceneLoader.Instance.OnForcedPlayerTransition("1_HereStands");
    }

    private IEnumerator DoOpeningConvo()
    {
        float tempPlayerVol = GameManager.Instance.entityVolume;
        GameManager.Instance.entityVolume = 0;
        int readtime = 5;
        yield return new WaitForSeconds(6f);

        openingText1.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        openingText2.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        openingText3.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        openingText4.UIFadeIn();
        yield return new WaitForSeconds(readtime);
        GameManager.Instance.entityVolume = tempPlayerVol;
        openingText1.UIFadeOutPiece();
        openingText2.UIFadeOutPiece();
        openingText3.UIFadeOutPiece();
        openingText4.UIFadeOutPiece();

    }

    private IEnumerator DoClosingConvo()
    {
        float tempPlayerVol = GameManager.Instance.entityVolume;
        GameManager.Instance.entityVolume = 0;
        int readtime = 5;
        yield return new WaitForSeconds(2f);

        closingText1.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        closingText2.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        closingText3.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        closingText4.UIFadeIn();
        yield return new WaitForSeconds(readtime - 1f);
        GameManager.Instance.entityVolume = tempPlayerVol;
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
