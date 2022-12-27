using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    // While we are touching it, we want to be able to interact with it by hitting a button
    /*private void OnCollisionStay(Collision collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (collision.gameObject.CompareTag("Interactable")){
            
                collision.gameObject.transform.Translate(0f, 1f, 0f);
            }
        }
    }*/
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
