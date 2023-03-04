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

    private bool activeCoroutine;
    private bool slotChosen;

    public InventorySlot_UI SelectedInventorySlot => slots[selectedSlot];

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
        //slots[selectedSlot].Selected = true;
        //slots[selectedSlot].UpdateUISlot();
        activeCoroutine = false;
        slotChosen = false;
    }

    private void Update()
    {
        if (!activeCoroutine && !slots[0].CheckEmpty()) // If there isn't an active coroutine and if we have at least 1 item
        {
            selector.gameObject.SetActive(true);

            if (!slotChosen)    // If we haven't chosen a slot yet
            {
                if (Input.GetButtonDown("Interact"))
                {
                    selector.SelectorSwap();
                    slots[selectedSlot].Selected = true;
                    slots[selectedSlot].UpdateUISlot();

                    slotChosen = true;
                }

                verticalInput = Input.GetAxis("Vertical");
                if (verticalInput > 0)
                {
                    StartCoroutine(DoMoveUp());
                }
                else if (verticalInput < 0)
                {
                    StartCoroutine(DoMoveDown());
                }
            }
            else
            {
                if (Input.GetButtonDown("Interact"))
                {
                    // Ok, and now I actually have to use the item. TBH, I'd rather just do this right the first time, so we can 
                    // call it tonight and do the consumable / nonconsumable split tomorrow

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
        yield return new WaitForSeconds(.5f);
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
       // slots[selectedSlot].Selected = true;
        //slots[selectedSlot].UpdateUISlot();
        yield return new WaitForSeconds(.5f);
        activeCoroutine = false;
        yield return null;
    }

    
}
