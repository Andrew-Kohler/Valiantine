/*
In-Game UI View
Used on:    GameObject
For:    Marks a game object as the in-game UI that's up while the player is moving around
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIView : View
{
   // Camera cam;
    //CameraFollow camController;
    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        //cam = Camera.main;
        //camController = cam.GetComponent<CameraFollow>();
    }

    void Update()
    {
        if (GameManager.Instance.isInventory())
        {
            ViewManager.ShowFade<InventoryMenuView>(true);
            //camController.setCamInventory();
        }
        else if (GameManager.Instance.isSettings())
        {
            ViewManager.Show<SettingsMenuView>(true);
        }
    }
}
