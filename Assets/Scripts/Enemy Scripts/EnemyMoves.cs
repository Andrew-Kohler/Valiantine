using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoves : MonoBehaviour
{
    protected EnemyStats enemyStats;
    protected EnemyAnimatorS enemyAnimatorS;

    public int Move1LVLCap; // What level the enemy must be to use these moves
    public int Move2LVLCap;
    public int Move3LVLCap;
    public int Move4LVLCap;

    public bool moveInProgress; // Allows us to halt the battle manager until a move is done

    public delegate void DamagePlayer(int dmgTaken);
    public static event DamagePlayer damagePlayer;

    protected void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyAnimatorS = GetComponentInChildren<EnemyAnimatorS>();
    }

    // And then, 4 virtual methods representing 4 moves
    public void Move1(PlayerStats playerStats)
    {
        StartCoroutine(DoMove1(playerStats));
    }

    public void Move2(PlayerStats playerStats)
    {
        StartCoroutine(DoMove2(playerStats));
    }

    public void Move3(PlayerStats playerStats)
    {
        StartCoroutine(DoMove3(playerStats));
    }

    public void Move4(PlayerStats playerStats)
    {
        StartCoroutine(DoMove4(playerStats));
    }

    // Coroutines -----------------------------------------------
    protected virtual IEnumerator DoMove1(PlayerStats playerStats)
    {
        yield return null;
    }

    protected virtual IEnumerator DoMove2(PlayerStats playerStats)
    {
        yield return null;
    }

    protected virtual IEnumerator DoMove3(PlayerStats playerStats)
    {
        yield return null;
    }

    protected virtual IEnumerator DoMove4(PlayerStats playerStats)
    {
        yield return null;
    }
}
