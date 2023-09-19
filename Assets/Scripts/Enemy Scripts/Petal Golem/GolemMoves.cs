using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMoves : EnemyMoves
{
    public static event DamagePlayer damagePlayer;
    protected override IEnumerator DoMove1(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on (maybe an event is better?)
        /*enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitUntil(() => enemyAnimatorS.dealDamage);  // Wait until it is time to deal damage
        int dmgDealt = enemyStats.CalculateDMG(playerStats.GetATK()); // Calculate damage being dealt (in this case, ATK power is a clean 100%)
        damagePlayer?.Invoke(-dmgDealt);                              // Send that via an event

        // PlayerStats receives the initial event, and then sends an animation event to PlayerAnimatorS once it determines whether Emily lives or dies

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation*/
        moveInProgress = false;                 // Lets other classes know the move is done (maybe an event is better?)
        enemyAnimatorS.dealDamage = false;
        yield return null;
    }
}
