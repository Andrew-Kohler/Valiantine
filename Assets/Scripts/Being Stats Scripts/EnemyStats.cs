using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] public bool isBattling = false;           // If involved in the battle
    [SerializeField] public bool remainOnPlayerFlight = true; // If this enemy should continue to exist if the player runs from the battle
    [SerializeField] public string enemyName;
    [SerializeField] GameObject dmgNums;

    EnemyAnimatorS enemyAnimator;
    public EnemyStats()
    {
        HP = 1;
        MaxHP = 1;
        MP = 1;
        MaxMP = 1;
        ATK = 1;
        DEF = 1;
        SPD = 1;
        LVL = 1;
        XP = 1;
        baseXPThreshold = 2;
        LVLExponent = 1.4f;
        ATKMod = 1f;
        DEFMod = 1f;
        SPDMod = 1f;
        MaxMPMod = 1f;
        MaxHPMod = 1f;
        XPMod = 1f;
        down = false;

    }

    private void Start()
    {
        enemyAnimator = GetComponentInChildren<EnemyAnimatorS>();
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
    }

    public override void SetHP(int changeVal)
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
            GameObject ouch = Instantiate(dmgNums, this.transform.position, Quaternion.identity);
            ouch.GetComponent<DamageNumbers>().SetValues(7f, changeVal, 1);
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

}
