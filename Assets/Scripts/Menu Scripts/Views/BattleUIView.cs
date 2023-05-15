/*
Battle UI View
Used on:    GameObject
For:    Marks a game object as the in-game battle user interface
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUIView : View
{
    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] GameObject actionIndicators;
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject manaBar;
    //[SerializeField] GameObject player;

    // Things the inventory uses
    GemSystem gemSystem;

    // Other things
    IndicatorAction indicatorInfo;
    PlayerStats playerStats;
    HealthBar healthBarUI;
    ManaBar manaBarUI;
    
    public override void Initialize()
    {
        battleText.text = "";
        indicatorInfo = actionIndicators.GetComponent<IndicatorAction>();
        //playerStats = player.GetComponent<PlayerStats>();
        playerStats = PlayerManager.Instance.PlayerStats();
        healthBarUI = healthBar.GetComponent<HealthBar>();
        manaBarUI = manaBar.GetComponent<ManaBar>();

        gemSystem = PlayerManager.Instance.GemSystem();
    }

    private void Update()
    {
        // Seeing as there may be other uses for this loop in the future, I will refrain from flooding it with the code for 
        // what text should be displayed, and just make it a method instead.
        if (GameManager.Instance.isBattle())
        {
            updateText();
            updateHPandMP();

            // Alright gamers, tomorrow we enter the battlefield once again.
            // Ok, new plan, literally just switch the view over to inventory, why not
            // The complexity of retrofitting this class far outweigh the relative benefits of just...switching to what we have
            BattleManager.MenuStatus status = BattleManager.Instance.GetPlayerStatus();
            if(status == BattleManager.MenuStatus.Inventory)
            {
                GameManager.Instance.Inventory(true);
                ViewManager.ShowFade<InventoryMenuView>(true); // Show the inventory menu
            }


            if (GameManager.Instance.isSettings())  // Considerations made for changing to settings
            {
                ViewManager.Show<SettingsMenuView>(true);
            }
        }
        
    }

    void updateText() 
    {
        string currentBoxName = indicatorInfo.GetLeadBox();
        BattleManager.MenuStatus status = BattleManager.Instance.GetPlayerStatus();
        if (status == BattleManager.MenuStatus.Selecting)
        {
            if (currentBoxName == "ATK")
            {
                battleText.text = "Attack an enemy for 100% damage.";
            }
            else if (currentBoxName == "SPL")
            {
                battleText.text = "Cast your gem spell. Current Spell: " + gemSystem.CurrentGemText.UseDescription.Substring(6); 
            }
            else if (currentBoxName == "ITM")
            {
                battleText.text = "Take a turn to use an item or switch out your equiped gem.";
            }
            else if (currentBoxName == "RUN")
            {
                battleText.text = "Attempt to flee the battle.";
            }
        }
        else if (status == BattleManager.MenuStatus.Attack)
        {
            battleText.text = "Which enemy will you attack?";
        }
        else if (status == BattleManager.MenuStatus.Spell)
        {
            battleText.text = "This will be...complicated, and will need to function on a per-gem basis.";
        }
        else if (status == BattleManager.MenuStatus.Inventory)
        {
            battleText.text = "Use a health or mana potion to restore your strength.";
        }
        else if (status == BattleManager.MenuStatus.Run)
        {
            battleText.text = "You got away, but just wait until I add speed checks in, you yellow-bellied ninny.";
        }
        else if (status == BattleManager.MenuStatus.Inactive)
        {
            battleText.text = "Well, uh...it's the white square's turn. You got anything, bud? ...No? Well, take your time.";
        }
        else
        {
            battleText.text = "";
        }
    }

    void updateHPandMP()
    {
        healthBarUI.SetHealth(playerStats.GetHP());
        healthBarUI.SetMaxHealth(playerStats.GetMaxHP());

        manaBarUI.SetMana(playerStats.GetMP());
        manaBarUI.SetMaxMana(playerStats.GetMaxMP());
    }

    
}
