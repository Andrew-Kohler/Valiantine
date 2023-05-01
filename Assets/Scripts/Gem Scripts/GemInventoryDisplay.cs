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
    string currentText;
    public string CurrentText => currentText;
    int selectedSlot;

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
        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();
    }

    private void Update()
    {
        if (!activeCoroutine)
        {
            if (Input.GetButtonDown("Inventory Left"))
            {
                StartCoroutine(DoMoveLeft());
                onSelectedGemChange?.Invoke();
            }
            else if (Input.GetButtonDown("Inventory Right"))
            {
                StartCoroutine(DoMoveRight());
                onSelectedGemChange?.Invoke();
            }
            else if(Input.GetButtonDown("Inventory Up") && showSpell)
            {
                showSpell = false;
                UpdateText();
                tabIndicator.transform.Rotate(new Vector3(0, 0, 180));
            }
            else if (Input.GetButtonDown("Inventory Down") && !showSpell)
            {
                showSpell = true;
                UpdateText();
                tabIndicator.transform.Rotate(new Vector3(0, 0, 180));
            }
        }
        
    }

    // Methods ----------------------------------

    private void UpdateText()
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
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
            currentText = "Perhaps this blade holds secrets yet...";
        }
    }

    private void UpdatePointer()
    {
        Vector3 selectedPosition = gemDisplays[selectedSlot].transform.position;
        indicator.transform.position = new Vector3(selectedPosition.x, selectedPosition.y + yOffset, selectedPosition.z);
    }
    private void UpdateStatDisplay()
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
