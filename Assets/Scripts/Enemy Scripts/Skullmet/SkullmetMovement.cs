/*
Skullmet Movement
Used on:    Skullmet
For:    A specialized movement script for the Skullmet; uses same components as original movement
        setup for enemies, but w/o need to turn scripts on and off
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullmetMovement : EnemyMovement
{
    [SerializeField] GameObject[] waypoints;    // The waypoints the Skullmet travels between
    //[SerializeField] float normalMovementSpeed = 170f;  // The speed of the enemy
    //[SerializeField] float chaseMovementSpeed = 374f;   // The chase speed of the enemy

    //private EnemyFollow enemyFollow;                    // Our script that checks if we should be chasing the player
    //private Rigidbody rb;

    int currentWaypointIndex = 0;
    bool active = false;
    public bool ChasingPlayer => active;
    bool movementEnabled = true;
    Vector3 pursuitVector;

    //Vector3 direction;

    /*void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyFollow = GetComponent<EnemyFollow>();   
    }*/

    void Update()
    {
        active = enemyFollow.isFollow();    // Are we following the player or not?

        if (!active && GameManager.Instance.enemyCanMove()) // If we are not chasing the player
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

            direction = (waypoints[currentWaypointIndex].transform.position - this.transform.position).normalized * normalMovementSpeed;
        }
        else if (active && GameManager.Instance.enemyCanMove()) // If chase has been enabled
        {
           Transform playerTransform = PlayerManager.Instance.PlayerTransform();
            pursuitVector = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);

            direction = (pursuitVector - this.transform.position).normalized * chaseMovementSpeed;
        }
/*        else    // If enemies can't move, freeze them
        {
            direction = new Vector3(0f, 0f, 0f);
        }*/
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enemyCanMove()) // If we are not chasing the player
        {
            if (movementEnabled)
            {
                rb.velocity = direction * Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity = new Vector3(0f, 0f, 0f);
            }

        }
        /*else
        {
            rb.velocity = direction * Time.fixedDeltaTime;
        }*/
    }

    // Public methods
    public void MovementToggle(bool enabled)    // Toggles the overworld movement on and off
    {
        movementEnabled = enabled;
    }
}
