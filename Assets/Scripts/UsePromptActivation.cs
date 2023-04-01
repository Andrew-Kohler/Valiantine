using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePromptActivation : MonoBehaviour
{
    [SerializeField] GameObject usePrompt;
    [SerializeField] GameObject interactable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //onPossibleInteraction?.Invoke();
            Debug.Log("Enter");
            interactable.GetComponent<Interactable>().possibleInteraction();
            GameManager.Instance.CanInteract(true);
            usePrompt.GetComponent<UsePrompt>().FadeIn();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exit");
            GameManager.Instance.CanInteract(false);
            usePrompt.GetComponent<UsePrompt>().FadeOut();
        }

    }
}
