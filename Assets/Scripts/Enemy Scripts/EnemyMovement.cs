using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] protected float normalMovementSpeed = 170f;  // The speed of the enemy
    [SerializeField] protected float chaseMovementSpeed = 374f;   // The chase speed of the enemy

    protected EnemyFollow enemyFollow;                    // Our script that checks if we should be chasing the player
    protected Rigidbody rb;

    protected Vector3 direction;
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyFollow = GetComponent<EnemyFollow>();
    }
}
