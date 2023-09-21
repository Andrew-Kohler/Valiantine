using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The backend setup for gems

[System.Serializable]
public class GemSystem : MonoBehaviour
{
    bool hasWill;
    bool hasCourage;
    bool hasConstitution;
    bool hasPatience;
    bool hasGreatPatience;
    bool hasCunning;
    bool hasHeart;

    // Current order in these lists: 
    // Will (starter)
    // Courage
    // Patience
    // Constitution
    // Cunning
    // Great Patience
    // Heart

    [SerializeField] protected List<GemStatBlock> gemStats;   // Stores the static, unchaging data of what the gems do
    public List<GemStatBlock> GemStats => gemStats;

    GemStatBlock currentGem;    // The gem that's currently equipped
    ItemData currentGemText;
    public GemStatBlock CurrentGem => currentGem;   // Getter for current gem
    public ItemData CurrentGemText => currentGemText;   // Getter for current gem item data

    [SerializeField] ItemData[] heldGemList;
    public ItemData[] HeldGemList => heldGemList;

    PlayerStats playerStats;

    private void OnEnable()
    {
        InventorySystem.onGemObtain += obtainGem;
    }

    private void OnDisable()
    {
        InventorySystem.onGemObtain -= obtainGem;
    }

    private void Start()
    {
        heldGemList = new ItemData[7];
        currentGem = gemStats[0];
        playerStats = PlayerManager.Instance.PlayerStats();
    }


    public void obtainGem(ItemData data) // Method for adding a gem to player inventory
    {
        heldGemList[data.MPRestore] = data;
    }

    public void equipGem(int index)  // Method for changing which gem is equipped
    {
        currentGem = gemStats[index];
        currentGemText = heldGemList[index];
        // TODO: Update Player Stats with all its new mods
        playerStats.SetGemATKMod(currentGem.ATKMod);
        playerStats.SetGemDEFMod(currentGem.DEFMod);
        playerStats.SetGemSPDMod(currentGem.SPDMod);
        playerStats.SetGemMaxHPMod(currentGem.HPMod);
        playerStats.SetGemMaxMPMod(currentGem.MPMod);

        if (playerStats.GetHP() > playerStats.GetMaxHP())   // Lowers HP and MP if maxes are lowered beyond previous full
        {
            playerStats.SetHP(-(playerStats.GetHP() - playerStats.GetMaxHP()), false);
        }
        if (playerStats.GetMP() > playerStats.GetMaxMP())
        {
            playerStats.SetMP(-(playerStats.GetMP() - playerStats.GetMaxMP()));
        }
    }

}
