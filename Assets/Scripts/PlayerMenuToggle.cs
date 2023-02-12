/*
Player Menu Toggle
Used on:    Player
For:    Allows the player to open and close the menu with "esc"
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuToggle : MonoBehaviour
{

    void Update()
    {
        if (Input.GetButtonDown("Menu"))    // If the player presses esc, it should toggle the menu on and off
        {
            //GameManager.Instance.Menu(!GameManager.Instance.isMenu());
        }
    }
}
