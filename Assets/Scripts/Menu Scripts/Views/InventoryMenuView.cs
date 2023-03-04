using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryMenuView : View
{
    [SerializeField] private GameObject itemsTab;
    [SerializeField] private GameObject gemsTab;

    [SerializeField] TextMeshProUGUI flavorText;
    public override void Initialize()
    {
        itemsTab.SetActive(true);
        gemsTab.SetActive(false);
    }

    private void Update()
    {
        UpdateText();
        if(Input.GetButtonDown("Inv.Tab 1"))
        {
            Switch(itemsTab);
        }
        if (Input.GetButtonDown("Inv.Tab 2"))
        {
            Switch(gemsTab);
        }

        if (GameManager.Instance.isSettings())  // Considerations made for changing to other views
        {
            ViewManager.Show<SettingsMenuView>(true);
        }
        if (!GameManager.Instance.isInventory())
        {
            GetComponent<FadeUI>().UIFadeOut();
        }
        if (GameManager.Instance.isBattle())
        {
            GameManager.Instance.Inventory(false);
        }
    }

    private void Switch(GameObject desiredSubmenu)  // A method that will switch the active sub-menu of the player menu
    {
        desiredSubmenu.SetActive(true);
        if (itemsTab != desiredSubmenu)
        {
            itemsTab.SetActive(false);
        }
        if (gemsTab != desiredSubmenu)
        {
            gemsTab.SetActive(false);
        }
    }

    private void UpdateText()
    {
        if (itemsTab.activeSelf)
        {
            if (!itemsTab.GetComponent<StaticInventoryDisplay>().SelectedInventorySlot.CheckEmpty())
            {
                flavorText.text = itemsTab.GetComponent<StaticInventoryDisplay>().SelectedInventorySlot.AssignedInventorySlot.Data.InventoryDescription;
            }
            else
            {
                flavorText.text = "Courage is not an item to be carried; it lies within you.";
            }
        }
        else
        {
            flavorText.text = "Ok, wait, hold on, you don't have to implement this buddy, we can ta-";
        }
    }

}
