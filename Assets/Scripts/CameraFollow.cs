/*
Camera Follow
Used on:    Main Camera
For:    Tells the main camera to follow the player around the scene, within pre-defined and adjustable boundaries
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject upperX; // The boundary points that form the confining square of the camera
    [SerializeField] GameObject lowerX;
    [SerializeField] GameObject upperZ;
    [SerializeField] GameObject lowerZ;
    [SerializeField] GameObject player; // The player, whose x and z positions are rather important

    float upperXPos;
    float lowerXPos;
    float upperZPos;
    float lowerZPos;

    float xValue;
    float yValue;
    float zValue;               // Value of z distance between player and camera (liable to change)
    float zConstant = 15.34f;   // The maximum distance between the player and the camera

    Vector3 tempPos;

    void Start()
    {
        // Potentially place something in here, because when the player enters a scene the effect will be lost
        // until they re-enter the bounding zone?
        upperXPos = upperX.transform.position.x;    // Positions of camera boundaries (won't change, don't need to be in update)
        lowerXPos = lowerX.transform.position.x;
        upperZPos = upperZ.transform.position.z;
        lowerZPos = lowerZ.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        tempPos = transform.position;

        xValue = player.transform.position.x;               // Initial position values which may or may not change every Update()
        yValue = transform.position.y;
        zValue = player.transform.position.z - zConstant; 

        if (xValue > upperXPos)  // If statements checking if the camera is trying to exit bounds which reposition it
        {
            xValue = upperXPos;
        }

        if (xValue < lowerXPos)
        {
            xValue = lowerXPos;
        }

        if (zValue > upperZPos)
        {
            zValue = upperZPos;
        }

        if (zValue < lowerZPos)
        {
            zValue = lowerZPos;
        }

        tempPos.x = xValue;
        tempPos.y = yValue;
        tempPos.z = zValue;

        transform.position = tempPos;
    }
}
