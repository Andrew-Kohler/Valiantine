using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    public EnemyStats()
    {
        HP = 1;
        MaxHP = 1;
        MP = 1;
        MaxMP = 1;
        ATK = 1f;
        DEF = 1f;
        SPD = 1;
        LVL = 1;
        XP = 1;
        XPThreshold = 2;
        ATKMod = 1f;
        DEFMod = 1f;
        down = false;

    }
}
