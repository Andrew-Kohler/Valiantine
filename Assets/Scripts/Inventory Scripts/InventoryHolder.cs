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

}
