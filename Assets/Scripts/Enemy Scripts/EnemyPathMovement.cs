/*
Enemy Path Movement
Used on:    Enemies
For:    Behavior of patroling between a given set of waypoints
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathMovement : MonoBehaviour
{
    // This script goes on the actual enemy themselves
    [SerializeField] GameObject[] waypoints;
    EnemyFollow followCheck;
    EnemyChaseMovement chaseMovement;
    Rigidbody rb;

    int currentWaypointIndex = 0;
    [SerializeField] float movementSpeed = 6f;
    bool active;

    private void Start()
    {
        followCheck = GetComponent<EnemyFollow>();    
        chaseMovement = GetComponent<EnemyChaseMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (followCheck == null)  // This bool allows us to use this script with or without the enemy chasing the player
        {
            active = false;
        }
        else
        {
            active = followCheck.isFollow();
        }
        

        if (!active && GameManager.Instance.canMove()) 
        {
            // First, check if we're at a waypoint (that means we need to change target)
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < .1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, movementSpeed * Time.deltaTime);
            // Time.deltaTime is the amount of time passed since the last frame, allows for framerate independence
        }
        else if(active && GameManager.Instance.canMove()) // If chase has been enabled
        {
            chaseMovement.enabled = true;
            this.enabled = false;
        }
        else
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
        }

    }

}

