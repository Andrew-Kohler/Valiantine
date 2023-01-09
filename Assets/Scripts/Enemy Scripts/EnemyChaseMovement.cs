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
        player = GameObject.Find("Player");
    }

    
    void Update()
    {
        if (chase.isFollow() && GameManager.Instance.canMove())   // If we are actively pursuing the player
        { //  && GameManager.Instance.canMove()
            pursuitVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, pursuitVector, speed * Time.deltaTime);
        }
    }
}
