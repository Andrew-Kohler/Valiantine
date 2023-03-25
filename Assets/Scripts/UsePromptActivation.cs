using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePromptActivation : MonoBehaviour
{
    [SerializeField] GameObject usePrompt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            usePrompt.GetComponent<UsePrompt>().FadeIn();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            usePrompt.GetComponent<UsePrompt>().FadeOut();
        }

    }
}
