using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ATKNum;    // The text readouts of player ATK, DEF, and SPD values
    [SerializeField] TextMeshProUGUI DEFNum;
    [SerializeField] TextMeshProUGUI SPDNum;

    [SerializeField] TextMeshProUGUI ATKChange;    // The text readouts of player ATK, DEF, and SPD potential changes with gem equips
    [SerializeField] TextMeshProUGUI DEFChange;
    [SerializeField] TextMeshProUGUI SPDChange;

    [SerializeField] TextMeshProUGUI XPMult;
    [SerializeField] TextMeshProUGUI HPChange;
    [SerializeField] TextMeshProUGUI MPChange;

    [SerializeField] PlayerStats playerStats;

    GemStatBlock equippedGemStatBlock;  // Used for calculations between 
    GemStatBlock selectedGemStatBlock;

    private void OnEnable()
    {
        GemInventoryDisplay.onSelectedGemChange += UpdateStatPreviews;
        // Also in here, we need an event for when a gem is changed to actually update stats
    }

    private void OnDisable()
    {
        GemInventoryDisplay.onSelectedGemChange -= UpdateStatPreviews;
    }

    private void Start()
    {
        //playerStats = PlayerManager.Instance.PlayerStats();
        UpdateStats();
        equippedGemStatBlock = null;
        selectedGemStatBlock = null;

        
    }

    // Methods
    public void SetRelevantGemStats(GemStatBlock equipped, GemStatBlock selected)
    {
        equippedGemStatBlock = equipped;
        selectedGemStatBlock = selected;
        UpdateStats();
    }

    private void UpdateStats()  // For when a gem is equipped and the stats actually change
    {
        ATKNum.text = playerStats.GetATK().ToString();
        DEFNum.text = playerStats.GetDEF().ToString();
        SPDNum.text = playerStats.GetSPD().ToString();
        UpdateStatPreviews();
    }

    private void UpdateStatPreviews() // For when the player is navigating the gem inventory to give a quick preview of potential stat changes
    {
        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetATKRaw() * equippedGemStatBlock.ATKMod == playerStats.GetATKRaw() * selectedGemStatBlock.ATKMod)   // ATK
        {
            ATKChange.text = "";
        }
        else if (playerStats.GetATKRaw() * equippedGemStatBlock.ATKMod >= playerStats.GetATKRaw() * selectedGemStatBlock.ATKMod)
        {
            ATKChange.text = "(-" + (playerStats.GetATKRaw() * equippedGemStatBlock.ATKMod - playerStats.GetATKRaw() * selectedGemStatBlock.ATKMod) + ")";
            ATKChange.color = Color.red;
        }
        else if (playerStats.GetATK() * equippedGemStatBlock.ATKMod <= playerStats.GetATKRaw() * selectedGemStatBlock.ATKMod)
        {
            ATKChange.text = "(+" + (playerStats.GetATKRaw() * selectedGemStatBlock.ATKMod - playerStats.GetATKRaw() * equippedGemStatBlock.ATKMod) + ")";
            ATKChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetDEFRaw() * equippedGemStatBlock.DEFMod == playerStats.GetDEFRaw() * selectedGemStatBlock.DEFMod)   // DEF
        {
            DEFChange.text = "";
        }
        else if (playerStats.GetDEFRaw() * equippedGemStatBlock.DEFMod >= playerStats.GetDEFRaw() * selectedGemStatBlock.DEFMod)
        {
            DEFChange.text = "(-" + (playerStats.GetDEFRaw() * equippedGemStatBlock.DEFMod - playerStats.GetDEFRaw() * selectedGemStatBlock.DEFMod) + ")";
            DEFChange.color = Color.red;
        }
        else if (playerStats.GetDEF() * equippedGemStatBlock.DEFMod <= playerStats.GetDEFRaw() * selectedGemStatBlock.DEFMod)
        {
            DEFChange.text = "(+" + (playerStats.GetDEFRaw() * selectedGemStatBlock.DEFMod - playerStats.GetDEFRaw() * equippedGemStatBlock.DEFMod) + ")";
            DEFChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetSPDRaw() * equippedGemStatBlock.SPDMod == playerStats.GetSPDRaw() * selectedGemStatBlock.SPDMod)   // SPD
        {
            SPDChange.text = "";
        }
        else if (playerStats.GetSPDRaw() * equippedGemStatBlock.SPDMod >= playerStats.GetSPDRaw() * selectedGemStatBlock.SPDMod)
        {
            SPDChange.text = "(-" + ((int)(playerStats.GetSPDRaw() * equippedGemStatBlock.SPDMod) - (int)(playerStats.GetSPDRaw() * selectedGemStatBlock.SPDMod)) + ")";
            SPDChange.color = Color.red;
            
        }
        else if (playerStats.GetSPDRaw() * equippedGemStatBlock.SPDMod <= playerStats.GetSPDRaw() * selectedGemStatBlock.SPDMod)
        {
            SPDChange.text = "(+" + ((int)(playerStats.GetSPDRaw() * selectedGemStatBlock.SPDMod) - (int)(playerStats.GetSPDRaw() * equippedGemStatBlock.SPDMod)) + ")";
            SPDChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetMaxHPRaw() * equippedGemStatBlock.HPMod == playerStats.GetMaxHPRaw() * selectedGemStatBlock.HPMod)   // HP
        {
            HPChange.text = "";
        }
        else if (playerStats.GetMaxHPRaw() * equippedGemStatBlock.HPMod >= playerStats.GetMaxHPRaw() * selectedGemStatBlock.HPMod)
        {
            HPChange.text = "(-" + (int)(playerStats.GetMaxHPRaw() * equippedGemStatBlock.HPMod - playerStats.GetMaxHPRaw() * selectedGemStatBlock.HPMod) + ")";
            HPChange.color = Color.red;

        }
        else if (playerStats.GetMaxHPRaw() * equippedGemStatBlock.HPMod <= playerStats.GetMaxHPRaw() * selectedGemStatBlock.HPMod)
        {
            HPChange.text = "(+" + (int)(playerStats.GetMaxHPRaw() * selectedGemStatBlock.HPMod - playerStats.GetMaxHPRaw() * equippedGemStatBlock.HPMod) + ")";
            HPChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetMaxMPRaw() * equippedGemStatBlock.MPMod == playerStats.GetMaxMPRaw() * selectedGemStatBlock.MPMod)   // MP
        {
            MPChange.text = "";
        }
        else if (playerStats.GetMaxMPRaw() * equippedGemStatBlock.MPMod >= playerStats.GetMaxMPRaw() * selectedGemStatBlock.MPMod)
        {
            MPChange.text = "(-" + ((int)(playerStats.GetMaxMPRaw() * equippedGemStatBlock.MPMod) - (int)(playerStats.GetMaxMPRaw() * selectedGemStatBlock.MPMod)) + ")";
            MPChange.color = Color.red;

        }
        else if (playerStats.GetMaxMPRaw() * equippedGemStatBlock.MPMod <= playerStats.GetMaxMPRaw() * selectedGemStatBlock.MPMod)
        {
            MPChange.text = "(+" + ((int)(playerStats.GetMaxMPRaw() * selectedGemStatBlock.MPMod) - (int)(playerStats.GetMaxMPRaw() * equippedGemStatBlock.MPMod)) + ")";
            MPChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null)   // XP
        {
            XPMult.text = "";
        }
        else if (equippedGemStatBlock.XPMod > selectedGemStatBlock.XPMod)
        {
            XPMult.text = "x" + selectedGemStatBlock.XPMod + " XP gain";
            XPMult.color = Color.red;

        }
        else if (equippedGemStatBlock.XPMod < selectedGemStatBlock.XPMod)
        {
            XPMult.text = "x" + selectedGemStatBlock.XPMod + " XP gain";
            XPMult.color = Color.green;
        }
        else if (equippedGemStatBlock.XPMod == selectedGemStatBlock.XPMod)
        {
            XPMult.color = Color.white;
            if(equippedGemStatBlock.XPMod == 1)
            {
                XPMult.text = "";
            }
            else
            {
                XPMult.text = "x" + equippedGemStatBlock.XPMod + " XP gain";
            }
        }



    }
}
