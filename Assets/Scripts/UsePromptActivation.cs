using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePromptActivation : MonoBehaviour
{
    [SerializeField] GameObject usePrompt;

    public delegate void OnPossibleInteraction();
    public static event OnPossibleInteraction onPossibleInteraction;    // Event for when player enters interaction zone

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onPossibleInteraction?.Invoke();
            GameManager.Instance.CanInteract(true);
            usePrompt.GetComponent<UsePrompt>().FadeIn();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.CanInteract(false);
            usePrompt.GetComponent<UsePrompt>().FadeOut();
        }

    }
}
