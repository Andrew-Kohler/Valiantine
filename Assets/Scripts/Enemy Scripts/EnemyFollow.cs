/*
Enemy Follow
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

    private bool follow;
    [SerializeField] private int sightRadius;   // The radius a player needs to enter to be seen
    [SerializeField] private bool altBounds;
    void Start()
    {
        if (altBounds)
        {
            upperX = GameObject.Find("Upper XE (1)").transform; // Locate the boundaries the enemies can operate in
            lowerX = GameObject.Find("Lower XE (1)").transform;
            upperZ = GameObject.Find("Upper ZE (1)").transform;
            lowerZ = GameObject.Find("Lower ZE (1)").transform;
        }
        else
        {
            upperX = GameObject.Find("Upper XE").transform; 
            lowerX = GameObject.Find("Lower XE").transform;
            upperZ = GameObject.Find("Upper ZE").transform;
            lowerZ = GameObject.Find("Lower ZE").transform;
        }
        

        follow = false;
    }

    private void Update()
    {
        if (PlayerManager.Instance != null)
        {
            if (Vector3.Distance(transform.position, PlayerManager.Instance.PlayerTransform().position) <= sightRadius && inBounds() && playerInBounds())
            {
                follow = true;
            }
            else
            {
                follow = false;
            }
        }
        
    }

    public bool inBounds()    // Checks if we are inside acceptable boundaries
    {

        if (transform.position.x >= upperX.position.x || transform.position.x <= lowerX.position.x)
        {
            return false;
        }
        if (transform.position.z >= upperZ.position.z || transform.position.z <= lowerZ.position.z)
        {
            return false;
        }
        return true;
    }

    public bool playerInBounds()
    {
        if (PlayerManager.Instance.PlayerTransform().position.x >= upperX.position.x || PlayerManager.Instance.PlayerTransform().position.x <= lowerX.position.x)
        {
            return false;
        }
        if (PlayerManager.Instance.PlayerTransform().position.z >= upperZ.position.z || PlayerManager.Instance.PlayerTransform().position.z <= lowerZ.position.z)
        {
            return false;
        }
        return true;
    }

    public bool isFollow()
    {
        return follow;
    }

}
