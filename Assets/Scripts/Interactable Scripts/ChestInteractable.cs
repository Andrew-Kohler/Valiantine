using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : Interactable
{
    [SerializeField] ItemData chestItem;
    [SerializeField] int numItems;
    [SerializeField] GameObject trigger;

    private bool opened;

    public bool Opened => opened;

    private void Start()
    {
        if(numItems == 1)
        {
            lines.Add(chestItem.SingleObtainDescription);
        }
        
    }
    public override void Interact()
    {
        if (!opened)
        {
            StartCoroutine(DoInteraction());
        }
        
    }

    protected override IEnumerator DoInteraction()
    {
        usePrompt.GetComponent<UsePrompt>().FadeOut(); // Fades out the little indicator

        PlayerManager.Instance.PlayerInventory().InventorySystem.AddToInventory(chestItem, numItems);

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);

        trigger.SetActive(false);
        opened = true;
        yield return null;
    }
}
