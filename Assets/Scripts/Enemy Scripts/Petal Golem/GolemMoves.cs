using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMoves : EnemyMoves
{
    public static event DamagePlayer damagePlayer;
    //Golem Moves
    //1
    //2
    // 3 - Persuasive Napping (Drop player speed by 20%)
    // 4 - Strength of Ages (Buff all enemy DEF by 25%)
    protected override IEnumerator DoMove1(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        /*enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);  // Wait until it is time to deal damage

        int dmgDealt = enemyStats.CalculateDMG(playerStats.GetDEF()); // Calculate damage being dealt (in this case, ATK power is a clean 100%)
        Debug.Log(dmgDealt);
        if (enemyStats.GetCrit())
        {
            damagePlayer?.Invoke(-dmgDealt * 2, true);                              // Send that via an event
        }
        else
        {
            damagePlayer?.Invoke(-dmgDealt, false);      // PlayerStats receives the initial event, and then sends an animation event to PlayerAnimatorS
                                                         // once it determines whether Emily lives or dies  */                      
        //}

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove2(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        /*enemyAnimatorS.PlayMove2();             // Play the attack animation

        // Buff all living enemies' ATK by 15% for 3 turns and heal them for 15%
        foreach (GameObject enemy in BattleManager.Instance.GetBattlingEnemies())
        {
            if (!enemy.GetComponent<EnemyStats>().getDowned())
            {
                enemy.GetComponent<EnemyStats>().UpdateStatMods(new StatMod(3, 0, .15f));
                enemy.GetComponent<EnemyStats>().SetHP((int)(enemy.GetComponent<EnemyStats>().GetMaxHPRaw() * .15f), false);
            }
        }
        */
        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove3(PlayerStats playerStats)
    {
        moveInProgress = true;
        enemyAnimatorS.PlayMove3();
        /*
        playerStats.UpdateStatMods(new StatMod(2, 0, -.2f));    // Drop player ATK by 20% for 2 turns
        */
        playerStats.UpdateStatMods(new StatMod(2, 2, -.2f));    // Drop player SPD by 20% for 2 turns
        
        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);
        moveInProgress = false;
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove4(PlayerStats playerStats)
    {
        moveInProgress = true;
        enemyAnimatorS.PlayMove4();
        foreach (GameObject enemy in BattleManager.Instance.GetBattlingEnemies())
        {
            if (!enemy.GetComponent<EnemyStats>().getDowned())
            {
                enemy.GetComponent<EnemyStats>().UpdateStatMods(new StatMod(3, 1, .25f));
            }
        }

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);
        moveInProgress = false;
        enemyAnimatorS.dealDamage = false;
    }
}
