using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    public List<InventorySlot> InventorySlots => inventorySlots;    // Ohh, this functions as a getter method
    public int InventorySize => inventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public delegate void OnGemObtain(ItemData data);
    public static event OnGemObtain onGemObtain;

    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);
        for(int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(ItemData itemToAdd, int amountToAdd)
    {
        if(ContainsItem(itemToAdd, out List<InventorySlot> invSlot)) // If the inventory contains the item
        {
            foreach (var slot in invSlot) // Check if the slot has room left and add the item to it if it does
            {
                if (slot.RoomLeftInStack(amountToAdd))
                {
                    slot.AddToStack(amountToAdd);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
            
        }

        if(HasFreeSlot(out InventorySlot freeSlot) && !ContainsItem(itemToAdd, out List<InventorySlot> invSlot2)) // If it doesn't contain the item but DOES have a free slot
        {   // The ContainsItem here is present to allow an item to be picked up, but to prevent a new stack from being made
            if(itemToAdd.HPRestore != -1)   // If the item isn't a gem
            {
                freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                OnInventorySlotChanged?.Invoke(freeSlot);
                
            }
            else // If the item IS a gem
            {
                //Debug.Log(itemToAdd.DisplayName);
                onGemObtain?.Invoke(itemToAdd);
            }
            return true;

        }

        return false;
    }

    public bool RemoveFromInventory(ItemData itemToRemove, int amountToRemove, int index) // Removes a given item from the inventory
    {
        if (ContainsItem(itemToRemove, out List<InventorySlot> invSlot)) // If the inventory contains the item
        {
            invSlot[0].RemoveFromStack(amountToRemove); // Remove the item
            OnInventorySlotChanged?.Invoke(invSlot[0]);

            //Debug.Log(ContainsItem(itemToRemove, out List<InventorySlot> invSlot2));
            if(!ContainsItem(itemToRemove, out List<InventorySlot> invSlot4)){   // If there are no more items in the slot
               // Debug.Log("Moving on up");
                MoveItemsUp(index);
            }

            return true;
        }

        return false;
    }
    

    public bool ContainsItem(ItemData itemToAdd, out List<InventorySlot> invSlot) // Check if our inventory contains an item
    {
        // Ok, so this is sort of like a 1 line for and if all in one
        // It goes through all the items, and if the item data in a slot == the item we're adding, it drops it in the list
        invSlot = InventorySlots.Where(i => i.Data == itemToAdd).ToList(); // Linq madness

        //return invSlot == null ? false : true; 
        return invSlot.Count == 0 ? false : true;

    }

    public bool ContainsItem(ItemData itemToCheck) // Check if our inventory contains an item
    {
        // Ok, so this is sort of like a 1 line for and if all in one
        // It goes through all the items, and if the item data in a slot == the item we're adding, it drops it in the list
        List<InventorySlot> invSlot = InventorySlots.Where(i => i.Data == itemToCheck).ToList(); // Linq madness

        //return invSlot == null ? false : true; 
        return invSlot.Count == 0 ? false : true;

    }

    public bool HasFreeSlot(out InventorySlot freeSlot) // Check if we have a free empty slot
    {
        // So, I think this "out" notation is kind of like a cheat free extra return?

        freeSlot = InventorySlots.FirstOrDefault(i => i.Data == null); // Gets the first empty slot
        return freeSlot == null ? false : true;
    }

    private void MoveItemsUp(int index) // Moves all items below the given index up one slot
    {
        for(int i = index + 1; i < inventorySlots.Count; i++)
        { 
            inventorySlots[i-1].UpdateInventorySlot(inventorySlots[i]);
            inventorySlots[i].ClearSlot();
        }
    }
}
