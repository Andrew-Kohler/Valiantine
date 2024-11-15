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
    [SerializeField] TextMeshProUGUI tutorialText;
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
        if (tutorialText)
        {
            tutorialText.text = "A & D to choose an action // E to select an action // Q to back out of an action";
        }
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
                GetComponent<FadeUI>().UIFadeOut();
                 // Show the inventory menu
            }


            if (GameManager.Instance.isSettings())  // Considerations made for changing to settings
            {
                ViewManager.Show<SettingsMenuView>(true);
            }
        }
        
    }

    public void setText(string text)
    {
        battleText.text = text;
    }

    public void setTutorialText(string text)
    {
        if (GameManager.Instance.tutorialText)
        {
            tutorialText.text = text;
        }
    }

    void updateText() 
    {
        // This was cute for a while, but I really think I need to shift it all over to a system where it gets passed from BatManager
        string currentBoxName = indicatorInfo.GetLeadBox();
        BattleManager.MenuStatus status = BattleManager.Instance.GetPlayerStatus();
        if (status == BattleManager.MenuStatus.Selecting)
        {
            if (GameManager.Instance.tutorialText)
                tutorialText.text = "A & D to choose an action // E to select an action // Q to back out of an action";
            else
                tutorialText.text = "";
            if (currentBoxName == "ATK")
            {
                battleText.text = "Attack an enemy for " + ((int)(((float)PlayerManager.Instance.PlayerStats().GetATK() / (float)PlayerManager.Instance.PlayerStats().GetATKRaw()) * 100)) +  "% of base attack.";
            }
            else if (currentBoxName == "SPL")
            {
                battleText.text = "Cast your gem spell. Current Spell: " + gemSystem.CurrentGemText.UseDescription.Substring(6); 
            }
            else if (currentBoxName == "ITM")
            {
                battleText.text = "Take a turn to use an item or switch out your equipped gem.";
            }
            else if (currentBoxName == "RUN")
            {
                battleText.text = "Attempt to flee the battle.";
            }
        }
        /*else if (status == BattleManager.MenuStatus.Attack)
        {
            battleText.text = "Which enemy will you attack?";
        }
        else if (status == BattleManager.MenuStatus.Spell)
        {
            battleText.text = "This will be...complicated, and will need to function on a per-gem basis.";
        }
        else if (status == BattleManager.MenuStatus.Inventory)
        {
            battleText.text = "Take a turn to use an item or switch out your equipped gem.";
        }
        else if (status == BattleManager.MenuStatus.Run)
        {
            battleText.text = "You got away, but just wait until I add speed checks in, you yellow-bellied ninny.";
        }*/
        /*else if (status == BattleManager.MenuStatus.Inactive)
        {
            battleText.text = BattleManager.Instance.GetCurrentTurnName() + " moves to attack!";
        }*/
        /*else
        {
            battleText.text = "";
        }*/
    }

    void updateHPandMP()
    {
        healthBarUI.SetHealth(playerStats.GetHP());
        healthBarUI.SetMaxHealth(playerStats.GetMaxHP());

        manaBarUI.SetMana(playerStats.GetMP());
        manaBarUI.SetMaxMana(playerStats.GetMaxMP());
    }

    
}
