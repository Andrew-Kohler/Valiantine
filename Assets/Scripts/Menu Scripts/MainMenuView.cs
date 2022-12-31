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
