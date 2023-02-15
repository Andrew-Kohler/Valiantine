using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuView : View
{
    [SerializeField] private GameObject itemsTab;
    [SerializeField] private GameObject gemsTab;
    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        itemsTab.SetActive(true);
        gemsTab.SetActive(false);
    }

    private void Update()
    {
        
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
}
