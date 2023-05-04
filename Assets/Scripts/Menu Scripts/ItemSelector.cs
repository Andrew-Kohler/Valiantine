using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] Image Arrow;
    [SerializeField] Image Backboard;
    [SerializeField] TextMeshProUGUI useText;

    [SerializeField] GameObject itemsTab;
    [SerializeField] float xOffset = 330;
    
    void Start()
    {
        Arrow.color = Color.white;
        Backboard.color = Color.clear;
        useText.text = "";
        SetPosition();
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetPosition();
        Arrow.color = Color.white;
        Backboard.color = Color.clear;
        useText.text = "";
    }


    void Update()
    {
        if(!itemsTab.GetComponent<StaticInventoryDisplay>().SelectedInventorySlot.CheckEmpty() && itemsTab.activeInHierarchy) // If we have at least one item
        {
            SetPosition();
        }
        else // If we don't, flip the off switch
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SelectorSwap()  // Allows selector visuals to be swapped from a different class (in this case, inv display)
    {
        if(useText.text == "")
        {
            Arrow.color = Color.clear;
            Backboard.color = Color.white;
            useText.text = "Use?";
        }
        else
        {
            Arrow.color = Color.white;
            Backboard.color = Color.clear;
            useText.text = "";
        }
    }

    private void SetPosition()
    {
         Vector3 selectedPosition = itemsTab.GetComponent<StaticInventoryDisplay>().SelectedInventorySlot.transform.position;
         this.transform.position = new Vector3(selectedPosition.x - xOffset, selectedPosition.y, selectedPosition.z); 
    }

}
