using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    protected float HP;
    protected float MaxHP;
    protected float MP;
    protected float MaxMP;
    protected float ATK;
    protected float DEF;
    protected float SPD;
    [SerializeField] protected int LVL;
    protected int XP;
    protected int XPThreshold;

    protected float ATKMod;
    protected float DEFMod;
    protected float SPDMod;
    protected float MaxHPMod;
    protected float MaxMPMod;
    protected float XPMod;

    protected bool down;

    public int GetHP()  // Getter and setter for HP
    {
        return (int)HP;
    }

    public void SetHP(int changeVal)
    {
        HP += changeVal;
        if(HP > MaxHP)  // Accounts for attempts at healing beyond max, damage beyond min, and revives
        {
            HP = MaxHP;
        }
        else if(HP < 0)
        {
            HP = 0;
            down = true;
        }
        else if(HP > 0 && down)
        {
            down = false;
        }
    }

    public int GetMaxHP()
    {
        return (int)(MaxHP * MaxHPMod);
    }

    public int GetMaxHPRaw()
    {
        return (int)MaxHP;
    }

    public int GetMP()  // Getter and setter for MP
    {
        return (int)MP;
    }

    public void SetMP(int changeVal)
    {
        MP += changeVal;
        if (MP > MaxMP)  // Accounts for attempts at re-upping beyond max and trying to spend beyond 0
        {                // The latter should NEVER occur if I'm doing things right elsewhere
            MP = MaxMP;
        }
        else if (MP < 0)
        {
            MP = 0;
        }
    }

    public int GetMaxMP()
    {
        return (int)(MaxMP * MaxMPMod);
    }

    public int GetMaxMPRaw()
    {
        return (int)MaxMP;
    }

    public float GetATK()   // No setter because permanent ATK stat should never be changed outside this class
    {
        return ATK * ATKMod;
    }

    public float GetATKRaw()
    {
        return ATK;
    }

    public float GetDEF()   // No setter because permanent DEF stat should never be changed outside this class
    {
        return DEF * DEFMod;
    }

    public float GetDEFRaw()
    {
        return DEF;
    }

    public int GetSPD()   // No setter for same reason as DEF
    {
        return (int)(SPD * SPDMod);
    }

    public int GetSPDRaw()
    {
        return (int)SPD;
    }

    public void SetATKMod(float changeVal)   // No getter because temp ATK boost will never be referenced outside this class
    {
        ATKMod = changeVal;
    }

    public float GetDEFMod()
    {
        return DEFMod;
    }

    public void SetDEFMod(float changeVal)
    {
        DEFMod = changeVal;
    }

    public void SetSPDMod(float changeVal)
    {
        SPDMod = changeVal;
    }

    public void SetMaxHPMod(float changeVal)
    {
        MaxHPMod = changeVal;
    }

    public void SetMaxMPMod(float changeVal)
    {
        MaxMPMod = changeVal;
    }

    public int GetLVL() // No setter because level changes are triggered by the XP setter method
    {
        return LVL;
        // Get this before and after we set XP in BM if we win so we know if we need to tell the player they levelled up
    }

    private void LVLUp()    // Contains the logic for levelling up
    {
        // This is a separate Trello card and a wholly separate topic from just writing this class
        // Potentially abstract?
    }

    public int GetXP()
    {
        return XP;
    }

    public int GetXPThreshold()
    {
        return XPThreshold;
    }

    public void SetXP(int changeVal)
    {
        XP += changeVal;
        if(XP > XPThreshold)    // If it's time to level up
        {
            int leftover = XP - XPThreshold;
            XP = 0;
            LVLUp();
            SetXP(leftover);
        }
    }

    public int CalculateDMG(float oppoDef)
    {
        return 1;
        //TODO: Come up with me secret formula, ARGARGARGARGH
    }

    public bool getDowned()
    {
        return down;
    }
    
}
