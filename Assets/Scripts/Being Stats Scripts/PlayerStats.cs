using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public PlayerStats()
    {
        HP = 22;
        MaxHP = 33;
        MP = 3;
        MaxMP = 18;

        ATK = 10;
        DEF = 10;
        SPD = 20;

        LVL = 1;
        XP = 2;
        XPThreshold = 15;

        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxMPMod = 1f;
        MaxHPMod = 1f;
        XPMod = 1f;

        down = false;

    }
}
