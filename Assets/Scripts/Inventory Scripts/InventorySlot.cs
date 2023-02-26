using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int stackSize;

    public ItemData Data => itemData;   // We can change these from outside the script, but also get a reference to them as 
                                        // public properties
    public int StackSize => stackSize;
    public InventorySlot(ItemData source, int amount) // Initializes a slot with something in it
    {
        itemData = source;
        stackSize = amount;
    }

    public InventorySlot() // Initializes an empty slot
    {
        ClearSlot();
    }

    public void ClearSlot() // Clears the inventory slot
    {
        itemData = null;
        stackSize = -1;
    }

    public void UpdateInventorySlot(ItemData data, int amount)
    {
        itemData = data;
        stackSize = amount;
    }

    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = itemData.MaxStackSize - stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    public bool RoomLeftInStack(int amountToAdd)
    {
        if (stackSize + amountToAdd <= itemData.MaxStackSize)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddToStack(int amount) // Adds x amount of items to the stack
    {
        stackSize += amount;
    }

    public void RemoveFromStack(int amount) // Removes x amount of items from the stack
    {
        stackSize -= amount;
    }
}
