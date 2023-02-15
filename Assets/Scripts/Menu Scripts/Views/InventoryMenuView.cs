using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuView : View
{
    bool exit;

   // Camera cam;
    //CameraFollow camController;

    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        //cam = Camera.main;
        //camController = cam.GetComponent<CameraFollow>();
    }


    private void Update()
    {
        if (GameManager.Instance.isInventory())
        {
            exit = true;
        }
        if (!GameManager.Instance.isInventory() && exit)
        {
            //camController.camReturnToPos();
            GetComponent<FadeUI>().UIFadeOut();
            exit = false;
        }

        if (GameManager.Instance.isSettings())
        {
            ViewManager.Show<SettingsMenuView>(true);
        }
    }
}
