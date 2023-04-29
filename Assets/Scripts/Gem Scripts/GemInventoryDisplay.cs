using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInventoryDisplay : MonoBehaviour
{
    [SerializeField] private List<GameObject> gemDisplays;   // A list of all gem displays for easy access
    [SerializeField] private GemSystem gemSystem;
    [SerializeField] private GameObject indicator;
    [SerializeField] float yOffset = 10f;

    bool activeCoroutine;
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

    private void OnEnable()
    {
        for(int i = 0; i < gemSystem.HeldGemList.Length; i++)
        {
            if(gemSystem.HeldGemList[i] != null)
            {
                Debug.Log("Gem shown");
                gemDisplays[i].GetComponent<GemDisplay>().showGem();
            }
        }
        selectedSlot = 0;
        UpdateText();
        UpdatePointer();
    }

    private void Start()
    {
        Debug.Log("Start has happened");
        for (int i = 0; i < gemSystem.HeldGemList.Length; i++)
        {
            if (gemSystem.HeldGemList[i] != null)
            {
                Debug.Log("Gem shown");
                gemDisplays[i].GetComponent<GemDisplay>().showGem();
            }
        }
        selectedSlot = 0;
        UpdateText();
        UpdatePointer();
    }

    private void Update()
    {
        if (!activeCoroutine)
        {
            if (Input.GetButtonDown("Inventory Left"))
            {
                StartCoroutine(DoMoveLeft());
            }
            else if (Input.GetButtonDown("Inventory Right"))
            {
                StartCoroutine(DoMoveRight());
            }
        }
        
    }

    // Methods ----------------------------------

    private void UpdateText()
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
            currentText = gemSystem.HeldGemList[selectedSlot].InventoryDescription;
        }
        else
        {
            currentText = "Perhaps this blade holds secrets yet.";
        }
    }

    private void UpdatePointer()
    {
        Vector3 selectedPosition = gemDisplays[selectedSlot].transform.position;
        indicator.transform.position = new Vector3(selectedPosition.x, selectedPosition.y + yOffset, selectedPosition.z);
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

        activeCoroutine = false;
        yield return null;
    }

    // Add a thing where you can hit up and down to look at the spell
}
