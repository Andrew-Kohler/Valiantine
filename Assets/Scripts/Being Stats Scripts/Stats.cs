using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stats : MonoBehaviour
{
    protected int HP;
    protected int MaxHP;
    protected int MP;
    protected int MaxMP;
    protected int ATK;
    protected int DEF;
    protected int SPD;
    [SerializeField] protected int LVL;
    protected int XP;
    protected int baseXPThreshold;
    protected float LVLExponent;

    protected float ATKMod;
    protected float DEFMod;
    protected float SPDMod;
    protected float MaxHPMod;
    protected float MaxMPMod;
    protected float XPMod;

    protected bool down;

    private void Awake()
    {
        for (int i = 1; i < LVL; i++)
        {
            LVLUp();
            //Debug.Log("LVL Up");
        }
    }

    public int GetHP()  // Getter and setter for HP
    {
        return HP;
    }

    public virtual void SetHP(int changeVal)
    {
        HP += changeVal;
        if(HP > MaxHP)  // Accounts for attempts at healing beyond max, damage beyond min, and revives
        {
            HP = MaxHP;
        }
        else if(HP <= 0)
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
        return MaxHP;
    }

    public int GetMP()  // Getter and setter for MP
    {
        return MP;
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
        return MaxMP;
    }

    public int GetATK()   // No setter because permanent ATK stat should never be changed outside this class
    {
        return (int)(ATK * ATKMod);
    }

    public int GetATKRaw()
    {
        return ATK;
    }

    public int GetDEF()   // No setter because permanent DEF stat should never be changed outside this class
    {
        return (int)(DEF * DEFMod);
    }

    public int GetDEFRaw()
    {
        return DEF;
    }

    public int GetSPD()   // No setter for same reason as DEF
    {
        return (int)(SPD * SPDMod);
    }

    public int GetSPDRaw()
    {
        return SPD;
    }

    public void SetATKMod(float changeVal)   // No getter because temp ATK boost will never be referenced outside this class
    {
        ATKMod = changeVal;
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

    public void SetLVL(int LVL)
    {
        for (int i = this.LVL; i < LVL; i++)
        {
            LVLUp();
            //Debug.Log("LVL Up");
        }
        this.LVL = LVL;
    }

    protected abstract void LVLUp();    // Contains the logic for levelling up

    public int GetXP()
    {
        return XP;
    }

    public int GetXPThreshold()
    {
        return (int)Mathf.Floor(baseXPThreshold * Mathf.Pow(LVL, LVLExponent)); 
    }

    public void SetXP(int changeVal)
    {
        XP += changeVal;
        if(XP > GetXPThreshold())    // If it's time to level up
        {
            int leftover = XP - GetXPThreshold();
            XP = 0;
            LVL += 1;
            LVLUp();
            SetXP(leftover);
        }
    }

    public int CalculateDMG(int oppoDef) // Calculate the damage you are going to do to your opponent
    {
        int dmgDone = this.GetATK() - oppoDef;
        if(dmgDone <= 0)
        {
            dmgDone = 1;
        }
        return dmgDone;
    }

    public bool getDowned()
    {
        return down;
    }
    
}
