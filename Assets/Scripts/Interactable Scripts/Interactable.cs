using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected List<string> lines = new List<string>();
    [SerializeField] protected GameObject usePrompt;
    //[SerializeField] protected GameObject interactionTrigger;

    /*private void OnEnable()
    {
        UsePromptActivation.onPossibleInteraction += possibleInteraction;
    }

    private void OnDisable()
    {
        UsePromptActivation.onPossibleInteraction -= possibleInteraction;
    }*/
    public abstract void Interact();

    protected abstract IEnumerator DoInteraction();

    public void possibleInteraction() // If there is a chance the player will interact with this object, send its data to GameManager
    {
        GameManager.Instance.CurrentInteractable(this.gameObject);
        // So, uh, big oversight on my part here.
        // If the player enters ANY UsePromptActivation, this gets triggered.
        // Which means that, right now, if the player enters ANY interaction zone, EVERY interactable's
        // data gets sent to GameManager at once.
        // Which, you know, no biggie. Each interactable just has to have a closer association with their detection zone
        // that's a little more intertwined than an event.
    }

}
