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
        ViewManager.GetView<BattleUIView>().setText("You wonder why somebody left a bearcrow in the middle of the woods.");
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove2(PlayerStats playerStats)
    {
        ViewManager.GetView<BattleUIView>().setText("Whoever made this went to a lot of effort to just leave it here.");
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove3(PlayerStats playerStats)
    {
        ViewManager.GetView<BattleUIView>().setText("There are no crows around. It must be working.");
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove4(PlayerStats playerStats)
    {
        ViewManager.GetView<BattleUIView>().setText("The bearcrow, being inanimate, does nothing.");
        moveInProgress = true;                  // Lets other classes know a move is going on 
        enemyAnimatorS.PlayMove1();             // Play the attack animation
        yield return new WaitForSeconds(3f);
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }
}
