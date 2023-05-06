using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemInventoryDisplay : MonoBehaviour
{
    [SerializeField] private List<GameObject> gemDisplays;   // A list of all gem displays for easy access
    [SerializeField] private List<Sprite> ittyBitty;   // All the itty bitty gem assets
    [SerializeField] private Image currentIttyBitty;    // The currently enshrined itty bitty
    [SerializeField] private Image sword;
    [SerializeField] private GemSystem gemSystem;
    [SerializeField] private StatDisplay statDisplay;

    [SerializeField] private GameObject indicator;          // The little arrow that shows what gem you have selected
    [SerializeField] private GameObject equipTag;          // The little arrow that shows what gem you have selected
    [SerializeField] private GameObject textTabIndicator;   // The little arrow that shows which way to scroll in the text box


    [SerializeField] float yOffset = 10f;
    float waitReset = 7.0f;
    float waitTime;

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
        waitTime = waitReset;
        gemChosen = false;
        showSpell = false;
        equipTag.SetActive(false);
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
        currentIttyBitty.sprite = ittyBitty[0];
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
                    textTabIndicator.transform.Rotate(new Vector3(0, 0, 180));
                    waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time
                }
                else if (Input.GetButtonDown("Inventory Down") && !showSpell && gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
                {
                    showSpell = true;
                    UpdateText();
                    textTabIndicator.transform.Rotate(new Vector3(0, 0, 180));
                    waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time
                }
                else if (Input.GetButtonDown("Interact"))   // Start the thing where we ask if they player wants to equip the gem
                {
                    if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld && !gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemEquipped)
                    {
                        gemChosen = true;
                        equipTag.SetActive(true);
                        // Change the indicator to a "Equip?" prompt similar to the one in the other inventory
                    }
                    else
                    {
                        // In the far, far future, this will make a sound play.
                    }


                }

            }
            else    // If we have selected a gem
            {
                waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time
                if (Input.GetButtonDown("Interact")) // If we choose to equip that gem
                {
                    if (equippedSlot != -1) // TODO: Find a way to give the player the Gem of Will and have it equipped already when the game begins
                    {
                        gemDisplays[equippedSlot].GetComponent<GemDisplay>().equipGem(false); // Turn off the outline on the old selected one
                    }
                    gemSystem.equipGem(selectedSlot);                                       // Alert the backend gem system that a change has been made
                    gemDisplays[selectedSlot].GetComponent<GemDisplay>().equipGem(true);    // Turn on the outline on the new selected one
                    equippedSlot = selectedSlot;   
                    UpdateStatDisplay();    // Alert the stat display that a change has been made
                    currentIttyBitty.sprite = ittyBitty[equippedSlot];      // Alert the itty bitty gem that a change has been made

                    // Revert the changes made to the indicator
                    equipTag.SetActive(false);
                    gemChosen = false;

                }
                else if (Input.GetButtonDown("Return"))
                {
                    // Revert the changes made to the indicator
                    equipTag.SetActive(false);
                    gemChosen = false;
                }
            }

            // For activating the shine
            waitTime -= Time.deltaTime;
            if(waitTime <= 0)
            {
                StartCoroutine(DoShine());
            }
        }
        
    }

    // Methods ----------------------------------

    private void UpdateText()
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
            textTabIndicator.SetActive(true);
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
            textTabIndicator.SetActive(false);
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
        waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time

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
        waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time

        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoShine()
    {
        waitTime = waitReset; // Reset the timer
        for (int i = 0; i < 7; i++)
        {
            if (gemDisplays[i].GetComponent<GemDisplay>().GemHeld)
            {
                gemDisplays[i].GetComponent<GemDisplay>().shineGem();
            }
            yield return new WaitForSeconds(.16f);
        }
        sword.GetComponent<SwordDisplay>().shineSword();
        waitTime = waitReset; // Reset the timer
        yield return null;
    }

}
