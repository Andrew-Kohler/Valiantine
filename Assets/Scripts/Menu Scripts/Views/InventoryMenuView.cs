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
        //throw new System.NotImplementedException();
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
            flavorText.text = "Wow. You don't have any items. Loser.";
        }
        else
        {
            flavorText.text = "Gemless + no sword + 0 mana + ratio";
        }
    }

}
