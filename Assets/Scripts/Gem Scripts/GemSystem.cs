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
    public GemStatBlock CurrentGem => currentGem;   // Getter for current gem
    
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
        // TODO: Update Player Stats with all its new mods
        playerStats.SetATKMod(currentGem.ATKMod);
        playerStats.SetDEFMod(currentGem.DEFMod);
        playerStats.SetSPDMod(currentGem.SPDMod);
        playerStats.SetMaxHPMod(currentGem.HPMod);
        playerStats.SetMaxMPMod(currentGem.MPMod);
    }

}
