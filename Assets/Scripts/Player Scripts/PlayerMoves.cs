using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoves : MonoBehaviour
{
    private PlayerAnimatorS animator;
    private PlayerStats stats;

    public bool moveInProgress; // Allows us to halt the battle manager until a move is done

    private void Start()
    {
        animator = GetComponentInChildren<PlayerAnimatorS>();
        stats = GetComponent<PlayerStats>();
    }
    public void Attack(GameObject enemy)
    {
        StartCoroutine(DoAttack(enemy));
    }

    public void SpellOfWill()
    {
        StartCoroutine(DoWill());
    }


    // Coroutines -----------------------------------------------
    protected virtual IEnumerator DoAttack(GameObject enemy)
    {
        moveInProgress = true;
        animator.PlayAttack(enemy.transform);
        yield return new WaitUntil(() => animator.dealDamage);  // Wait until it is time to deal damage

        int dmgDealt = stats.CalculateDMG(enemy.GetComponent<EnemyStats>().GetDEF()); // Calculate damage being dealt 
        enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt);

        yield return new WaitUntil(() => animator.activeCoroutine == false);   // Wait out the rest of the animation

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoWill() // I will add all of the spells...
    {
        yield return null;
    }

}
