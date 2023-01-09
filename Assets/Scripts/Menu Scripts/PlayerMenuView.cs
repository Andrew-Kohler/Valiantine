/*
Player Menu View
Used on:    GameObject
For:    Marks a game object as the player menu opened using "esc", and sets up that menu's basic contents
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuView : View
{
    [SerializeField] private Button playerInfoButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button settingsButton;

    [SerializeField] private GameObject playerInfoTab;
    [SerializeField] private GameObject inventoryTab;
    [SerializeField] private GameObject settingsTab;
    public override void Initialize()
    {
        playerInfoTab.SetActive(true);
        inventoryTab.SetActive(false);
        settingsTab.SetActive(false);

        playerInfoButton.onClick.AddListener(() => Switch(playerInfoTab));

        inventoryButton.onClick.AddListener(() => Switch(inventoryTab));

        settingsButton.onClick.AddListener(() => Switch(settingsTab));
    }

    private void Update()
    {
        if (!GameManager.Instance.isMenu()) // If esc is hit, go back out of the whole menu
        {
            Switch(playerInfoTab);
            ViewManager.ShowLast();
        }
    }

    private void Switch(GameObject desiredSubmenu)  // A method that will switch the active sub-menu of the player menu
    {
        desiredSubmenu.SetActive(true);
        if(playerInfoTab != desiredSubmenu)
        {
            playerInfoTab.SetActive(false);
        }
        if (inventoryTab != desiredSubmenu)
        {
            inventoryTab.SetActive(false);
        }
        if (settingsTab != desiredSubmenu)
        {
            settingsTab.SetActive(false);
        }
       
    }

}

