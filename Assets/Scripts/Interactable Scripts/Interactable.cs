using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected List<string> lines = new List<string>();
    [SerializeField] protected GameObject usePrompt;

    public abstract void Interact();

    protected abstract IEnumerator DoInteraction();

    public void possibleInteraction() // If there is a chance the player will interact with this object, send its data to GameManager
    {
        GameManager.Instance.CurrentInteractable(this.gameObject);
    }

}
