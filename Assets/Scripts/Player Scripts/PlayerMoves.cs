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

    public void SpellOfPatience()
    {
        StartCoroutine(DoPatience());
    }

    public void SpellOfGreatPatience()
    {
        StartCoroutine(DoGreatPatience());
    }

    public void SpellOfCunning()
    {
        StartCoroutine(DoCunning());
    }

    public void SpellOfHeart()
    {
        StartCoroutine(DoHeart());
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
        stats.SetMP(-5);
        // Play the spellcast animation


        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoCourage(GameObject enemy) 
    {
        moveInProgress = true;
        stats.SetMP(-6);
        // Play the spellcast animation
        animator.PlaySpell(5);
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        // Play the attack animation
        animator.PlayAttack(enemy.transform);
        yield return new WaitUntil(() => animator.dealDamage);  // Wait until it is time to deal damage

        int dmgDealt = (int)(stats.CalculateDMG(enemy.GetComponent<EnemyStats>().GetDEF()) * 1.75f); // Calculate damage being dealt 
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

    protected virtual IEnumerator DoConstitution(GameObject[] enemies) 
    {
        moveInProgress = true;
        // Play the spellcast animation
        animator.PlaySpell(4);
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoPatience()
    {
        moveInProgress = true;
        stats.SetMP(-4);
        // Play the spellcast animation
        animator.PlaySpell(3);

        // Set a timer so that on the turn after next (if this is turn 0, effect on turn 2)
        // All stats are x2 their normal values
        // Items are applied doubly
        BattleManager.Instance.SetPatienceTimer();
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoGreatPatience()
    {
        moveInProgress = true;
        // Play the spellcast animation
        stats.SetMP(-6);
        animator.PlaySpell(1);

        BattleManager.Instance.SetGreatPatienceTimer();
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoCunning()
    {
        moveInProgress = true;
        // Play the spellcast animation
        // Set mana cost
        animator.PlaySpell(0);
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoHeart()
    {
        moveInProgress = true;
        stats.SetMP(-4);
        // Play the spellcast animation
        animator.PlaySpell(2);
        stats.SetHeartSpell(); // Teehee, this one is easy because I did it already
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

}
