/*
Battle Manager
Used on:    ---
For:    Overarching manager for turns, states, and actions in battle
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;

    // The booleans, integers, and enumeration that control what position in the update loop we travel to
    private bool battleIntro;
    private bool battleActive;
    private bool win;
    private bool loss;
    private bool activeCoroutine;

    private int currentTurn;

    public enum MenuStatus { Selecting, Attack, Spell, Inventory, Run, Inactive};
    MenuStatus status;

    // The enemy in question
    string enemyName;

    // Instances of all other necessary classes
    GameObject currentEnemy;
    GameObject player;
    GameObject actInds;

    PlayerStats playerStats;
    EnemyStats enemyStats;
    IndicatorAction indAction;

    Stats[] turnArray;

    Rigidbody playerRb;
    Rigidbody enemyRb;

    Camera cam;
    CameraFollow camController;

    GameObject battleUI;

    private BattleManager()
    {
        battleIntro = true;
        battleActive = false;
        win = false;
        loss = false;
        activeCoroutine = false;
        currentTurn = 0;

    }

    public static BattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject managerHolder = new GameObject("[Battle Manager]");
                managerHolder.AddComponent<BattleManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        battleIntro = true;
        battleActive = false;
        cam = Camera.main;
        camController = cam.GetComponent<CameraFollow>();

        actInds = GameObject.Find("Action Indicators");
        indAction = actInds.GetComponent<IndicatorAction>();

        status = MenuStatus.Inactive;
        

    }

    private void Update()
    {
        if (GameManager.Instance.isBattle())
        {
            if (battleIntro && !activeCoroutine)    // The fun intro sequence that happens when the battle begins
            {
                    // Get the instances of:
                    player = GameObject.Find("Player");                 // Player
                    playerStats = player.GetComponent<PlayerStats>();   // Player stats
                    enemyStats = currentEnemy.GetComponent<EnemyStats>(); // Enemy stats
                    playerRb = player.GetComponent<Rigidbody>();
                    enemyRb = currentEnemy.GetComponent<Rigidbody>();

                    HideEnemies(currentEnemy);    // Hide all other enemies

                    // Determine turn order
                    turnArray = new Stats[2];  // This will make potential expansion of this system easier in the future
                    if(playerStats.GetSpeed() >= enemyStats.GetSpeed()) // A very basic speed check
                    {
                        turnArray[0] = playerStats;
                        turnArray[1] = enemyStats;
                    }
                    else
                    {
                        turnArray[1] = playerStats;
                        turnArray[0] = enemyStats;
                    }
                    currentTurn = 0;    // Set the current turn to 0 so the first actor goes
                    StartCoroutine(DoBattleIntro());    // Use a coroutine to time visual elements (player motion, UI swap)           
            } // End of battle intro

            else if (battleActive && !activeCoroutine)  // Primary turn loop
            {
                if (!turnArray[currentTurn].getDowned()) // If the current turn taker is not downed/dead
                {
                    if(turnArray[currentTurn].name == player.name)  // Currently we're only dealing with 1v1s, so that's how we'll code
                    {
                        if(status == MenuStatus.Inactive) // Show the action indicators
                        {
                            status = MenuStatus.Selecting;
                            StartCoroutine(indAction.DoFlashIn());    // Flash our action indicators in
                        }
                        else if(status == MenuStatus.Selecting) // Handles all the conditionals for choosing an action
                        {
                            indAction.enabled = true;
                            if (Input.GetButtonDown("Interact"))
                            {
                                if (indAction.GetLeadBox() == "ATK") 
                                {
                                    status = MenuStatus.Attack;
                                }
                                else if (indAction.GetLeadBox() == "SPL")
                                {
                                    status = MenuStatus.Spell;
                                }
                                else if (indAction.GetLeadBox() == "ITM")
                                {
                                    status = MenuStatus.Inventory;
                                }
                                else if (indAction.GetLeadBox() == "RUN")
                                {
                                    status = MenuStatus.Run;
                                }
                            }
                        }
                        else if(status == MenuStatus.Attack) // If the player has chosen to attack
                        {
                            indAction.enabled = false;
                            if (Input.GetButtonDown("Cancel"))
                            {
                                status = MenuStatus.Selecting;
                            }        
                        }
                        else if (status == MenuStatus.Spell) // If the player has chosen to cast a spell
                        {
                            indAction.enabled = false;
                            StartCoroutine(indAction.DoFlashOut());
                        }
                        else if (status == MenuStatus.Inventory) // If the player has chosen to open the inventory
                        {
                            indAction.enabled = false;
                            if (Input.GetButtonDown("Cancel"))
                            {
                                status = MenuStatus.Selecting;
                            }
                        }
                        else if (status == MenuStatus.Run) // If the player has chosen to run
                        {
                            indAction.enabled = false;
                            StartCoroutine(indAction.DoFlashOut());
                        }
                    }
                    else // If not the player (an enemy)
                    {

                    }
                }

            } // End of primary turn loop

            else if (!battleActive)
            {
                if (win)
                {

                }
                else if (loss)
                {

                }
            } // End of win/loss conditions

        } // End of battle check
    }

    public void SetTarget(GameObject enemy)
    {
        currentEnemy = enemy;
    }

    public MenuStatus GetPlayerStatus()
    {
        return status;
    }

    private void HideEnemies(GameObject show)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");   // Get all the components in Enemies

        foreach (GameObject enemy in enemies)
        {
            if (enemy.name != show.name)
            {
                enemy.GetComponent<FadeEnemy>().FadeOut();
                
            }
        }
    }

    private void BattleRecoil() // Launches the player and enemy up like they recoil from each other on battle start
    { 
        PlayerMovement playerM = player.GetComponent<PlayerMovement>();
        EnemyChaseMovement enemyCM = currentEnemy.GetComponent <EnemyChaseMovement>();

        playerM.enabled = false;
        enemyCM.enabled = false;
        if (currentEnemy.TryGetComponent(out EnemyPathMovement enemyPM))
        {
            enemyPM.enabled = false;
        }
        else if(currentEnemy.TryGetComponent(out EnemyRandomMovement enemyRM))
        {
            enemyRM.enabled = false;
        }

        if (player.transform.position.x <= currentEnemy.transform.position.x)    // If the player is left of the enemy
        {
            Debug.Log("Player is left of the enemy");
            playerRb.velocity = new Vector3(-10f, 3f, 0f);
            enemyRb.velocity = new Vector3(10f, 3f, 0f);
        }
        else // If the player is right of the enemy
        {
            Debug.Log("Player is right of the enemy");
            playerRb.velocity = new Vector3(10f, 3f, 0f);
            enemyRb.velocity = new Vector3(-10f, 3f, 0f);
        }
    }

    // NEED A SHOW ENEMIES METHOD
    // Need to reenable some scripts on battle end

    // Coroutines --------------------------------------------
    IEnumerator DoBattleIntro()
    {
        activeCoroutine = true;

        // TODO: Arrange the camera in a better way
        float camX = (enemyRb.position.x + playerRb.position.x) / 2;
        float camZ = playerRb.position.z;
        camController.setCamVals(camX, camZ);

        BattleRecoil();                             // Correctly position the player and the enemy
        yield return new WaitForSeconds(.7f);       // Wait for Battle Recoil to finish
        playerRb.velocity = new Vector3(0f, 0f, 0f);
        enemyRb.velocity = new Vector3(0f, 0f, 0f);

        ViewManager.Show<BattleUIView>(true);
        battleUI = GameObject.Find("Battle UI");
        battleUI.GetComponent<FadeUI>().BattleFadeIn();

        battleIntro = false;                        // Set battleIntro to false and battleActive to true 
        battleActive = true;
        activeCoroutine = false;
        yield return null;
    }
}
