using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryMenuView : View
{
    [SerializeField] private GameObject itemsTab;   // The two tabs that contain all of their tab's things
    [SerializeField] private GameObject gemsTab;

    [SerializeField] GameObject itemsTabIndicator;  // The two tab icons
    [SerializeField] GameObject gemsTabIndicator;
    [SerializeField] GameObject selectedTabIndicator;   // The line under them
    SelectedTabIndicator selectionLine;

    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject manaBar;
    [SerializeField] GameObject XPIndicator;

    //[SerializeField] GameObject player;
    PlayerStats playerStats;
    HealthBar healthBarUI;
    ManaBar manaBarUI;
    XPDisplay xpUI;

    [SerializeField] private float tabIndicatorStep = .1f;

    [SerializeField] TextMeshProUGUI flavorText;

    private bool fadeOut;

    public delegate void OnTabSwitch();
    public static event OnTabSwitch onTabSwitch;
    public override void Initialize()
    {
        itemsTab.SetActive(true);
        gemsTab.SetActive(false);

        playerStats = PlayerManager.Instance.PlayerStats();
        healthBarUI = healthBar.GetComponent<HealthBar>();
        manaBarUI = manaBar.GetComponent<ManaBar>();
        xpUI = XPIndicator.GetComponent<XPDisplay>();
    }

    private void OnEnable()
    {
        fadeOut = false;
    }

    private void Start()
    {
        selectionLine = selectedTabIndicator.GetComponent<SelectedTabIndicator>();
    }

    private void Update()
    {
        UpdateText();
        updateHPMPXP();
        if (Input.GetButtonDown("Inv.Tab 1"))
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
        if (!GameManager.Instance.isInventory())    // If we close the inventory
        {
            if (!fadeOut) // Only calls this once so we don't go ZOOOOMM
            {
                Debug.Log("Left foot out");
                GetComponent<FadeUI>().UIFadeOut(); 
                fadeOut = true; 
            }
    
        }
        if (GameManager.Instance.isBattle())    // If we are in battle
        {
            BattleManager.MenuStatus status = BattleManager.Instance.GetPlayerStatus();
            if (status == BattleManager.MenuStatus.Inventory)
            {
                GameManager.Instance.Inventory(true);
                if (Input.GetButtonDown("Inventory"))
                {
                    ViewManager.ShowLast();
                }
            }
            else
            {
                GameManager.Instance.Inventory(false);// The game manager no longer indicates that we are in the inventory 
            }
              
            // Problem: Since we are in battle, the inventory is perpetually going to fade itself out
        }
    }

    private void Switch(GameObject desiredSubmenu)  // A method that will switch the active sub-menu of the player menu
    {
        desiredSubmenu.SetActive(true);
        if (itemsTab == desiredSubmenu)
        {
            gemsTab.SetActive(false);
            selectionLine.moveLine(new Vector2(itemsTabIndicator.transform.position.x, selectedTabIndicator.transform.position.y), tabIndicatorStep);
        }
        if (gemsTab == desiredSubmenu)
        {
            itemsTab.SetActive(false);
            selectionLine.moveLine(new Vector2(gemsTabIndicator.transform.position.x, selectedTabIndicator.transform.position.y), tabIndicatorStep);
        }
        onTabSwitch?.Invoke();
    }

    private void UpdateText()
    {
        if (itemsTab.activeSelf)
        {
            if (!itemsTab.GetComponent<StaticInventoryDisplay>().SelectedInventorySlot.CheckEmpty())
            {
                flavorText.text = itemsTab.GetComponent<StaticInventoryDisplay>().CurrentText;
            }
            else
            {
                flavorText.text = "Courage is not an item to be carried; it lies within you.";
            }
        }
        else
        {
            flavorText.text = gemsTab.GetComponent<GemInventoryDisplay>().CurrentText;
        }
    }

    void updateHPMPXP()
    {
        healthBarUI.SetHealth(playerStats.GetHP());
        healthBarUI.SetMaxHealth(playerStats.GetMaxHP());

        manaBarUI.SetMana(playerStats.GetMP());
        manaBarUI.SetMaxMana(playerStats.GetMaxMP());

        xpUI.SetXP(playerStats.GetXP(), playerStats.GetXPThreshold());
        xpUI.SetLVL(playerStats.GetLVL());
    }

}
