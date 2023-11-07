using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearcrowMoves : EnemyMoves
{
    public static event DamagePlayer damagePlayer;
    // Bearcrow Moves
    // Move 1, 2, 3, 4 - Bearcrow Sit (0% damage, does not move, is just burlap and straw and some string really)

    protected override IEnumerator DoMove1(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove2(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove3(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove4(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }
}
