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
    float zConstant = 18.34f;   // The maximum distance between the player and the camera

    [SerializeField] float step = 20f;

    float battleX;
    float battleZ;

    Vector3 tempPos;

    bool activeCoroutine;

    void Start()
    {
        // Potentially place something in here, because when the player enters a scene the effect will be lost
        // until they re-enter the bounding zone?
        upperXPos = upperX.transform.position.x;    // Positions of camera boundaries (won't change, don't need to be in update)
        lowerXPos = lowerX.transform.position.x;
        upperZPos = upperZ.transform.position.z;
        lowerZPos = lowerZ.transform.position.z;

        activeCoroutine = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.canMove() && !activeCoroutine)
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

    public void setCamVals(float camX, float camZ)
    {
        Debug.Log("New cam vals set");
        battleX = camX;
        battleZ = camZ - zConstant;
        Debug.Log("Battle X: " + battleX + " Battle Z: " + battleZ);
        StartCoroutine(DoBattlePos());
    }

    IEnumerator DoBattlePos()
    {
        float tempStep = step;  // Preserves step through the modifications made during coroutine for slowdown
        activeCoroutine = true;
        Vector3 targetPos = new Vector3(battleX, yValue, battleZ);
        Debug.Log("Target X: " + targetPos.x + " Target Z: " + targetPos.z);
        
        while (Vector3.Distance(transform.position, targetPos) > .05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step * Time.deltaTime);  
            yield return null;
        }

        step = tempStep;
        activeCoroutine = false;
        yield return null;
    }
}

