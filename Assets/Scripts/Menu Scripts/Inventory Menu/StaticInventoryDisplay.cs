using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay //
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlot_UI[] slots;
    [SerializeField] ItemSelector selector;

    private float verticalInput;
    private int selectedSlot;

    private string currentText;

    private bool activeCoroutine;
    private bool slotChosen;

    public InventorySlot_UI SelectedInventorySlot => slots[selectedSlot];
    public string CurrentText => currentText;

    protected override void Start() // Fix my other inheritnece to work like this
    {
        base.Start();
        if(inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot; // Subscribes an action to an event

        }
        else
        {
            Debug.LogWarning($"No inventory assigned to {this.gameObject}");
        }

        AssignSlot(inventorySystem);
        selectedSlot = 0;
    }

    private void OnEnable()
    {
        if (!slots[selectedSlot].CheckEmpty())      // On enable, the selected slot will always be reset to the first one
        {
            slots[selectedSlot].Selected = false;
            slots[selectedSlot].UpdateUISlot();
        }

        selectedSlot = 0;
        activeCoroutine = false;
        slotChosen = false;

        if (!slots[selectedSlot].CheckEmpty())  
        {
            slots[selectedSlot].UpdateUISlot();
            currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
        }

        

    }

    private void Update()
    {
        if (!activeCoroutine && !slots[0].CheckEmpty()) // If there isn't an active coroutine and if we have at least 1 item
        {
            selector.gameObject.SetActive(true);

            if (!slotChosen)    // If we haven't chosen a slot yet
            {
               // currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
                if (Input.GetButtonDown("Interact")) 
                {
                    selector.SelectorSwap();
                    slots[selectedSlot].Selected = true;
                    slots[selectedSlot].UpdateUISlot();

                    slotChosen = true;
                }

                /*verticalInput = Input.GetAxis("Vertical");
                if (verticalInput > 0)
                {
                    StartCoroutine(DoMoveUp());
                }
                else if (verticalInput < 0)
                {
                    StartCoroutine(DoMoveDown());
                }*/
                if (Input.GetButtonDown("Inventory Up")) 
                {
                    StartCoroutine(DoMoveUp());
                }
                else if (Input.GetButtonDown("Inventory Down"))
                {
                    StartCoroutine(DoMoveDown());
                }
                
            }

            else // If we have selected an item
            {
                if (Input.GetButtonDown("Interact")) // If we choose to use that item
                {
                    currentText = slots[selectedSlot].AssignedInventorySlot.Data.UseDescription;
                    if (slots[selectedSlot].AssignedInventorySlot.Data.Consumable) // If the item is consumable, we want to (a) carry out its effects and (b) remove it from the inventory
                    {
                        // (a) carry out effects
                        PlayerManager.Instance.PlayerStats().SetHP(slots[selectedSlot].AssignedInventorySlot.Data.HPRestore);
                        PlayerManager.Instance.PlayerStats().SetMP(slots[selectedSlot].AssignedInventorySlot.Data.MPRestore);

                        // (b) remove from inventory
                        inventorySystem.RemoveFromInventory(slots[selectedSlot].AssignedInventorySlot.Data, 1, selectedSlot);
                        UpdateSlotsBelow(selectedSlot);
                    }

                    selector.SelectorSwap();
                    slots[selectedSlot].Selected = false;
                    slots[selectedSlot].UpdateUISlot();
                    slotChosen = false;

                }
                else if (Input.GetButtonDown("Return"))
                {
                    selector.SelectorSwap();                // Back out of the selection sub-menu
                    slots[selectedSlot].Selected = false;
                    slots[selectedSlot].UpdateUISlot();
                    slotChosen = false;
                }
            }
            
        }
        
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if(slots.Length != inventorySystem.InventorySize)
        {
            Debug.Log($"Inventory slots out of sync on {this.gameObject}");
        }

        for(int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }

    private int GetFullSlotCount()
    {
        int count = 0;
        while (!slots[count].CheckEmpty())
        {
            count++;
        }
        return count;
    }

    private void UpdateSlotsBelow(int index)   
    {
        for(int i = index; i < slots.Length; i++)
        {
            slots[i].UpdateUISlot();
        }
    }

    // Coroutines ----------------------------------

    IEnumerator DoMoveUp()
    {
        activeCoroutine = true;
        //slots[selectedSlot].Selected = false;
        //slots[selectedSlot].UpdateUISlot();
        if (selectedSlot == 0)
        {   
            selectedSlot = GetFullSlotCount() - 1;
        }
        else
        {
            selectedSlot = selectedSlot - 1;
        }

        //slots[selectedSlot].Selected = true;
        // slots[selectedSlot].UpdateUISlot();
        currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
        //yield return new WaitForSeconds(.5f);
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoMoveDown()
    {
        activeCoroutine = true;
        //slots[selectedSlot].Selected = false;
        //slots[selectedSlot].UpdateUISlot();
        if (selectedSlot == GetFullSlotCount() - 1)
        {
           // Debug.Log(GetFullSlotCount());
            selectedSlot = 0;
        }
        else
        {
            selectedSlot = selectedSlot + 1;
        }

        currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
        // slots[selectedSlot].Selected = true;
        //slots[selectedSlot].UpdateUISlot();
        //yield return new WaitForSeconds(.5f);
        activeCoroutine = false;
        yield return null;
    }

    
}
