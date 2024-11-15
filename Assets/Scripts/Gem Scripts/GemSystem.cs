using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The backend setup for gems

[System.Serializable]
public class GemSystem : MonoBehaviour
{
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
    public int currentGemIndex = 0;        // Index for display
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
        playerStats = PlayerManager.Instance.PlayerStats();
        heldGemList = new ItemData[7];
        if(currentGem == null && heldGemList[0] == null)
        {
            obtainGem(Resources.Load<ItemData>("Gems/" + gemStats[0].name));
            equipGem(0);
            //currentGem = gemStats[0];
            currentGemIndex = 0;
        }
       
        
    }

    public void RefillGems(GemSystem newGems)   // For scene transitions
    {
        if(heldGemList != null)
        {
            for (int i = 0; i < heldGemList.Length; i++)
            {
                if(newGems.heldGemList[i] != null)
                    obtainGem(newGems.heldGemList[i]);
            }
            currentGem = newGems.currentGem;
        }
        currentGemIndex = newGems.currentGemIndex;
        
    }

    public void RefillGemsFromSaveData(GameManager.SavedGems gems)
    {
        for(int i = 0; i < gems.heldGems.Length; i++)
        {
            if (gems.heldGems[i]) // If we had this gem last we saved, re-add it to our inventory
            {
                obtainGem(Resources.Load<ItemData>("Gems/"+gemStats[i].name));
            }
        }

        if(heldGemList[gems.equippedGemIndex] != null)
        {
            equipGem(gems.equippedGemIndex); // Re-equip whatever we had on at the time
        }
        
    }

    public int GetRandomGemIndex()
    {
        int val;
        int index = Random.Range(0, heldGemList.Length);
        if (heldGemList[index] != null)
        {
            if (heldGemList[index].name != "Cunning")
                val = index;
            else
                val = GetRandomGemIndex();
        }
            
        else
            val = GetRandomGemIndex();

        return val;
    }

    public void obtainGem(ItemData data) // Method for adding a gem to player inventory
    {
        heldGemList[data.MPRestore] = data;
    }


    public void equipGem(int index)  // Method for changing which gem is equipped
    {
        currentGem = gemStats[index];
        currentGemText = heldGemList[index];
        currentGemIndex = index;
        // TODO: Update Player Stats with all its new mods
        playerStats.SetGemATKMod(currentGem.ATKMod);
        playerStats.SetGemDEFMod(currentGem.DEFMod);
        playerStats.SetGemSPDMod(currentGem.SPDMod);
        playerStats.SetGemMaxHPMod(currentGem.HPMod);
        playerStats.SetGemMaxMPMod(currentGem.MPMod);
        playerStats.SetXPMod(currentGem.XPMod);

        if (playerStats.GetHP() > playerStats.GetMaxHP())   // Lowers HP and MP if maxes are lowered beyond previous full
        {
            playerStats.SetHP(-(playerStats.GetHP() - playerStats.GetMaxHP()), false);
        }
        if (playerStats.GetMP() > playerStats.GetMaxMP())
        {
            playerStats.SetMP(-(playerStats.GetMP() - playerStats.GetMaxMP()));
        }

        if(currentGem.name == "Heart")
        {
            int avg = (playerStats.GetMP() + playerStats.GetHP()) / 2;
            Debug.Log(avg);
            playerStats.SetMPDir(avg);
            playerStats.SetHPDir(avg);
        }
    }

    public bool[] GetGemContents() // Returns a true/false array of which gems we have or don't
    {
        bool[] contents = new bool[7];
        for(int i = 0; i < heldGemList.Length; i++)
        {
            if(heldGemList[i] != null)
            {
                contents[i] = true; 
            }
        }
        return contents;
    }



}
