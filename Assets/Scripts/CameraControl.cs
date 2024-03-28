using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour
{
    [SerializeField] GameObject upperX; // The boundary points that form the confining square of the camera
    [SerializeField] GameObject lowerX;
    [SerializeField] GameObject upperZ;
    [SerializeField] GameObject lowerZ;
    [SerializeField] bool isYBound = false;
    [SerializeField] GameObject upperY;
    [SerializeField] GameObject player; // The player, whose position is Important

    [SerializeField] float inventoryX = 3.5f;   // X, Y, and Z position modifers for camera position in inventory
    [SerializeField] float inventoryY = -6f;
    float inventoryY2 = 3f;
    [SerializeField] float inventoryZ = 8f;

    // X, Y, and Z camera position modifers (relative to the player) for different interactions in the game
    [Header("Camera Positions")]
    [SerializeField] Vector3 saveStatuePos = new Vector3(5f, -9f, 12f);
    [SerializeField] Vector3 chestPos = new Vector3(0f, -3f, 2f);
    Vector3 chestPos2 = new Vector3(0f, 2.23f, 0f);
    [SerializeField] Vector3 castleEntryPos = new Vector3(0f, -3f, 0f);
    [SerializeField] Vector3 hereStandsPos = new Vector3(0f, 12f, -8f);
    [SerializeField] Vector3 endCutscenePos = new Vector3(0, 5f, 0f);

    [Header("Camera Save Point Positions")]
    [SerializeField] Vector3 savePos0 = new Vector3(10f, 10f, 10f); // Position modifier of the camera for the save point in the test area
    [SerializeField] Vector3 savePos1 = new Vector3(5f, 10f, 5f);   // Fountain
    [SerializeField] Vector3 savePos2 = new Vector3(-7f, -5f, -30f);   // Garden
    [SerializeField] Vector3 savePos3 = new Vector3(2f, 0f, -6f);   // Terrace before towerfall
    [SerializeField] Vector3 savePos4 = new Vector3(0f, -2f, 4f);   // Before boss
    [SerializeField] Vector3 savePos5 = new Vector3(5f, 0f, -10f);  // Terrace after towerfall

    // X, Y, and Z camera angles for different interactions in the game
    [Header("Camera Angles")]
    [SerializeField] Vector3 standardAngle = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 standardAngle2 = new Vector3(-10.51f, 0f, 0f); // For rooms 9 and 12
    [SerializeField] Vector3 battleAngle = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 inventoryAngle = new Vector3(0f, 0f, 0f);
    [SerializeField] Vector3 saveStatueAngle = new Vector3(-20f, -20f, 0f);
    [SerializeField] Vector3 chestAngle = new Vector3(10f, 0f, 0f);
    Vector3 chestAngle2 = new Vector3(0f, 0f, 0f);
    [SerializeField] Vector3 castleEntryAngle = new Vector3(-60f, 0f, 0f);
    [SerializeField] Vector3 hereStandsAngle = new Vector3(20f, 0f, 0f);

    [Header("Camera Save Point Angles")]
    [SerializeField] Vector3 saveAngle0 = new Vector3(20.51f, 0f, 0f); // Angle of the camera for the save point in the test area
    [SerializeField] Vector3 saveAngle1 = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 saveAngle2 = new Vector3(6.51f, 0f, 0f);
    [SerializeField] Vector3 saveAngle3 = new Vector3(-30.51f, 0f, 0f);
    [SerializeField] Vector3 saveAngle4 = new Vector3(20.51f, 0f, 0f);
    [SerializeField] Vector3 saveAngle5 = new Vector3(10.51f, 5f, 0f);

    Transform playerTransform;

    float upperXPos;
    float lowerXPos;
    float upperZPos;
    float lowerZPos;
    float upperYPos;

    float xValue;
    float yValue;
    float zValue;               // Value of z distance between player and camera (liable to change)
    float yConstant = 6.23f;
    float yConstant2 = -2.23f; // For rooms 9 and 12
    float zConstant = 18.34f;   // The maximum distance between the player and the camera

    [SerializeField] float battleStep = .25f;    // Previously 20f; now a time (sec) value
    [SerializeField] float inventoryStep = .5f; // Previously 10f; now a time (sec) value

    float battleX;
    [SerializeField] float battleYOffset = 0f;
    float battleYOffset2 = -8.23f;
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
        GameManager.onPlaqueStateChange += SetCamHereStands;
        GateInteractable.onCastleEnter += SetCamCastleEntry;
    }

    private void OnDisable()
    {
        GameManager.onBattleStateChange -= SetCamBattle;
        GameManager.onInventoryStateChange -= SetCamInventory;
        GameManager.onSaveStatueStateChange -= SetCamSaveStatue;
        GameManager.onChestStateChange -= SetCamChest;
        GameManager.onSavePointStateChange -= SetCamSavePoint;
        GameManager.onPlaqueStateChange -= SetCamHereStands;
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
        if(isYBound)
            upperYPos = upperY.transform.position.y;

        if(SceneManager.GetActiveScene().buildIndex == 10 || SceneManager.GetActiveScene().buildIndex == 13)
        {
            standardAngle = standardAngle2;
            yConstant = yConstant2;
            inventoryY = inventoryY2;
            chestAngle = chestAngle2;
            chestPos = chestPos2;
            battleYOffset = battleYOffset2;
        }

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
        if(isYBound)
        {
            if (yValue > upperYPos)
                yValue = upperYPos;
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
            else if (currentPoint == 1)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos1.x, playerTransform.position.y + yConstant + savePos1.y, playerTransform.position.z - zConstant + savePos1.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep * 3.0f, saveAngle1));
            }
            else if (currentPoint == 2)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos2.x, playerTransform.position.y + yConstant + savePos2.y, playerTransform.position.z - zConstant + savePos2.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep * 3.0f, saveAngle2));
            }
            else if (currentPoint == 3)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos3.x, playerTransform.position.y + yConstant + savePos3.y, playerTransform.position.z - zConstant + savePos3.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep * 3.0f, saveAngle3));
            }
            else if (currentPoint == 4)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos4.x, playerTransform.position.y + yConstant + savePos4.y, playerTransform.position.z - zConstant + savePos4.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep * 3.0f, saveAngle4));
            }
            else if (currentPoint == 5)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x + savePos5.x, playerTransform.position.y + yConstant + savePos5.y, playerTransform.position.z - zConstant + savePos5.z);
                StartCoroutine(DoCamPosition(targetPos, inventoryStep * 3.0f, saveAngle5));
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

    public void SetCamEndingCutscene()
    {
        Vector3 targetPos = new Vector3(playerTransform.position.x + endCutscenePos.x, playerTransform.position.y + yConstant + endCutscenePos.y, playerTransform.position.z - zConstant + endCutscenePos.z);
        StartCoroutine(DoCamPosition(targetPos, inventoryStep * 4, standardAngle));
    }

    private void SetCamHereStands()
    {
        Debug.Log("Aw yeah");
        StopAllCoroutines();
        if (GameManager.Instance.isInteraction())
        {
            Vector3 targetPos = new Vector3(playerTransform.position.x + hereStandsPos.x, playerTransform.position.y + yConstant + hereStandsPos.y, playerTransform.position.z - zConstant + hereStandsPos.z);
            StartCoroutine(DoCamPosition(targetPos, inventoryStep * 8, hereStandsAngle));
        }
        else
        {
            CamReset(battleStep);
        }
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
