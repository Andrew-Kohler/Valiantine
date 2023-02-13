/*
Main Menu View
Used on:    GameObject
For:    Marks a game object as the main, opening menu of the game
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] private Button settingsButton;
    public override void Initialize()
    {
        settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsMenuView>(true));
    }
}
