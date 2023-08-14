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

    void Start()
    {
        upperX = GameObject.Find("Upper XE").transform; // Locate the boundaries the enemies can operate in
        lowerX = GameObject.Find("Lower XE").transform;
        upperZ = GameObject.Find("Upper ZE").transform;
        lowerZ = GameObject.Find("Lower ZE").transform;

        follow = false;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, PlayerManager.Instance.PlayerTransform().position) <= sightRadius && inBounds())
        {
            follow = true;
        }
        else
        {
            follow = false;
        }
    }

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
