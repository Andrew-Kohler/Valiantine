/*
Settings Menu View
Used on:    GameObject
For:    Marks a game object as the main menu version of the game's settings menu
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuView : View
{
    //[SerializeField] private Button backButton;
    public override void Initialize()
    {
        //backButton.onClick.AddListener(() => ViewManager.ShowLast());
    }

    private void Update()
    {
        if (!GameManager.Instance.isSettings())
        {
            ViewManager.ShowLast();
        }
    }
}
