using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMoves : EnemyMoves
{
    public static event DamagePlayer damagePlayer;
    //Golem Moves
    // 1
    // 2 - Restorative Slumber (Fall asleep for one turn, heal 50%)
    // 3 - Persuasive Napping (Drop player speed by 20%)
    // 4 - Strength of Ages (Buff all enemy DEF by 25%)
    public bool asleep = false;

    public override void Move(PlayerStats playerStats)
    {
        if(!asleep)
            base.Move(playerStats); 
        else
            Move2(playerStats);
    }
    protected override IEnumerator DoMove1(PlayerStats playerStats)
    {
        moveInProgress = true;                  // Lets other classes know a move is going on 
        ViewManager.GetView<BattleUIView>().setText(BattleManager.Instance.GetCurrentTurnName() + " puts its left foot in!");
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

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove2(PlayerStats playerStats)
    {
        
        moveInProgress = true;                  // Lets other classes know a move is going on 
        if (!asleep) // Heal if this is the start of the move
        {
            ViewManager.GetView<BattleUIView>().setText(BattleManager.Instance.GetCurrentTurnName() + " has elected to take a nap.");
            GetComponent<EnemyStats>().SetHP((int)(GetComponent<EnemyStats>().GetMaxHPRaw() * .5f), false);
        }
        else
        {
            ViewManager.GetView<BattleUIView>().setText(BattleManager.Instance.GetCurrentTurnName() + " wakes up!");
        }
        enemyAnimatorS.PlayMove2();

        yield return new WaitUntil(() => enemyAnimatorS.activeCoroutine == false);   // Wait out the rest of the animation
        moveInProgress = false;                 // Lets other classes know the move is done 
        enemyAnimatorS.dealDamage = false;
    }

    protected override IEnumerator DoMove3(PlayerStats playerStats)
    {
        moveInProgress = true;
        ViewManager.GetView<BattleUIView>().setText(BattleManager.Instance.GetCurrentTurnName() + " takes a power nap. You feel sleepy...");
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
        ViewManager.GetView<BattleUIView>().setText(BattleManager.Instance.GetCurrentTurnName() + " gives its friends the strength of ages!");
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
