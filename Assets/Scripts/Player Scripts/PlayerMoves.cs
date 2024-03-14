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

    public void SpellOfWill(GameObject enemy)
    {
        StartCoroutine(DoWill(enemy));
    }

    public void SpellOfCourage(GameObject enemy)
    {
        StartCoroutine(DoCourage(enemy));
    }

    public void SpellOfConstitution(GameObject[] enemies)
    {
        StartCoroutine(DoConstitution(enemies));
    }


    // Coroutines -----------------------------------------------
    protected virtual IEnumerator DoAttack(GameObject enemy)
    {
        moveInProgress = true;
        animator.PlayAttack(enemy.transform);
        yield return new WaitUntil(() => animator.dealDamage);  // Wait until it is time to deal damage

        int dmgDealt = stats.CalculateDMG(enemy.GetComponent<EnemyStats>().GetDEF()); // Calculate damage being dealt 
        if (stats.GetCrit())
        {
            enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt * 2, true);
        }
        else
        {
            enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt, false);
        }
        

        yield return new WaitUntil(() => animator.activeCoroutine == false);   // Wait out the rest of the animation

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoWill(GameObject enemy) // I will add all of the spells...
    {
        moveInProgress = true;
        

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoCourage(GameObject enemy) 
    {
        moveInProgress = true;

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoConstitution(GameObject[] enemies) 
    {
        moveInProgress = true;

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoPatience()
    {
        moveInProgress = true;

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoGreatPatience()
    {
        moveInProgress = true;

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoCunning()
    {
        moveInProgress = true;

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoHeart()
    {
        moveInProgress = true;

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

}
