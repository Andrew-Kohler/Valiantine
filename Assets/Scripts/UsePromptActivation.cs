using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePromptActivation : MonoBehaviour
{
    [SerializeField] GameObject usePrompt;
    [SerializeField] GameObject interactable;

    public bool attachedToInteractable = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            /*if(interactable.GetComponent<ChestInteractable>() != null)  // Chests can only be used once, so we gotta check
            {
                if (!interactable.GetComponent<ChestInteractable>().Opened)
                {
                    interactable.GetComponent<Interactable>().possibleInteraction();
                    GameManager.Instance.CanInteract(true);
                    usePrompt.GetComponent<UsePrompt>().FadeIn();
                }
            }
            else
            {*/
            if (attachedToInteractable)
            {
                interactable.GetComponent<Interactable>().possibleInteraction();
                GameManager.Instance.CanInteract(true);
            }
                
                usePrompt.GetComponent<UsePrompt>().FadeIn();
            //}
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (attachedToInteractable)
            {
                GameManager.Instance.CanInteract(false);
            }
            
            usePrompt.GetComponent<UsePrompt>().FadeOut();
        }

    }
}
