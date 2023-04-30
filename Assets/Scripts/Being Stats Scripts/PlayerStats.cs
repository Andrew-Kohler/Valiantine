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
        ATK = 10f;
        DEF = 10f;
        SPD = 20;
        LVL = 1;
        XP = 1;
        XPThreshold = 2;
        ATKBoost = 0f;
        DEFBoost = 0f;
        down = false;

    }
}
