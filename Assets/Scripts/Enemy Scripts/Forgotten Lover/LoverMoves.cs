using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoverMoves : EnemyMoves
{
    public static event DamagePlayer damagePlayer;
    // Lover Moves
    // Move 1 - Vine Jab, ATK for 100%
    // Move 2 - 
    // Move 3 - Heartpiercer, ATK for 75% and apply 15% ATK and SPD debuffs
    // Move 4 - 

    protected override IEnumerator DoMove1(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
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
                                                         // once it determines whether Emily lives or dies                        
        }

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove2(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove2();             // Play the attack animation
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);  // Wait until it is time to deal damage
        playerStats.UpdateStatMods(new StatMod(3, 1, -.1f));    // Drop player DEF by 10% for 3 turns
        int dmgDealt = enemyStats.CalculateDMG(playerStats.GetDEF()); // Calculate damage being dealt (in this case, ATK power is a clean 100%)
        if (enemyStats.GetCrit())
        {
            damagePlayer?.Invoke((int)(-dmgDealt * 2 * 1.3f), true);                              // Send that via an event
        }
        else
        {
            damagePlayer?.Invoke((int)(-dmgDealt * 1.3f), false);      // PlayerStats receives the initial event, and then sends an animation event to PlayerAnimatorS
                                                                       // once it determines whether Emily lives or dies                        
        }

        yield return new WaitForSeconds(.6f);
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);  // Wait until it is time to deal damage
        dmgDealt = enemyStats.CalculateDMG(playerStats.GetDEF()); // Calculate damage being dealt (in this case, ATK power is a clean 100%)
        playerStats.UpdateStatMods(new StatMod(3, 1, -.1f));    // Drop player DEF by 10% for 3 turns
        if (enemyStats.GetCrit())
        {
            damagePlayer?.Invoke((int)(-dmgDealt * 2 * .3f), true);                              // Send that via an event
        }
        else
        {
            damagePlayer?.Invoke((int)(-dmgDealt * .3f), false);      // PlayerStats receives the initial event, and then sends an animation event to PlayerAnimatorS
                                                                      // once it determines whether Emily lives or dies                        
        }

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove3(PlayerStats playerStats)
    {
        moveInProgress = true;
        enemyAnimatorS.PlayMove3();
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);
        int dmgDealt = enemyStats.CalculateDMG(playerStats.GetDEF());
        if (enemyStats.GetCrit())
        {
            damagePlayer?.Invoke((int)(-dmgDealt * 2 * .75f), true);
        }
        else
        {
            damagePlayer?.Invoke((int)(-dmgDealt * .75f), false);

        }
        playerStats.UpdateStatMods(new StatMod(2, 0, -.2f));    // Drop player ATK by 20% for 2 turns
        playerStats.UpdateStatMods(new StatMod(2, 2, -.2f));    // Drop player DEF by 20% for 2 turns

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);
        moveInProgress = false;
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove4(PlayerStats playerStats)
    {
        moveInProgress = true;
        enemyAnimatorS.PlayMove4();
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);

        // Buff all living enemies' ATK, DEF, and SPD stats by 15% for 3 turns
        foreach (GameObject enemy in BattleManager.Instance.GetBattlingEnemies())
        {
            if (!enemy.GetComponent<EnemyStats>().getDowned())
            {
                enemy.GetComponent<EnemyStats>().UpdateStatMods(new StatMod(3, 0, .15f));
                enemy.GetComponent<EnemyStats>().UpdateStatMods(new StatMod(3, 1, .15f));
                enemy.GetComponent<EnemyStats>().UpdateStatMods(new StatMod(3, 2, .15f));
            }
        }

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);
        yield return new WaitForSeconds(.8f);
        moveInProgress = false;
        enemyAnimatorS.dealDamage = false;
    }
}
