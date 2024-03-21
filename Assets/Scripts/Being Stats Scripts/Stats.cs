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

    protected float ATKMod = 1;
    protected float DEFMod = 1;
    protected float SPDMod = 1;
    protected float MaxHPMod = 1;
    protected float MaxMPMod = 1;
    protected float XPMod = 1;

    protected bool down;

    protected Stack<StatMod> stack1;
    protected Stack<StatMod> stack2;

    protected StatModVisualController statVisuals;
    // Because we'd need 2, right?
    // We're popping them off one and pushing them onto another every time we check

    protected void OnEnable()
    {
        BattleManager.battleNewTurn += DecrementStatMods;
    }

    protected void OnDisable()
    {
        BattleManager.battleNewTurn -= DecrementStatMods;
    }

    private void Awake()
    {
        for (int i = 1; i < LVL; i++)
        {
            LVLUp();
        }
    }

    protected void Start()
    {
        stack1 = new Stack<StatMod>();
        stack2 = new Stack<StatMod>();
        statVisuals = GetComponent<StatModVisualController>();
    }

    public int GetHP()  // Getter and setter for HP
    {
        return HP;
    }

    public virtual void SetHP(int changeVal, bool crit)
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

    public void SetHPDir(int changeVal) //  DO NOT USE UNLESS YOU KNOW WHAT YOU ARE DOING
    {
        HP = changeVal;
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

    public virtual void SetMP(int changeVal)
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

    public void SetMPDir(int changeVal) //  DO NOT USE UNLESS YOU KNOW WHAT YOU ARE DOING
    {
        MP = changeVal;
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

    public bool SetXP(int changeVal)
    {
        XP += changeVal;
        if(XP > GetXPThreshold())    // If it's time to level up
        {
            int leftover = XP - GetXPThreshold();
            XP = 0;
            LVL += 1;
            LVLUp();
            SetXP(leftover);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual int CalculateDMG(int oppoDef) // Calculate the damage you are going to do to your opponent
    {
        int dmgDone = this.GetATK() - oppoDef;
        if(dmgDone <= 0)
        {
            dmgDone = 1;
        }
        return dmgDone;
    }

    public virtual bool GetCrit()
    {
        bool crit = false;
        int max = 101;
        if (Random.Range(1, max) == 1)
        {
            crit = true;
        }

        return crit;
    }

    protected void AddStatMod(StatMod newMod)
    {
        int type = newMod.getType();
        if (type == 0)
            ATKMod += newMod.getStatMod();
        else if (type == 1)
            DEFMod += newMod.getStatMod();
        else if (type == 2)
            SPDMod += newMod.getStatMod();
        else if (type == 3)
            MaxHPMod += newMod.getStatMod();
        else if (type == 4)
            MaxMPMod += newMod.getStatMod();

/*        if(newMod.getStatMod() > 0)
        {
            statVisuals.PlayStatChange(type, true);
        }
        else
        {
            statVisuals.PlayStatChange(type, false);
        }*/
    }

    public void UpdateStatMods(StatMod newMod) // Takes in a new StatMod and refreshes our count on all stats
    {

        AddStatMod(newMod);
        if (stack1.Count == 0) // Just whichever foot we're on - determines the correct one to add it to
        {
            stack2.Push(newMod);
        }
        else
        {
            stack1.Push(newMod);
        }
    }

    public void DecrementStatMods() // At the start of every new turn, take note of which buffs have run their course, and retally
    {
        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxHPMod = 1f;
        MaxMPMod = 1f;

        if (stack1.Count == 0) // Just whichever foot we're on
        {
            while (stack2.Count > 0)
            {
                // Perform on the top of the stack:
                if(stack2.TryPeek(out StatMod result))
                {
                    result.decrementDuration();  // Decrement the turn counter
                    if (result.getDuration() > 0)// If it's not done, add it to the other stack before popping it
                    {
                        AddStatMod(result);
                        stack1.Push(result);
                    }
                    stack2.Pop();
                }
            }
        }
        else
        {
            while (stack1.Count > 0)
            {
                // Perform on the top of the stack:
                if (stack1.TryPeek(out StatMod result))
                {
                    result.decrementDuration();  // Decrement the turn counter
                    if (result.getDuration() > 0)// If it's not done, add it to the other stack before popping it
                    {
                        AddStatMod(result);
                        stack2.Push(result);
                    }
                    stack1.Pop();
                }
            }
        }
    }

    public void ClearStatMods()
    {
        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxHPMod = 1f;
        MaxMPMod = 1f;

        if (stack1.Count == 0) // Just whichever foot we're on
        {
            while (stack2.Count > 0)
            {
                stack2.Pop();
            }
        }
        else
        {
            while (stack1.Count > 0)
            {
                stack1.Pop();
            }
        }
    }

    public bool getDowned()
    {
        return down;
    }
    
}
