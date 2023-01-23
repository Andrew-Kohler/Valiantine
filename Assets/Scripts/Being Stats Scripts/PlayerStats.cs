using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public PlayerStats()
    {
        HP = 1;
        MaxHP = 1;
        MP = 1;
        MaxMP = 1;
        ATK = 1f;
        DEF = 1f;
        SPD = 2;
        LVL = 1;
        XP = 1;
        XPThreshold = 2;
        ATKBoost = 0f;
        DEFBoost = 0f;
        down = false;

    }
}
