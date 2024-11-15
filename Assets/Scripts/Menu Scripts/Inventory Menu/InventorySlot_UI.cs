using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;
    

    public InventorySlot AssignedInventorySlot => assignedInventorySlot; // Getter for assigned inventory slot
    public InventoryDisplay ParentDisplay{get; private set;}
    public bool Selected;

    private void Awake()
    {
        ClearSlot();

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if(slot.Data != null) // If there is something in the slot
        {
            itemSprite.sprite = slot.Data.MenuIcon;
            itemSprite.color = Color.white;

            if (Selected) itemName.fontStyle = FontStyles.Bold;
            else itemName.fontStyle = FontStyles.Normal;

            itemName.text = slot.Data.DisplayName;

            if (slot.StackSize >= 1) itemCount.text = "x" + slot.StackSize.ToString();
            else
            {
                itemCount.text = "";
                ClearSlot();
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()  // We may just want to refresh a slot without passing in a new slot
    {
        if(assignedInventorySlot != null) UpdateUISlot(assignedInventorySlot);
    }

    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot(); // Check if the slot is null; clear it if it is. Represents connection between backend and UI
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemName.text = "";
        itemCount.text = "";
    }

    public void UseItemInSlot()
    {

    }

    public void OnUISlotClick() // Used in the tutorial for mouse interaction with an inventory slot, but I can coopt the structure for keyboard interaction
    {
        ParentDisplay?.SlotClicked(this);
    }

    public bool CheckEmpty()
    {
        if (itemSprite.sprite == null) return true;
        else return false;
    }
}
