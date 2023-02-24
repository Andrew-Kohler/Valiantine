/*
Player Menu Toggle
Used on:    Player
For:    Allows the player to open and close relevant in-game menus
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuToggle : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Inventory") )    // If the player presses Q, it should toggle the inventory
        {
            if (!GameManager.Instance.isInventory() && GameManager.Instance.canMove())
            {
                GameManager.Instance.Inventory(true);
            }
            else
            {
                GameManager.Instance.Inventory(false);
            }
        }
        else if (Input.GetButtonDown("Settings / Back") && !GameManager.Instance.isTransition()) // If the player presses Esc, it should toggle the settings
        {
            if (!GameManager.Instance.isSettings())
            {
                GameManager.Instance.Settings(true);
            }
            else
            {
                GameManager.Instance.Settings(false);
            }
        }
    }
}
