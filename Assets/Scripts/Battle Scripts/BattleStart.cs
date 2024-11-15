/*
Battle Start
Used on:    Player
For:    When the player collides with an enemy, switch the game state to "Battle"   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : MonoBehaviour
{
    //[SerializeField] GameObject enemyProper;
    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.Battle(true);  // We are now in battle, and pass along the enemy to BattleManager
            BattleManager.Instance.SetTarget(collision.gameObject);
        }  
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Battle(true);  // We are now in battle, and pass along the enemy to BattleManager
            BattleManager.Instance.SetTarget(this.gameObject);
        }
    }
}
