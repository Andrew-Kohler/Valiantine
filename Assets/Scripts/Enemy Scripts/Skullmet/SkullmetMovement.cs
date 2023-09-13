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
    int currentWaypointIndex = 0;
    bool active = false;
    public bool ChasingPlayer => active;
    bool movementEnabled = true; // For making the appearance of the skull hopping
    
    public bool spawnedToFight; // For disabling the attempts of the spawned enemies to move about
    Vector3 pursuitVector;

    bool battleMovementEnabled = false;
    public bool arrived = false;
    private Vector3 idleBattlePosition; // Where the Skullmet landed when battle began
    private Vector3 orderedBattlePosition;  // Where the Skullmet is headed when ordered to move
    private float distanceFromPoint;

    void Update()
    {
        if (!spawnedToFight)
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
        }

        if (GameManager.Instance.isBattle())
        {
            if (battleMovementEnabled)  // If we are moving
            {
                if(Vector3.Distance(orderedBattlePosition, this.transform.position) < distanceFromPoint)  // If we are pretty close
                {
                    arrived = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enemyCanMove()) // If we are not in battle
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
        if (GameManager.Instance.isBattle())
        {  
            if (battleMovementEnabled)
            { 
                rb.velocity = new Vector3(direction.x, 0f, direction.z) * Time.fixedDeltaTime;
            }
            else
            {
                
                rb.velocity = new Vector3(0f, 0f, 0f);
            }
        }
    }

    // Public methods---------------------------------------------------------------
    public void MovementToggle(bool enabled)    // Toggles the overworld movement on and off
    {
        movementEnabled = enabled;
    }

    public void BattleMovementToggle(bool enabled)    // Toggles the battle movement on and off
    {
        battleMovementEnabled = enabled;
    }

    public void SetBattleIdlePosition()
    {
        idleBattlePosition = this.transform.position;
    }

    public Vector3 GetBattleIdlePosition()
    {
        return idleBattlePosition;
    }

    public void MoveToPoint(Vector3 point, float distanceFromPoint) // Assigns a point to move to in the battle
    {
        orderedBattlePosition = point;
        this.distanceFromPoint = distanceFromPoint;
        direction = (point - this.transform.position).normalized * chaseMovementSpeed;
    }
    // Coroutines --------------------------------------------------------------
}
