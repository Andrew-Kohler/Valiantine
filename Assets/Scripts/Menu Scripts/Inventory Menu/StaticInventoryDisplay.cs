using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay //
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlot_UI[] slots;
    //[SerializeField] ItemSelector selector;
    [SerializeField] GameObject selectArrow;
    InventoryArrow selectorArrow;

    [SerializeField] float arrowSpeed = .1f;
    float arrowXOffset = 330;

    private float verticalInput;
    private int selectedSlot;

    private string currentText;

    private bool activeCoroutine;
    private bool slotChosen;
    private bool pointerUpdated;

    public InventorySlot_UI SelectedInventorySlot => slots[selectedSlot];
    public string CurrentText => currentText;

    public delegate void OnItemUse();
    public static event OnItemUse onItemUse;

    public delegate void OnHealthPotDrink();
    public static event OnHealthPotDrink onHealthPotDrink;
    public delegate void OnManaPotDrink();
    public static event OnManaPotDrink onManaPotDrink;

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
        selectorArrow = selectArrow.GetComponent<InventoryArrow>();
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
        pointerUpdated = false;

        if (!slots[selectedSlot].CheckEmpty())  
        {
            slots[selectedSlot].UpdateUISlot();
            currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
        }
    }

    private void OnDisable()
    {
        selectArrow.SetActive(false);
    }

    private void Update()
    {       
        if (!activeCoroutine && !slots[0].CheckEmpty()) // If there isn't an active coroutine and if we have at least 1 item
        {
            selectArrow.SetActive(true);
            if (!pointerUpdated)
            {
                UpdatePointerFast();
                currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
                pointerUpdated = true;
            }

            //currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;

            if (!slotChosen)    // If we haven't chosen a slot yet
            {
               // currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
                if (Input.GetButtonDown("Interact")) 
                {
                    selectorArrow.selectorSwap();
                    slots[selectedSlot].Selected = true;
                    slots[selectedSlot].UpdateUISlot();

                    slotChosen = true;
                }

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
                    if (GameManager.Instance.isBattle())
                    {
                        currentText = slots[selectedSlot].AssignedInventorySlot.Data.BattleUseDescription;
                    }
                    else
                    {
                        currentText = slots[selectedSlot].AssignedInventorySlot.Data.UseDescription;
                    }
                    
                    if (slots[selectedSlot].AssignedInventorySlot.Data.Consumable) // If the item is consumable, we want to (a) carry out its effects and (b) remove it from the inventory
                    {
                        int multiplier = 1;
                        // (a) carry out effects
                        if (GameManager.Instance.isBattle())
                        {
                            if(BattleManager.Instance.PatienceCounter == 0)
                            {
                                multiplier *= 2;
                            }
                            if(BattleManager.Instance.GreatPatienceCounter == 0)
                            {
                                multiplier *= 3;
                            }
                        }

                        PlayerManager.Instance.PlayerStats().SetHP(slots[selectedSlot].AssignedInventorySlot.Data.HPRestore * multiplier, false);
                        Debug.Log(slots[selectedSlot].AssignedInventorySlot.Data.HPRestore * multiplier);
                        if (slots[selectedSlot].AssignedInventorySlot.Data.HPRestore > 0)
                        {
                            onHealthPotDrink?.Invoke();
                        }
                        PlayerManager.Instance.PlayerStats().SetMP(slots[selectedSlot].AssignedInventorySlot.Data.MPRestore * multiplier);
                        if (slots[selectedSlot].AssignedInventorySlot.Data.MPRestore > 0)
                        {
                            onManaPotDrink?.Invoke();
                        }

                        // (b) remove from inventory
                        inventorySystem.RemoveFromInventory(slots[selectedSlot].AssignedInventorySlot.Data, 1, selectedSlot);
                        UpdateSlotsBelow(selectedSlot);
                        if (slots[selectedSlot].CheckEmpty())   // Accounting for what happens if a slot empties out
                        {
                            if(selectedSlot == 0)
                            {
                                selectArrow.SetActive(false);
                            }
                            else
                            {
                                StartCoroutine(DoMoveUp());
                            }
                        }
                    }

                    selectorArrow.selectorSwap();
                    slots[selectedSlot].Selected = false;
                    slots[selectedSlot].UpdateUISlot();
                    slotChosen = false;

                    if (GameManager.Instance.isBattle())
                    {
                        onItemUse?.Invoke();
                        StartCoroutine(DoWait());
                    }

                }
                else if (Input.GetButtonDown("Return"))
                {
                    selectorArrow.selectorSwap();                // Back out of the selection sub-menu
                    slots[selectedSlot].Selected = false;
                    slots[selectedSlot].UpdateUISlot();
                    slotChosen = false;
                }
            }
            
        }
        else
        {
            //if(!GameManager.Instance.isBattle())
                selectArrow.SetActive(false);
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

    private void UpdatePointer()
    {
        Vector3 selectedPosition = SelectedInventorySlot.transform.position;
        selectorArrow.moveArrow(new Vector2(selectedPosition.x - arrowXOffset, selectedPosition.y), arrowSpeed, true);
    }

    private void UpdatePointerFast()
    {
        Vector3 selectedPosition = SelectedInventorySlot.transform.position;
        selectorArrow.moveArrow(new Vector2(selectedPosition.x - arrowXOffset, selectedPosition.y), .001f, false);
    }

    // Coroutines ----------------------------------

    IEnumerator DoMoveUp()
    {
        activeCoroutine = true;
        if (selectedSlot == 0)
        {   
            selectedSlot = GetFullSlotCount() - 1;
        }
        else
        {
            selectedSlot = selectedSlot - 1;
        }

        currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
        UpdatePointer();  // Call a movement on the arrow
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoMoveDown()
    {
        activeCoroutine = true;
        if (selectedSlot == GetFullSlotCount() - 1)
        {
            selectedSlot = 0;
        }
        else
        {
            selectedSlot = selectedSlot + 1;
        }

        currentText = slots[selectedSlot].AssignedInventorySlot.Data.InventoryDescription;
        UpdatePointer(); // Call a movement on the arrow
        activeCoroutine = false;
        yield return null;
    }
    IEnumerator DoWait()    // Exists to freeze this class until the inventory closes
    {
        activeCoroutine = true;
        yield return new WaitForSeconds(5f);
        activeCoroutine = false;
    }

    
}
