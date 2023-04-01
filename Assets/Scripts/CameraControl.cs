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

    [SerializeField] float inventoryX = 3.5f;   // X, Y, and Z position modifers for camera position in inventory
    [SerializeField] float inventoryY = -6f;
    [SerializeField] float inventoryZ = 8f;

    [SerializeField] float saveStatueX = 3.5f;   // X, Y, and Z position modifers for camera position at save statue
    [SerializeField] float saveStatueY = -6f;
    [SerializeField] float saveStatueZ = 8f;

    // Camera angles (all X axis angles, camera shouldn't ever need to rotate along y or z)
    [SerializeField] float standardAngle0 = 20.51f;  // The regular angle for gameplay; where the camera will be most of the time
    [SerializeField] float inventoryAngle = 0f;     // The angle the camera goes to when viewing the inventory
    [SerializeField] float saveStatueAngle = -20f;

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
        Vector3 targetPos = new Vector3(playerTransform.position.x + saveStatueX, playerTransform.position.y + yConstant + saveStatueY, playerTransform.position.z - zConstant + saveStatueZ);
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
        StartCoroutine(DoCamPosition(targetPos, step, standardAngle0));
    }

    IEnumerator DoCamBattle()
    {
        yield return new WaitForEndOfFrame();   // Why do I have this?
        battleX = BattleManager.Instance.GetCamX();
        battleZ = BattleManager.Instance.GetCamZ() - zConstant;
        Vector3 targetPos = new Vector3(battleX, yValue, battleZ);
        Debug.Log("Battle X: " + battleX + " Battle Z: " + battleZ);
        StartCoroutine(DoCamPosition(targetPos, battleStep, standardAngle0));
        yield return null;
    }

    IEnumerator DoCamPosition(Vector3 targetPos, float step, float rotationTarget)
    {
        activeCoroutine = true;

        Vector3 velocity = Vector3.zero;    // Initial velocity values for the damping functions
        float xVelocity = 0f;

        while (Vector3.Distance(transform.position, targetPos) >= .05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, step); // Move camera position

            float xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x, rotationTarget, ref xVelocity, step);
            transform.eulerAngles = new Vector3(xAngle, 0f, 0f);    // Change camera rotation
            yield return null;
        }
        activeCoroutine = false;
        yield return null;
    }

    // So, problem. In these instances:
        // Player closes inventory, decides halfway through the zoomout they want to open it again
        // Player escapes a battle, but gets into another one before the camera finishes zooming out
        // Actually, hold up, lemme program in rotation and add a camera angle to the save statue so I can
        // have an actual full list of all these problems
    // Really, all I need to do is kill the current coroutine and start the new one
    // I just need a way to track which coroutine is active so I know how to kill it
}
