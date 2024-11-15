/*
Enemy Chase Movement
Used on:    Enemies
For:    Behavior of chasing the player (switched to if player is close) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseMovement : MonoBehaviour
{
    EnemyFollow chase;
    GameObject player;
    Vector3 pursuitVector;

    float speed = 6f;

    void Start()
    {
        chase = GetComponent<EnemyFollow>();    // previously in children
        //player = GameObject.Find("Player");
        enabled = false;
    }

    
    void Update()
    {
        if (chase.isFollow() && GameManager.Instance.enemyCanMove())   // If we are actively pursuing the player
        {
            Transform playerTransform = PlayerManager.Instance.PlayerTransform();
            pursuitVector = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, pursuitVector, speed * Time.deltaTime);
        }
        else // If we aren't chasing, we revert back to base behavior
        {
            if (TryGetComponent(out EnemyPathMovement enemyPM))
            {
                enemyPM.enabled = true;
            }
            else if (TryGetComponent(out EnemyRandomMovement enemyRM))
            {
                enemyRM.enabled = true;
            }
            enabled = false;
        }
    }
}
