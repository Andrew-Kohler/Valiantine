using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuView : View
{
    public override void Initialize()
    {
        //throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (!GameManager.Instance.isInventory())
        {
            ViewManager.ShowLast();
        }
    }
}
