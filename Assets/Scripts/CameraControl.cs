using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] GameObject upperX; // The boundary points that form the confining square of the camera
    [SerializeField] GameObject lowerX;
    [SerializeField] GameObject upperZ;
    [SerializeField] GameObject lowerZ;
    [SerializeField] GameObject player; // The player, whose position

    [SerializeField] float inventoryX = 3.5f;   // X, Y, and Z position modifers for camera position in inventory
    [SerializeField] float inventoryY = -6f;
    [SerializeField] float inventoryZ = 8f;

    // X, Y, and Z camera position modifers (relative to the player) for different interactions in the game
    [SerializeField] Vector3 saveStatuePos = new Vector3(5f, -9f, 12f);

    // X, Y, and Z camera angles for different interactions in the game
    [SerializeField] Vector3 standardAngle = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 inventoryAngle = new Vector3(0f, 0f, 0f);
    [SerializeField] Vector3 saveStatueAngle = new Vector3(-20f, -20f, 0f);

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

    [SerializeField] float battleStep = .25f;    // Previously 20f; now a time (sec) value
    [SerializeField] float inventoryStep = .5f; // Previously 10f; now a time (sec) value

    float battleX;
    float battleZ;

    Vector3 tempPos;

    bool activeCoroutine;
    bool activeInvSwitch;  // Ensures that perspective shifts for battle and such are only done once
    bool activeBattleSwitch;
    bool activeSaveSwitch;

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
        if(GameManager.Instance.freeCam() && (activeInvSwitch || activeBattleSwitch || activeSaveSwitch))
        {
            StopAllCoroutines();
            CamReset(battleStep);
            activeInvSwitch = false;
            activeBattleSwitch = false;
            activeSaveSwitch = false;
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

        // Oh, crimminey biscuits. These...yeah, these would go great with events.
        // Stuff it Andrew, "event refactor" is a DIFFERENT CARD.
        else if (GameManager.Instance.isBattle() && !activeBattleSwitch)
        {
            StopAllCoroutines();
            SetCamBattle();
            activeBattleSwitch = true;
            activeInvSwitch = false;
            activeSaveSwitch = false;
        }
        else if (GameManager.Instance.isInventory() && !activeInvSwitch)
        {
            StopAllCoroutines();
            SetCamInventory();
            activeInvSwitch = true;
            activeBattleSwitch = false;
            activeSaveSwitch = false;
        }
        else if(GameManager.Instance.isInteraction() && GameManager.Instance.getCurrentInteractable().CompareTag("Save Statue") && !activeSaveSwitch)
        {
            StopAllCoroutines();
            SetCamSaveStatue();
            activeSaveSwitch = true;
            activeBattleSwitch = false;
            activeInvSwitch = false;
        }
    } // End of update

    private void SetCamBattle()
    {
        StartCoroutine(DoCamBattle());
    }

    private void SetCamInventory()
    {
        Vector3 targetPos = new Vector3(playerTransform.position.x + inventoryX, playerTransform.position.y + yConstant + inventoryY, playerTransform.position.z - zConstant + inventoryZ);
        StartCoroutine(DoCamPosition(targetPos, inventoryStep, inventoryAngle));
    }

    private void SetCamSaveStatue()
    {
        Vector3 targetPos = new Vector3(playerTransform.position.x + saveStatuePos.x, playerTransform.position.y + yConstant + saveStatuePos.y, playerTransform.position.z - zConstant + saveStatuePos.z);
        StartCoroutine(DoCamPosition(targetPos, inventoryStep, saveStatueAngle));
    }

    private void SetCamBossBattle() // How I hope for the day I get to dust you off, love.
    {

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
        StartCoroutine(DoCamPosition(targetPos, step, standardAngle));
    }

    IEnumerator DoCamBattle()
    {
        yield return new WaitForEndOfFrame();   // Why do I have this?
        battleX = BattleManager.Instance.GetCamX();
        battleZ = BattleManager.Instance.GetCamZ() - zConstant;
        Vector3 targetPos = new Vector3(battleX, yValue, battleZ);
        Debug.Log("Battle X: " + battleX + " Battle Z: " + battleZ);
        StartCoroutine(DoCamPosition(targetPos, battleStep, standardAngle));
        yield return null;
    }

    IEnumerator DoCamPosition(Vector3 targetPos, float step, Vector3 targetRotation)
    {
        activeCoroutine = true;

        float xAngle;
        float yAngle;
        float zAngle;

        Vector3 velocity = Vector3.zero;    // Initial velocity values for the damping functions
        float xVelocity = 0f;
        float yVelocity = 0f;
        float zVelocity = 0f;

        while (Vector3.Distance(transform.position, targetPos) >= .05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, step); // Move camera position

            xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRotation.x, ref xVelocity, step);
            yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.y, ref yVelocity, step);
            zAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation.z, ref zVelocity, step);

            transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);    // Change camera rotation
            yield return null;
        }
        activeCoroutine = false;
        yield return null;
    }

}
