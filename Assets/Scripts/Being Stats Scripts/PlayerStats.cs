using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public PlayerStats()
    {
        HP = 15;
        MaxHP = 20;
        MP = 3;
        MaxMP = 15;

        ATK = 5;
        DEF = 7;
        SPD = 5;

        LVL = 3;
        XP = 2;
        baseXPThreshold = 15;
        LVLExponent = 1.4f;

        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxMPMod = 1f;
        MaxHPMod = 1f;
        XPMod = 1f;

        down = false;

    }

    protected override void LVLUp() // Adds all the basic stats for levelling up
    {
        int dont = Random.Range(1, 6);  // Randomly choose which stat to not level
        if (dont != 1){
            ATK += 1;
        }
        if (dont != 2)
        {
            DEF += 1;
        }
        if (dont != 3)
        {
            SPD += 1;
        }
        if (dont != 4)
        {
            HP += 2;
            MaxHP += 2;
        }
        if (dont != 5)
        {
            MP += 2;
            MaxMP += 2; 
        }

        // Give an extra bonus based on which gem is equipped when levelling occurs
        GemStatBlock currentGem = GetComponent<GemSystem>().CurrentGem;
        if(currentGem != null)
        {
            if(currentGem.name == "Will")
            {
                MaxHP += 1;
                HP += 1;
            }
            else if (currentGem.name == "Courage")
            {
                ATK += 1;
            }
            else if (currentGem.name == "Patience")
            {
                MaxMP += 1;
                MP += 1;
            }
            else if (currentGem.name == "Constitution")
            {
                DEF += 1;
            }
            else if (currentGem.name == "Cunning")
            {
                SPD += 1;
            }
            else if (currentGem.name == "Great Patience")
            {
                DEF += 1;
                ATK += 1;
            }
            else if (currentGem.name == "Heart")
            {
                MaxHP += 1;
                HP += 1;
                MaxMP += 1;
                MP += 1;
            }
        }

        // Give an EXTRA extra bonus based on player choice and averages
        // Which, uh, involves some selection and UI stuff that qualifies for a different method. Future Andrew <3
    }
}
