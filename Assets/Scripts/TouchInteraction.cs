/*
In-Game UI View
Used on:    Objects interactable by touch
For:    Triggers an interaction when the player touches the object
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.transform.Translate(0f, 1f, 0f);
        }
        
    }
}
