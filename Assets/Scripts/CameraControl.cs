using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] GameObject upperX; // The boundary points that form the confining square of the camera
    [SerializeField] GameObject lowerX;
    [SerializeField] GameObject upperZ;
    [SerializeField] GameObject lowerZ;
    [SerializeField] GameObject player; // The player, whose x and z positions are rather important

    [SerializeField] float inventoryX = 3.5f;
    [SerializeField] float inventoryY = -2.7f;
    [SerializeField] float inventoryZ = 8f;

    Transform playerTransform;

    float upperXPos;
    float lowerXPos;
    float upperZPos;
    float lowerZPos;

    float xValue;
    float yValue;
    float zValue;               // Value of z distance between player and camera (liable to change)
    float yConstant = 6.23f;
    float zConstant = 18.34f;   // The maximum distance between the player and the camera

    [SerializeField] float battleStep = 20f;
    [SerializeField] float inventoryStep = 10f;

    float battleX;
    float battleZ;

    Vector3 tempPos;

    bool activeCoroutine;
    bool activeInvSwitch;  // Ensures that perspective shifts for battle and such are only done once
    bool activeBattleSwitch;

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

    void Update()
    {
        if(GameManager.Instance.freeCam() && (activeInvSwitch || activeBattleSwitch))
        {
            CamReset(battleStep);
            activeInvSwitch = false;
            activeBattleSwitch = false;
        }

        else if (GameManager.Instance.freeCam() && !activeCoroutine) // The code which allows the camera to track the player
        {
            playerTransform = PlayerManager.Instance.PlayerTransform();
            tempPos = transform.position;

            xValue = playerTransform.position.x;               // Initial position values which may or may not change every Update()
            yValue = playerTransform.position.y + yConstant;
            zValue = playerTransform.position.z - zConstant;

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

        else if (GameManager.Instance.isBattle() && !activeBattleSwitch)
        {
            SetCamBattle();
            activeBattleSwitch = true;
            activeInvSwitch = false;
        }
        else if (GameManager.Instance.isInventory() && !activeInvSwitch)
        {
            SetCamInventory();
            activeInvSwitch = true;
            activeBattleSwitch = false;
        }
    } // End of update

    private void SetCamBattle()
    {
        StartCoroutine(DoCamBattle());
    }

    private void SetCamInventory()
    {
        //Vector3 targetPos = new Vector3(xValue + 5f, yValue, zValue + 2f);

        Vector3 targetPos = new Vector3(playerTransform.position.x + inventoryX, playerTransform.position.y + yConstant + inventoryY, playerTransform.position.z - zConstant + inventoryZ);
        StartCoroutine(DoCamPosition(targetPos, inventoryStep));
    }

    private void CamReset(float step)
    {
        xValue = playerTransform.position.x;               // Initial position values which may or may not change every Update()
        yValue = playerTransform.position.y + yConstant;
        zValue = playerTransform.position.z - zConstant;

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

        Vector3 targetPos = new Vector3(xValue, yValue, zValue);
        StartCoroutine(DoCamPosition(targetPos, step));
    }

    IEnumerator DoCamBattle()
    {
        yield return new WaitForEndOfFrame();
        battleX = BattleManager.Instance.GetCamX();
        battleZ = BattleManager.Instance.GetCamZ() - zConstant;
        Vector3 targetPos = new Vector3(battleX, yValue, battleZ);
        //Debug.Log("Battle X: " + battleX + " Battle Z: " + battleZ);
        StartCoroutine(DoCamPosition(targetPos, battleStep));
        yield return null;
    }

    IEnumerator DoCamPosition(Vector3 targetPos, float step)
    {
        activeCoroutine = true;

        //Debug.Log("Target X: " + targetPos.x + " Target Z: " + targetPos.z);

        while (Vector3.Distance(transform.position, targetPos) > .05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step * Time.deltaTime);
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }
}
