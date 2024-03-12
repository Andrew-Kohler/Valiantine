using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public delegate void OnChestInteract();
    public static event OnChestInteract onChestInteract;

    private void Start()
    {
        prompt = usePrompt.GetComponent<UsePrompt>();
        chestAnimator = chestSprite.GetComponent<ChestAnimatorS>();
        chestItemDisplay = itemSprite.GetComponent<ChestItemDisplay>();

        if(numItems == 1)
        {
            lines.Add(chestItem.SingleObtainDescription);
        }
        else
        {
            lines.Add(chestItem.MultipleObtainDescription);
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
        // Do an event HERE to tell the player to turn around
        onChestInteract?.Invoke();
        yield return new WaitForSeconds(1f);

        PlayerManager.Instance.PlayerInventory().InventorySystem.AddToInventory(chestItem, numItems); // Give the player the item
        chestAnimator.PlayOpenAnimation(); // Open the chest

        yield return new WaitForSeconds(2f); // Wait for the chest to open

        // Play the jump around animation
        // Wait for it to finish

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);

        // Show the item sprite
        chestItemDisplay.enabled = true;
        chestItemDisplay.showItem();

        trigger.SetActive(false); // Deactivate the pickup trigger
        //Destroy(trigger);
        GameManager.Instance.Animating(true);

        opened = true;
        yield return null;
    }
}
