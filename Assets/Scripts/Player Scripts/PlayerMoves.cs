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
        
        animator.PlaySpell(6);
        // When it's time to do damage, get fancy
        yield return new WaitUntil(() => animator.dealDamage);  // Wait until it is time to deal damage

        Object fire = Resources.Load("Fire");

        // Spawn the cool effect
        Instantiate(fire, enemy.transform.position, Quaternion.Euler(0, 0, 0));
        // Deal damage
        int dmgDealt = (int)(stats.CalculateDMG(enemy.GetComponent<EnemyStats>().GetDEF()) * 1.5f); // Calculate damage being dealt 
        if (stats.GetCrit())
        {
            enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt * 2, true);
        }
        else
        {
            enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt, false);
        }
        stats.UpdateStatMods(new StatMod(3, 0, .1f));
        stats.UpdateStatMods(new StatMod(3, 1, .1f));
        stats.UpdateStatMods(new StatMod(3, 2, .1f));

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoCourage(GameObject enemy) 
    {
        moveInProgress = true;

        // Play the spellcast animation
        animator.PlaySpell(5);
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        // Play the attack animation
        animator.PlayAttack(enemy.transform);
        yield return new WaitUntil(() => animator.dealDamage);  // Wait until it is time to deal damage

        int dmgDealt = (int)(stats.CalculateDMG(enemy.GetComponent<EnemyStats>().GetDEF()) * 2.5f); // Calculate damage being dealt 
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

        // When it's time to do damage, get fancy
        yield return new WaitUntil(() => animator.dealDamage);  // Wait until it is time to deal damage

        Object lighting = Resources.Load("Lightning");
        foreach (GameObject enemy in enemies)
        {
            // Spawn the cool effect
            Instantiate(lighting, enemy.transform.position, Quaternion.Euler(90,0,-180));
            // Deal damage
            int dmgDealt = Mathf.Max((int)(stats.CalculateDMG(enemy.GetComponent<EnemyStats>().GetDEF())), 1); // Calculate damage being dealt 
            if (stats.GetCrit())
            {
                enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt * 2, true);
            }
            else
            {
                enemy.GetComponent<EnemyStats>().SetHP(-dmgDealt, false);
            }
        }
        

        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

    protected virtual IEnumerator DoPatience()
    {
        moveInProgress = true;

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

        // Play the spellcast animation
        animator.PlaySpell(2);
        stats.SetHeartSpell(); // Teehee, this one is easy because I did it already
        yield return new WaitUntil(() => animator.activeCoroutine == false);

        moveInProgress = false;
        animator.dealDamage = false;
        yield return null;
    }

}
