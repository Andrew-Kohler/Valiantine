using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInventoryDisplay : MonoBehaviour
{
    [SerializeField] private List<GameObject> gemDisplays;   // A list of all gem displays for easy access
    [SerializeField] private GemSystem gemSystem;
    [SerializeField] private StatDisplay statDisplay;

    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject tabIndicator;


    [SerializeField] float yOffset = 10f;

    bool activeCoroutine;
    bool showSpell;
    bool gemChosen;
    string currentText;
    public string CurrentText => currentText;
    int selectedSlot;
    int equippedSlot;

    // Current order in these lists: 
    // Will (starter)
    // Courage
    // Patience
    // Constitution
    // Cunning
    // Great Patience
    // Heart

    public delegate void OnSelectedGemChange();
    public static event OnSelectedGemChange onSelectedGemChange;

    private void OnEnable()
    {
        for(int i = 0; i < gemSystem.HeldGemList.Length; i++)
        {
            if(gemSystem.HeldGemList[i] != null)
            {
                gemDisplays[i].GetComponent<GemDisplay>().showGem();
            }
        }
        selectedSlot = 0;
        showSpell = false;
        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();
        onSelectedGemChange?.Invoke();
    }

    private void Start()
    {
        for (int i = 0; i < gemSystem.HeldGemList.Length; i++)
        {
            if (gemSystem.HeldGemList[i] != null)
            {
                gemDisplays[i].GetComponent<GemDisplay>().showGem();
            }
        }
        selectedSlot = 0;
        equippedSlot = -1;
        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();
    }

    private void Update()
    {
        if (!activeCoroutine)
        {
            if (!gemChosen)
            {
                if (Input.GetButtonDown("Inventory Left"))  // Move the selection arrow left
                {
                    StartCoroutine(DoMoveLeft());
                    onSelectedGemChange?.Invoke();
                }
                else if (Input.GetButtonDown("Inventory Right")) // Move the selection arrow right
                {
                    StartCoroutine(DoMoveRight());
                    onSelectedGemChange?.Invoke();
                }
                else if (Input.GetButtonDown("Inventory Up") && showSpell && gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)  // Change descriptive text
                {
                    showSpell = false;
                    UpdateText();
                    tabIndicator.transform.Rotate(new Vector3(0, 0, 180));
                }
                else if (Input.GetButtonDown("Inventory Down") && !showSpell && gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
                {
                    showSpell = true;
                    UpdateText();
                    tabIndicator.transform.Rotate(new Vector3(0, 0, 180));
                }
                else if (Input.GetButtonDown("Interact"))   // Start the thing where we ask if they player wants to equip the gem
                {
                    if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld && !gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemEquipped)
                    {
                        gemChosen = true;
                        // Change the indicator to a "Equip?" prompt similar to the one in the other inventory
                    }
                    else
                    {
                        // In the far, far future, this will make a sound play.

                        // So, MP is still broken, but it looks like that's it.
                    }


                }

            }
            else    // If we have selected a gem
            {
                if (Input.GetButtonDown("Interact")) // If we choose to equip that gem
                { 
                    if(equippedSlot != -1) // TODO: Find a way to give the player the Gem of Will and have it equipped already when the game begins
                    {
                        gemDisplays[equippedSlot].GetComponent<GemDisplay>().equipGem(false); // Turn off the outline on the old selected one
                    }
                    gemSystem.equipGem(selectedSlot);                                       // Alert the backend gem system that a change has been made
                    gemDisplays[selectedSlot].GetComponent<GemDisplay>().equipGem(true);    // Turn on the outline on the new selected one
                    equippedSlot = selectedSlot;   
                    UpdateStatDisplay();                                                    // Alert the stat display that a change has been made

                    // Alert the itty bitty gem that a change has been made
                    // Revert the changes made to the indicator

                    gemChosen = false;

                }
                else if (Input.GetButtonDown("Return"))
                {
                    // Revert the changes made to the indicator
                    gemChosen = false;
                }
            }
            
        }
        
    }

    // Methods ----------------------------------

    private void UpdateText()
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
            tabIndicator.SetActive(true);
            if (showSpell)
            {
                currentText = gemSystem.HeldGemList[selectedSlot].UseDescription;
            }
            else
            {
                currentText = gemSystem.HeldGemList[selectedSlot].InventoryDescription;
            }
            
        }
        else
        {
            tabIndicator.SetActive(false);
            currentText = "Perhaps this blade holds secrets yet...";
        }
    }

    private void UpdatePointer()
    {
        Vector3 selectedPosition = gemDisplays[selectedSlot].transform.position;
        indicator.transform.position = new Vector3(selectedPosition.x, selectedPosition.y + yOffset, selectedPosition.z);
    }
    private void UpdateStatDisplay()    // Feeds the stat display the relevant data from making stat predictions
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
            statDisplay.SetRelevantGemStats(gemSystem.CurrentGem, gemSystem.GemStats[selectedSlot]);
        }
        else
        {
            statDisplay.SetRelevantGemStats(gemSystem.CurrentGem, gemSystem.CurrentGem);
        }
    }

    // Coroutines ----------------------------------

    IEnumerator DoMoveLeft()
    {
        activeCoroutine = true;
        if (selectedSlot == 0)
        {
            selectedSlot = 6;
        }
        else
        {
            selectedSlot = selectedSlot - 1;
        }

        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();

        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoMoveRight()
    {
        activeCoroutine = true;
        if (selectedSlot == 6)
        {
            selectedSlot = 0;
        }
        else
        {
            selectedSlot = selectedSlot + 1;
        }

        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();

        activeCoroutine = false;
        yield return null;
    }

    // Add a thing where you can hit up and down to look at the spell
}
