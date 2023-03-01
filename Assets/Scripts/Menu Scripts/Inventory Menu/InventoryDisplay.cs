using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InventoryDisplay : MonoBehaviour // This is the one that actually displays the inventory
{

    protected InventorySystem inventorySystem;  // The inventory system that we're getting the yummy data from and displaying
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary; // This is just a data type that lets you store a key/value pair

    public InventorySystem InventorySystem => inventorySystem;  // Getters
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot) // We pass in an inventory slot (backend), find it in the dictionary, and update its UI counterpart
    {
        foreach(var slot in SlotDictionary) // Value is the backend, 
        {
            if(slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    public void SlotClicked(InventorySlot_UI clickedSlot) //Again, co-opting architecture
    {
        Debug.Log("Slot clicked");
    }
    
}
