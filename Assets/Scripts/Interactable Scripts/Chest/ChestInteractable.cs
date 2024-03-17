using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestInteractable : Interactable
{
    [Header("Save Index")]
    public int saveIndex;
    [Header("Contents")]
    [SerializeField] ItemData chestItem;
    [SerializeField] int numItems;
    [SerializeField] GameObject trigger;
    UsePrompt prompt;
    [Header("Sprites")]
    [SerializeField] GameObject chestSprite;
    [SerializeField] GameObject itemSprite;

    
    [Header("Animation")]
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

        if (GameManager.Instance.openedChests[saveIndex]) // If this chest has been opened, it shall not be re-opened for additional gain
        {
            AlreadyOpened();
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
        GameManager.Instance.openedChests[saveIndex] = true; // Save that we have opened this chest
        yield return null;
    }

    private void AlreadyOpened()
    {
        chestAnimator.SetOpen();
        opened = true;
        trigger.SetActive(false);
    }
}
