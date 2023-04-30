using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    protected int HP;
    protected int MaxHP;
    protected int MP;
    protected int MaxMP;
    protected float ATK;
    protected float DEF;
    protected int SPD;
    [SerializeField] protected int LVL;
    protected int XP;
    protected int XPThreshold;

    protected float ATKBoost;
    protected float DEFBoost;

    protected bool down;

    public int GetHP()  // Getter and setter for HP
    {
        return HP;
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
        return MaxMP;
    }

    public float GetATK()   // No setter because permanent ATK stat should never be changed outside this class
    {
        return ATK;
    }

    public float GetDEF()   // No setter because permanent DEF stat should never be changed outside this class
    {
        return DEF;
    }

    public int GetSPD()   // No setter for same reason as DEF
    {
        return SPD;
    }

    public void SetATKBoost(float changeVal)   // No getter because temp ATK boost will never be referenced outside this class
    {
        ATKBoost += changeVal;
    }

    public float GetDEFBoost()
    {
        return DEFBoost;
    }

    public void SetDEFBoost(float changeVal)
    {
        DEFBoost += changeVal;
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
