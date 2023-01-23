/*
Button Interaction
Used on:    Player
For:    Allows the player to interact with certain objects by pressing a button
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    bool interact = false;
    Transform chest;

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && interact)
        {
            chest.Translate(0f, 1f, 0f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            interact = true;
            chest = other.gameObject.transform;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            interact = false;
        }
       
    }

    // If we're in the trigger, we can interact


}
