using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] GameObject upperX; // The boundary points that form the confining square of the camera
    [SerializeField] GameObject lowerX;
    [SerializeField] GameObject upperZ;
    [SerializeField] GameObject lowerZ;
    [SerializeField] GameObject player; // The player, whose position is Important

    [SerializeField] float inventoryX = 3.5f;   // X, Y, and Z position modifers for camera position in inventory
    [SerializeField] float inventoryY = -6f;
    [SerializeField] float inventoryZ = 8f;

    // X, Y, and Z camera position modifers (relative to the player) for different interactions in the game
    [SerializeField] Vector3 saveStatuePos = new Vector3(5f, -9f, 12f);
    [SerializeField] Vector3 chestPos = new Vector3(0f, -3f, 2f);
    [SerializeField] Vector3 castleEntryPos = new Vector3(0f, -3f, 0f);

    [SerializeField] Vector3 savePos0 = new Vector3(10f, 10f, 10f);   // Position modifier of the camera for the save point in the test area

    // X, Y, and Z camera angles for different interactions in the game
    [SerializeField] Vector3 standardAngle = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 battleAngle = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 inventoryAngle = new Vector3(0f, 0f, 0f);
    [SerializeField] Vector3 saveStatueAngle = new Vector3(-20f, -20f, 0f);
    [SerializeField] Vector3 chestAngle = new Vector3(10f, 0f, 0f);
    [SerializeField] Vector3 castleEntryAngle = new Vector3(-60f, 0f, 0f);
    

    [SerializeField] Vector3 saveAngle0 = new Vector3(20.51f, 0f, 0f); // Angle of the camera for the save point in the test area

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
    [SerializeField] float battleYOffset = 0f;
    float battleZ;
    [SerializeField] float battleZOffset = 0f;

    Vector3 tempPos;

    bool activeCoroutine;

    private void OnEnable()
    {
        GameManager.onBattleStateChange += SetCamBattle;
        GameManager.onInventoryStateChange += SetCamInventory;
        GameManager.onSaveStatueStateChange += SetCamSaveStatue; 
        GameManager.onChestStateChange += SetCamChest;
        GameManager.onSavePointStateChange += SetCamSavePoint;
        GateInteractable.onCastleEnter += SetCamCastleEntry;
    }

    private void OnDisable()
    {
        GameManager.onBattleStateChange -= SetCamBattle;
        GameManager.onInventoryStateChange -= SetCamInventory;
        GameManager.onSaveStatueStateChange -= SetCamSaveStatue;
        GameManager.onChestStateChange -= SetCamChest;
        GameManager.onSavePointStateChange -= SetCamSavePoint;
        GateInteractable.onCastleEnter -= SetCamCastleEntry;
    }

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
        // The code which allows the camera to track the player
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

        // We always want this calculation running (it's used to reset the camera), but we only want to apply it
        // when freeCam is on

        if (GameManager.Instance.freeCam() && !activeCoroutine) 
        {
            transform.position = tempPos;
        }
    } // End of update

    private void SetCamBattle()
    {
        StopAllCoroutines();
        if (GameManager.Instance.isBattle())
        {
            StartCoroutine(DoCamBattle());
        }
        else
        {
            CamReset(battleStep);
        }
        
        
    }

    private void SetCamInventory()
    {
        StopAllCoroutines();
        if (!GameManager.Instance.isBattle())
        {
            if (GameManager.Instance.isInventory())
            {
                // This only happens if we're looking at the inventory when we aren't in battle
                // Otherwise, we keep the battle camera angle in battle, which is great
                Vector3 targetPos = new Vector3(playerTransform.position.x + inventoryX, playerTransform.position.y + yConstant + inventoryY, playerTransform.position.z - zConstant + inventoryZ);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep, inventoryAngle));
            }
            else
            {
                CamReset(battleStep);
            }
        }
        
        
    }

    private void SetCamSaveStatue()
    {
        StopAllCoroutines();
        if (GameManager.Instance.isInteraction())
        {
            Vector3 targetPos = new Vector3(playerTransform.position.x + saveStatuePos.x, playerTransform.position.y + yConstant + saveStatuePos.y, playerTransform.position.z - zConstant + saveStatuePos.z);
            StartCoroutine(DoCamPosition(targetPos, inventoryStep, saveStatueAngle));
        }
        else
        {
            CamReset(battleStep);
        }
        
    }

    private void SetCamChest() 
    {
        StopAllCoroutines();
        if (GameManager.Instance.isInteraction() && !GameManager.Instance.getCurrentInteractable().GetComponent<ChestInteractable>().Opened)
        {
            Vector3 targetPos = new Vector3(playerTransform.position.x + chestPos.x, playerTransform.position.y + yConstant + chestPos.y, playerTransform.position.z - zConstant + chestPos.z);
            StartCoroutine(DoCamPosition(targetPos, inventoryStep, chestAngle));
        }
        else
        {
            CamReset(battleStep);
        }
    }

    private void SetCamSavePoint(int currentPoint)
    {
        StopAllCoroutines();
        if (GameManager.Instance.isInteraction())
        {
            if(currentPoint == 0)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos0.x, playerTransform.position.y + yConstant + savePos0.y, playerTransform.position.z - zConstant + savePos0.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep, saveAngle0));
            }
            else
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos0.x, playerTransform.position.y + yConstant + savePos0.y, playerTransform.position.z - zConstant + savePos0.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep, saveAngle0));
            }             
        }
        else
        {
            CamReset(battleStep);
        }
    }

    private void SetCamCastleEntry()
    {
/*        StopAllCoroutines();
        if (GameManager.Instance.isInteraction() && GameManager.Instance.getCurrentInteractable().GetComponent<GateInteractable>().mainDoors )
        {*/
            Vector3 targetPos = new Vector3(playerTransform.position.x + castleEntryPos.x, playerTransform.position.y + yConstant + castleEntryPos.y, playerTransform.position.z - zConstant + castleEntryPos.z);
            StartCoroutine(DoCamPosition(targetPos, inventoryStep * 8, castleEntryAngle));
        //}
/*        else
        {
            CamReset(battleStep);
        }*/
    }

    private void SetCamBossBattle() // How I hope for the day I get to dust you off, love.
    {

    }

    private void CamReset(float step)
    {
        StartCoroutine(DoCamReset2(step, standardAngle));
    }

    IEnumerator DoCamReset2(float step, Vector3 targetRotation) // The sequel to the original camera reset method (this one is better)
    {
        activeCoroutine = true;

        float xAngle;
        float yAngle;
        float zAngle;

        Vector3 velocity = Vector3.zero;    // Initial velocity values for the damping functions
        float xVelocity = 0f;
        float yVelocity = 0f;
        float zVelocity = 0f;

        while (Vector3.Distance(transform.position, tempPos) >= .05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, tempPos, ref velocity, step); // Move camera position

            xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRotation.x, ref xVelocity, step);
            yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.y, ref yVelocity, step);
            zAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation.z, ref zVelocity, step);

            transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);    // Change camera rotation
            yield return null;
        }
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoCamBattle()
    {
        yield return new WaitForEndOfFrame();   // Why do I have this?
        battleX = BattleManager.Instance.GetCamX();
        battleZ = BattleManager.Instance.GetCamZ() - zConstant;
        Vector3 targetPos = new Vector3(battleX, yValue - battleYOffset, battleZ + battleZOffset);
        //Debug.Log("Battle X: " + battleX + " Battle Z: " + battleZ);
        StartCoroutine(DoCamPosition(targetPos, battleStep, battleAngle));
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
