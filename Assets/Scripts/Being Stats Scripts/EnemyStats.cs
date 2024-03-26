using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] public bool isBattling = false;           // If involved in the battle
    [SerializeField] public bool remainOnPlayerFlight = true; // If this enemy should continue to exist if the player runs from the battle
    [SerializeField] public string enemyName;
    [SerializeField] GameObject dmgNums;

    public int XPValue; // How much XP an enemy is worth
    public float height;

    EnemyAnimatorS enemyAnimator;
    public EnemyStats()
    {
        HP = 10;
        MaxHP = 10;
        MP = 10;
        MaxMP = 10;
        ATK = 7;
        DEF = 5;
        SPD = 4;
        LVL = 1;
        XP = 0;
        baseXPThreshold = 2;
        LVLExponent = 1.4f;
        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxMPMod = 1f;
        MaxHPMod = 1f;
        XPMod = 1f;
        down = false;
        XPValue = 3;
        height = 1;

    }

    private new void Start()
    {
        base.Start();
        enemyAnimator = GetComponentInChildren<EnemyAnimatorS>();
    }

    new private void OnEnable()
    {
        base.OnEnable();
    }

    new private void OnDisable()
    {
        base.OnDisable();
    }

    protected override void LVLUp()
    {
        // These should probably change between enemies? E.g. Petal Golems level DEF faster 
        int dont = Random.Range(1, 6);  // Randomly choose which stat to not level
        if (dont != 1)
        {
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
        XPValue = XPValue * 2;
    }

    public override void SetHP(int changeVal, bool crit)
    {
        HP += changeVal;
        if (HP > MaxHP)  // Accounts for attempts at healing beyond max, damage beyond min, and revives
        {
            HP = MaxHP;
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
            ouch.GetComponent<DamageNumbers>().SetValues(7f, changeVal, 1, crit);
            if (down)
            {
                enemyAnimator.PlayDie();
                // The game over sequence is Something I Have To Do
            }
            else
            {
                enemyAnimator.PlayHurt();
            }
        }
    }

    public void Resurrect() // Used on a parent enemy at the end of a battle if the player runs away
    {
        HP = MaxHP;
        if (down)
        {
            down = false;
            enemyAnimator.PlayDieReverse();
        }
    }

    public int GetXPValue()
    {
        return XPValue;
    }

    public void SetXPValue(int val)
    {
        XPValue = val;
    }

}
