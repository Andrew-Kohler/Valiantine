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

    PlayerStats playerStats;

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
        playerStats = PlayerManager.Instance.PlayerStats();
        UpdateStats();
        equippedGemStatBlock = null;
        selectedGemStatBlock = null;
        //UpdateStatPreviews();
    }

    // Methods
    public void SetRelevantGemStats(GemStatBlock equipped, GemStatBlock selected)
    {
        equippedGemStatBlock = equipped;
        selectedGemStatBlock = selected;
    }

    private void UpdateStats()  // For when a gem is equipped and the stats actually change
    {
        ATKNum.text = playerStats.GetATK().ToString();
        DEFNum.text = playerStats.GetDEF().ToString();
        SPDNum.text = playerStats.GetSPD().ToString();
    }

    private void UpdateStatPreviews() // For when the player is navigating the gem inventory to give a quick preview of potential stat changes
    {
        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetATK() * equippedGemStatBlock.ATKMod == playerStats.GetATK() * selectedGemStatBlock.ATKMod)   // ATK
        {
            ATKChange.text = "";
        }
        else if (playerStats.GetATK() * equippedGemStatBlock.ATKMod >= playerStats.GetATK() * selectedGemStatBlock.ATKMod)
        {
            ATKChange.text = "(-" + (playerStats.GetATK() * equippedGemStatBlock.ATKMod - playerStats.GetATK() * selectedGemStatBlock.ATKMod) + ")";
            ATKChange.color = Color.red;
        }
        else if (playerStats.GetATK() * equippedGemStatBlock.ATKMod <= playerStats.GetATK() * selectedGemStatBlock.ATKMod)
        {
            ATKChange.text = "(+" + (playerStats.GetATK() * selectedGemStatBlock.ATKMod - playerStats.GetATK() * equippedGemStatBlock.ATKMod) + ")";
            ATKChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetDEF() * equippedGemStatBlock.DEFMod == playerStats.GetDEF() * selectedGemStatBlock.DEFMod)   // DEF
        {
            DEFChange.text = "";
        }
        else if (playerStats.GetDEF() * equippedGemStatBlock.DEFMod >= playerStats.GetDEF() * selectedGemStatBlock.DEFMod)
        {
            DEFChange.text = "(-" + (playerStats.GetDEF() * equippedGemStatBlock.DEFMod - playerStats.GetDEF() * selectedGemStatBlock.DEFMod) + ")";
            DEFChange.color = Color.red;
        }
        else if (playerStats.GetDEF() * equippedGemStatBlock.DEFMod <= playerStats.GetDEF() * selectedGemStatBlock.DEFMod)
        {
            DEFChange.text = "(+" + (playerStats.GetDEF() * selectedGemStatBlock.DEFMod - playerStats.GetDEF() * equippedGemStatBlock.DEFMod) + ")";
            DEFChange.color = Color.green;
        }

        if (selectedGemStatBlock == null || equippedGemStatBlock == null || playerStats.GetSPD() * equippedGemStatBlock.SPDMod == playerStats.GetSPD() * selectedGemStatBlock.SPDMod)   // SPD
        {
            SPDChange.text = "";
        }
        else if (playerStats.GetSPD() * equippedGemStatBlock.SPDMod >= playerStats.GetSPD() * selectedGemStatBlock.SPDMod)
        {
            SPDChange.text = "(-" + (playerStats.GetSPD() * equippedGemStatBlock.SPDMod - playerStats.GetSPD() * selectedGemStatBlock.SPDMod) + ")";
            SPDChange.color = Color.red;
            
        }
        else if (playerStats.GetSPD() * equippedGemStatBlock.SPDMod <= playerStats.GetSPD() * selectedGemStatBlock.SPDMod)
        {
            SPDChange.text = "(+" + (playerStats.GetSPD() * selectedGemStatBlock.SPDMod - playerStats.GetSPD() * equippedGemStatBlock.SPDMod) + ")";
            SPDChange.color = Color.green;
        }
    }
}
