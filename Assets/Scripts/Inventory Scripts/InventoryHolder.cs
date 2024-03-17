using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem inventorySystem;

    public InventorySystem InventorySystem => inventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

    private void Awake()
    {
        inventorySystem = new InventorySystem(inventorySize);
    }

    public void RefillInventory(InventorySystem newInventory)
    {
        for(int i = 0; i < newInventory.InventorySize; i++)
        {
            if (newInventory.InventorySlots[i].Data != null)
                InventorySystem.AddToInventory(newInventory.InventorySlots[i].Data, newInventory.InventorySlots[i].StackSize);
        }
    }

    public void RefillInventoryFromSaveData(GameManager.SavedInventoryContents newInventory)
    {
        for(int i = 0; i < newInventory.itemNames.Length; i++)
        {
            InventorySystem.AddToInventory(Resources.Load<ItemData>(newInventory.itemNames[i]), newInventory.itemCounts[i]);
        }
    }

    public ArrayList GetInventoryContents()
    {
        ArrayList contents = new ArrayList();
        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            if (inventorySystem.InventorySlots[i].Data != null)
                contents.Add(inventorySystem.InventorySlots[i].Data.name);
        }
        return contents;
    }

    public ArrayList GetInventoryStackSizes()
    {
        ArrayList contents = new ArrayList();
        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            if (inventorySystem.InventorySlots[i].Data != null)
                contents.Add(inventorySystem.InventorySlots[i].StackSize);
        }
        return contents;
    }

}
