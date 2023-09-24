using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullmetMoves : EnemyMoves
{
    public static event DamagePlayer damagePlayer;
    // Skullmet Moves
    // Move 1 - Bite, ATK for 100%
    // Move 2 - Ghastly Gnawing, ATK for 35% x 2 and drop player DEF by 10% twice
    // Move 3 - Helmet Bash, ATK for 150% 
    // Move 4 - War Cry, raise all enemies' ATK/DEF/SPD by 20%

    protected override IEnumerator DoMove1(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);  // Wait until it is time to deal damage
        int dmgDealt = enemyStats.CalculateDMG(playerStats.GetDEF()); // Calculate damage being dealt (in this case, ATK power is a clean 100%)
        if (enemyStats.GetCrit())
        {
            damagePlayer?.Invoke(-dmgDealt * 2, true);                              // Send that via an event
        }
        else
        {
            damagePlayer?.Invoke(-dmgDealt, false);      // PlayerStats receives the initial event, and then sends an animation event to PlayerAnimatorS
                                                         // once it determines whether Emily lives or dies                        
        }

        yield return new WaitUntil(()=> enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove2(PlayerStats playerStats)
    {
        moveInProgress = true;                


        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   
        moveInProgress = false;                 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove3(PlayerStats playerStats)
    {
        moveInProgress = true;               


        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false); 
        moveInProgress = false;           
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove4(PlayerStats playerStats)
    {
        moveInProgress = true;    


        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);  
        moveInProgress = false;             
        enemyAnimatorS.dealDamage = false;
    }
}
