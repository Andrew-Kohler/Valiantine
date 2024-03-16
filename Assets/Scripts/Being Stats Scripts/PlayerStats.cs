using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerStats : Stats
{
    private float GemATKMod;    // The gem modifiers are separate because they cannot be affected by enemy spells
    private float GemDEFMod;
    private float GemSPDMod;
    private float GemMaxMPMod;
    private float GemMaxHPMod;

    PlayerAnimatorS animator;

    private bool HeartProtectionActive; // Whether or not the Gem of Heart's protection spell is active
    //privca

    [SerializeField] GameObject dmgNums;

    public PlayerStats()
    {
        HP = 15;
        MaxHP = 20;
        MP = 3;
        MaxMP = 15;

        ATK = 5;
        DEF = 7;
        SPD = 5;

        LVL = 1;
        XP = 0;
        baseXPThreshold = 15;
        LVLExponent = 1.4f;

        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxMPMod = 1f;
        MaxHPMod = 1f;
        XPMod = 1f;

        GemATKMod = 1f;
        GemDEFMod = 1f;
        GemSPDMod = 1f;
        GemMaxMPMod = 1f;
        GemMaxHPMod = 1f;

        down = false;

    }

    public void SetStats(PlayerStats newStats)
    {
        HP = newStats.HP;
        MaxHP = newStats.MaxHP;
        MP = newStats.MP;
        MaxMP = newStats.MaxMP;

        ATK = newStats.ATK;
        DEF = newStats.DEF;
        SPD = newStats.SPD;

        LVL = newStats.LVL;
        XP = newStats.XP;
        baseXPThreshold = newStats.baseXPThreshold;
        LVLExponent = newStats.LVLExponent;

        ATKMod = newStats.ATKMod;
        DEFMod = newStats.DEFMod;
        SPDMod = newStats.SPDMod;
        MaxMPMod = newStats.MaxMPMod;
        MaxHPMod = newStats.MaxHPMod;
        XPMod = newStats.XPMod;

        GemATKMod = newStats.GemATKMod;
        GemDEFMod = newStats.GemDEFMod;
        GemSPDMod = newStats.GemSPDMod;
        GemMaxMPMod = newStats.GemMaxMPMod;
        GemMaxHPMod = newStats.GemMaxHPMod;

        down = false;
    }

    private void OnEnable()
    {
        base.OnEnable();
        SkullmetMoves.damagePlayer += SetHP;
    }

    private void OnDisable()
    {
        base.OnDisable();
        SkullmetMoves.damagePlayer -= SetHP;
    }

    private void Start()
    {
        base.Start();
        animator = GetComponentInChildren<PlayerAnimatorS>();
    }

    public override void SetHP(int changeVal, bool crit)
    {
        if (GetComponent<GemSystem>().CurrentGem.name == "Heart") // If the Gem of Heart is equipped, we have to do stuff differently
        {
            if (changeVal < 0 && HeartProtectionActive) // If we cast the Gem of Heart's spell, we reap its benefit on the next damaging hit
            {
                HeartProtectionActive = false;
                changeVal = changeVal * -1;
            }

            HP += changeVal;
            MP += changeVal;
            if (HP > GetMaxHP())  // Accounts for attempts at healing beyond max, damage beyond min, and revives
            {
                HP = GetMaxHP();
                MP = GetMaxMP();
            }
            else if (HP <= 0)
            {
                HP = 0;
                MP = 0;
                down = true;
            }
            else if (HP > 0 && down)
            {
                down = false;
            }

            if (changeVal < 0 && GameManager.Instance.isBattle())  // Animation logic
            {
                GameObject ouch = Instantiate(dmgNums, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - .02f), Quaternion.identity);
                ouch.GetComponent<DamageNumbers>().SetValues(7f, changeVal, -1, crit);
                if (down)
                {
                    // The game over sequence is Something I Have To Do
                }
                else
                {
                    animator.PlayHurt();
                }
            }
        }
        else // Normal stuff
        {
            if (changeVal < 0 && HeartProtectionActive) // If we cast the Gem of Heart's spell, we reap its benefit on the next damaging hit
            {
                HeartProtectionActive = false;
                changeVal = changeVal * -1;
            }

            HP += changeVal;
            if (HP > GetMaxHP())  // Accounts for attempts at healing beyond max, damage beyond min, and revives
            {
                HP = GetMaxHP();
            }
            else if (HP <= 0)
            {
                HP = 0;
                down = true;
            }
            else if (HP > 0 && down)
            {
                down = false;
            }

            if (changeVal < 0 && GameManager.Instance.isBattle())  // Animation logic
            {
                GameObject ouch = Instantiate(dmgNums, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - .02f), Quaternion.identity);
                ouch.GetComponent<DamageNumbers>().SetValues(7f, changeVal, -1, crit);
                if (down)
                {
                    // The game over sequence is Something I Have To Do
                    // This should probs be an event
                }
                else
                {
                    animator.PlayHurt();
                }
            }
        }
    }

    public override void SetMP(int changeVal)
    {
        if (GetComponent<GemSystem>().CurrentGem.name == "Heart") // If the Gem of Heart is equipped, we have to do stuff differently
        {
            if (changeVal < 0 && HeartProtectionActive) // If we cast the Gem of Heart's spell, we reap its benefit on the next damaging hit
            {
                HeartProtectionActive = false;
                changeVal = changeVal * -1;
            }

            HP += changeVal;
            MP += changeVal;
            if (HP > GetMaxHP())  // Accounts for attempts at healing beyond max, damage beyond min, and revives
            {
                HP = GetMaxHP();
                MP = GetMaxMP();
            }
            else if (HP <= 0)
            {
                HP = 0;
                MP = 0;
                down = true;
            }
            else if (HP > 0 && down)
            {
                down = false;
            }

            if (changeVal < 0 && GameManager.Instance.isBattle())  // Animation logic
            {
                GameObject ouch = Instantiate(dmgNums, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - .02f), Quaternion.identity);
                ouch.GetComponent<DamageNumbers>().SetValues(7f, changeVal, -1, false);
                if (down)
                {
                    // The game over sequence is Something I Have To Do
                }
                /*else  We don't play the hurt anim on an MP related loss because it might interfere with spellcast stuff
                {
                    animator.PlayHurt();
                }*/
            }
        }
        else
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
        
    }

    // Overrides for all of the stat getters to account for the passive modifications of the gems
    public new int GetMaxHP()
    {
        if (GetComponent<GemSystem>().CurrentGem.name == "Heart")
        {
            return (int)(MaxHP * MaxHPMod * GemMaxHPMod) + (int)(MaxMP * MaxMPMod * GemMaxMPMod);
        }
        return (int)(MaxHP * MaxHPMod * GemMaxHPMod);
    }
    public new int GetMaxMP()
    {
        if (GetComponent<GemSystem>().CurrentGem.name == "Heart")
        {
            return (int)(MaxHP * MaxHPMod * GemMaxHPMod) + (int)(MaxMP * MaxMPMod * GemMaxMPMod);

        }
        return (int)(MaxMP * MaxMPMod * GemMaxMPMod);
    }
    public new int GetATK()
    {
        return (int)(ATK * ATKMod * GemATKMod);
    }
    public new int GetDEF()
    {
        return (int)(DEF * DEFMod * GemDEFMod);
    }
    public new int GetSPD()
    {
        return (int)(SPD * SPDMod * GemSPDMod);
    }

    public float GetXPMod()
    {
        return XPMod;
    }

    // Setters for all of the gem modifiers
    public void SetGemMaxHPMod(float changeVal)
    {
        GemMaxHPMod = changeVal;
    }
    public void SetGemMaxMPMod(float changeVal)
    {
        GemMaxMPMod = changeVal;
    }
    public void SetGemATKMod(float changeVal)
    {
        GemATKMod = changeVal;
    }
    public void SetGemDEFMod(float changeVal)
    {
        GemDEFMod = changeVal;
    }
    public void SetGemSPDMod(float changeVal)
    {
        GemSPDMod = changeVal;
    }

    public void SetXPMod(float changeVal)
    {
        XPMod = changeVal;
    }

    // Unique setters to facilitate unique gem qualities - these are for ACTIVE SPELLS
    public void SetHeartSpell()
    {
        HeartProtectionActive = !HeartProtectionActive;
    }

    protected override void LVLUp() // Adds all the basic stats for levelling up
    {
        int dont = UnityEngine.Random.Range(1, 6);  // Randomly choose which stat to not level
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

    public override bool GetCrit()
    {
        bool crit = false;
        int max = 101;
        if (DateTime.Now.Month == 2 && DateTime.Now.Day == 14) // If it is Valentine's Day, the player crit chances are x2
        {
            max = 51;
        }

        if (UnityEngine.Random.Range(1, max) == 1)
        {
            crit = true;
        }

        return crit;
    }

    public override int CalculateDMG(int oppoDef) // Calculate the damage you are going to do to your opponent
    {
        int dmgDone = this.GetATK() - oppoDef;
        if (dmgDone <= 0)
        {
            dmgDone = 1;
        }
        return dmgDone;
    }

    public string GetLVLUpText()
    {
        GemStatBlock currentGem = GetComponent<GemSystem>().CurrentGem;
        string text = "";
        if (currentGem != null)
        {
            if (currentGem.name == "Will")
            {
                text = "Your experience levels you up, increasing your stats! You feel your will to continue on restored.";
            }
            else if (currentGem.name == "Courage")
            {
                text = "Your experience levels you up, increasing your stats! Courage swells within your heart as you take stock of your accomplishments.";
            }
            else if (currentGem.name == "Patience")
            {
                text = "Your experience levels you up, increasing your stats! You feel as though a wave of calm has washed over you.";
            }
            else if (currentGem.name == "Constitution")
            {
                text = "Your experience levels you up, increasing your stats! Your sores and wounds seem lesser now; you know they will become callouses and scars.";
            }
            else if (currentGem.name == "Cunning")
            {
                text = "Your experience levels you up, increasing your stats! Your armor weights a little less heavily on your shoulders, and you find a new bounce in your step.";
            }
            else if (currentGem.name == "Great Patience")
            {
                text = "Your experience levels you up, increasing your stats! Battle has forged you into a warrior, making you tougher and more dangerous.";
            }
            else if (currentGem.name == "Heart")
            {
                text = "Your experience levels you up, increasing your stats! You long for the hour when you can put down your sword; it is this longing that empowers you.";
            }
        }
        return text;
    }
}
