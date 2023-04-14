using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : Interactable
{
    [SerializeField] ItemData chestItem;
    [SerializeField] int numItems;
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject chestSprite;
    [SerializeField] GameObject itemSprite;

    UsePrompt prompt;
    ChestAnimatorS chestAnimator;
    ChestItemDisplay chestItemDisplay;

    private bool opened;

    public bool Opened => opened;

    

    private void Start()
    {
        prompt = usePrompt.GetComponent<UsePrompt>();
        chestAnimator = chestSprite.GetComponent<ChestAnimatorS>();
        chestItemDisplay = itemSprite.GetComponent<ChestItemDisplay>();

        

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
        chestItemDisplay.setItemSprite(chestItem.OverworldIcon);
        prompt.FadeOut(); // Fades out the little indicator
        yield return new WaitForSeconds(1f);

        PlayerManager.Instance.PlayerInventory().InventorySystem.AddToInventory(chestItem, numItems); // Give the player the item
        chestAnimator.PlayOpenAnimation(); // Open the chest and wait for it to open

        yield return new WaitForSeconds(2f);

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);

        // Show the item sprite
        chestItemDisplay.showItem();

        trigger.SetActive(false);
        opened = true;
        yield return null;
    }
}
