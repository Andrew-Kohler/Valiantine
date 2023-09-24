using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMod
{
    int turnDuration;   // How long this buff/debuff is active for
    int type;           // What type of stat mod this is
    float mod;
    //float ATKMod;       // Mod type 0
    //float DEFMod;       // Mod type 1
    //float SPDMod;       // Mod type 2
    //float MaxHPMod;     // Mod type 3
    //float MaxMPMod;     // Mod type 4

    public StatMod(int turnDuration, int type, float mod) // Constructor
    {
        this.turnDuration = turnDuration;
        this.type = type;
        this.mod = mod;
    }

    public float getStatMod() // Return the value of this stat mod
    {
        return mod;
    }

    public int getType()
    {
        return type;
    }

    public void decrementDuration() // Subtracts 1 turn off of the modifier's lifetime
    {
        turnDuration--;
    }

    public int getDuration() // Gets how many turns this stat mod has left to live
    {
        return turnDuration;
    }
}
