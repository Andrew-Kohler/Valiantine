/*
Enemy Random Movement
Used on:    Enemies
For:    Detects if an enemy is trying to move out of bounds, and whether it is a valid time for an enemy to chase 
        a player
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    // This script goes on the invisible sphere around the enemy

    Transform upperX; // The boundary points that form the confining square of the enemy
    Transform lowerX;
    Transform upperZ;
    Transform lowerZ;

    Transform player;

    private bool follow;
    private int trackingDist = 10;

    void Start()
    {
        upperX = GameObject.Find("Upper XE").transform; // Locate the boundaries the enemies can operate in
        lowerX = GameObject.Find("Lower XE").transform;
        upperZ = GameObject.Find("Upper ZE").transform;
        lowerZ = GameObject.Find("Lower ZE").transform;

        player = GameObject.Find("Player").transform;

        follow = false;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) <= trackingDist && inBounds())
        {
            follow = true;
        }
        else
        {
            follow = false;
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        // When the player enters the trigger:
        // Are they within the valid zone?
        // If they are, follow is true
        // If they are not, follow is false
        if (other.gameObject.CompareTag("Player") && inBounds())
        {
            Debug.Log("Entered");
            follow = true; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // While the player is in the trigger:
        // Are they in the valid zone?
        // If they are, follow is true
        // If they are not, follow is false
        if (other.gameObject.CompareTag("Player") && inBounds())
        {
            follow = true; 
        }
        else
        {
            follow = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player leaves the trigger:
        // Stop following lol
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Left");
            follow = false;
        }
        
    }*/

    public bool inBounds()    // Checks if we are inside acceptable boundaries
    {

        if (transform.position.x >= upperX.position.x || transform.position.x <= lowerX.position.x)
        {
            //Debug.Log("No dice - x");
            return false;
        }
        if (transform.position.z >= upperZ.position.z || transform.position.z <= lowerZ.position.z)
        {
            //Debug.Log("No dice - z");
            return false;
        }
        return true;
    }

    public bool isFollow()
    {
        return follow;
    }

}
