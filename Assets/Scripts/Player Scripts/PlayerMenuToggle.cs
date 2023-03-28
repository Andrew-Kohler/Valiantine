/*
Player Menu Toggle
Used on:    Player
For:    Allows the player to open and close relevant in-game menus
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuToggle : MonoBehaviour
{

    private void OnEnable()
    {
        PlayerMovement.onInteractButton += StartInteraction;
        InGameUIView.onInteractionEnd += AllowStartInteraction;
    }

    private void OnDisable()
    {
        PlayerMovement.onInteractButton -= StartInteraction;
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory") )    // If the player presses Q, it should toggle the inventory
        {
            if (!GameManager.Instance.isInventory() && GameManager.Instance.canMove())
            {
                GameManager.Instance.Inventory(true);
            }
            else
            {
                GameManager.Instance.Inventory(false);
            }
        }
        else if (Input.GetButtonDown("Settings / Back") && !GameManager.Instance.isTransition()) // If the player presses Esc, it should toggle the settings
        {
            if (!GameManager.Instance.isSettings())
            {
                GameManager.Instance.Settings(true);
            }
            else
            {
                GameManager.Instance.Settings(false);
            }
        }
        /*else if (GameManager.Instance.isCanInteract() && ) // If the player presses Esc, it should toggle the settings
        {
            
        }*/
    }

    private void StartInteraction() // If we are in an interactable zone and all is well, we trigger the interaction
    {
        if(GameManager.Instance.isCanInteract() && GameManager.Instance.canMove())
        {
            PlayerMovement.onInteractButton -= StartInteraction;
            GameManager.Instance.Interaction(true);
        }
        /*else if (GameManager.Instance.isInteraction())
        {
            GameManager.Instance.Interaction(false);
        }*/

        // Hmmm... this is acting up as soon as it can, sticking us in an endless interaction loop.
        // If we get rid of the listener in StartInteraction and only re-apply it at the end of an interaction...
        // But how do we do that
        // I need some way to inform this class that the interaction has conclude - oh.
        // Event #2.
    }

    private void AllowStartInteraction()
    {
        PlayerMovement.onInteractButton += StartInteraction;
    }
}
