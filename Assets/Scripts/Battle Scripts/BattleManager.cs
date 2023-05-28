/*
Battle Manager
Used on:    ---
For:    Overarching manager for turns, states, and actions in battle
*/

//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ref

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;

    // The booleans, integers, and enumeration that control what position in the update loop we travel to
    private bool battleIntro;
    private bool battleActive;
    private bool activeCoroutine;

    private int currentTurn;

    public enum MenuStatus { Selecting, Attack, Spell, Inventory, Run, Inactive };
    MenuStatus status;

    enum EndStatus { None, Win, Loss, Run };
    EndStatus endResult;

    // The enemy in question
    string enemyName;

    float camX;
    float camZ;

    // Instances of all other necessary classes
    GameObject currentEnemy;
    GameObject[] enemies;           // ALL enemies (we need this as a tether - they all get deactivated, and this class holds onto them)
    GameObject[] battlingEnemies;   // Battling enemies (the relevant ones)
    GameObject actInds;

    PlayerStats playerStats;
    EnemyStats enemyStats;
    EnemyGroup enemyGroup;
    IndicatorAction indAction;

    EnemyRandomMovement mainEnemyRandom;
    EnemyPathMovement mainEnemyPath;

    Stats[] turnArray;          // The stats of every combatant, which get sorted by speed from greatest to least to determine turn order
    GameObject[] combatants;    // ALL combatants' GameObjects, for moving them around the field

    Rigidbody playerRb;
    Rigidbody enemyRb;

    GameObject battleUI;

    public delegate void BattleHideEnemies();
    public static event BattleHideEnemies battleHideEnemies;
    public delegate void BattleShowEnemies();
    public static event BattleShowEnemies battleShowEnemies;

    private BattleManager()
    {
        battleIntro = true;
        battleActive = false;
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

    private void OnEnable()
    {
        InventoryMenuView.onBattleInvenExit += EndTurnInventory;
    }

    private void OnDisable()
    {
        InventoryMenuView.onBattleInvenExit -= EndTurnInventory;
    }

    private void Start()
    {
        battleIntro = true;
        battleActive = false;

        actInds = GameObject.Find("Action Indicators");
        indAction = actInds.GetComponent<IndicatorAction>();

        status = MenuStatus.Inactive;
        endResult = EndStatus.None;


    }

    private void Update()
    {
        if (GameManager.Instance.isBattle())
        {
            if (battleIntro && !activeCoroutine)    // The fun intro sequence that happens when the battle begins
            {
                StopAllCoroutines();
                // Get the instances of:
                playerStats = PlayerManager.Instance.PlayerStats();   // Player stats
                enemyStats = currentEnemy.GetComponent<EnemyStats>(); // Enemy stats
                enemyGroup = currentEnemy.GetComponent<EnemyGroup>(); // The enemy group that spawns the Gang

                playerRb = PlayerManager.Instance.PlayerRigidbody();    // Get player and enemy RBs
                enemyRb = currentEnemy.GetComponent<Rigidbody>();

                if (currentEnemy.GetComponent<EnemyPathMovement>() != null)
                {
                    mainEnemyPath = currentEnemy.GetComponent<EnemyPathMovement>();
                }
                if (currentEnemy.GetComponent<EnemyRandomMovement>() != null)
                {
                    mainEnemyRandom = currentEnemy.GetComponent<EnemyRandomMovement>();
                }

                camX = (enemyRb.position.x + playerRb.position.x) / 2;  // Tell the camera where to go
                camZ = playerRb.position.z;

                battlingEnemies = new GameObject[1 + enemyGroup.numberToSpawn];  // Create list of how many foes Em is facing
                enemyStats.isBattling = true;   // We are battling the enemy we collided with
                enemyGroup.SpawnEncounter();    // Beam our new friends in if we have any
                enemies = GameObject.FindGameObjectsWithTag("Enemy");   // Get all the components in Enemies

                int i = 0;
                foreach (GameObject enemy in enemies)   // Add the active ones to their own list for ease of access
                {
                    if (enemy.GetComponent<EnemyStats>().isBattling)
                    {
                        battlingEnemies[i] = enemy;
                        i++;
                    }
                }
                battleHideEnemies?.Invoke(); // Hide everyone who isn't battling

                // Determine turn order
                turnArray = new Stats[1 + battlingEnemies.Length];
                combatants = new GameObject[1 + battlingEnemies.Length];
                i = 0;
                foreach (GameObject enemy in enemies)   // Add the active ones to their own list for ease of access
                {
                    if (enemy.GetComponent<EnemyStats>().isBattling)
                    {
                        turnArray[i] = enemy.GetComponent<EnemyStats>();
                        combatants[i] = enemy;
                        i++;
                    }
                }
                turnArray[i] = playerStats;
                combatants[i] = PlayerManager.Instance.GameObject();
                CalculateTurnOrder(ref turnArray, ref combatants);


                /*if (playerStats.GetSPD() >= enemyStats.GetSPD()) // A very basic speed check
                {
                    turnArray[0] = playerStats;
                    turnArray[1] = enemyStats;
                }
                else
                {
                    turnArray[1] = playerStats;
                    turnArray[0] = enemyStats;
                    // Oh god I'm gonna need a recursive method to sort these
                }*/

                // Alright, no more running, I need to figure this out
                // So, say I have 4 objects, 1 player stats and 3 enemy stats
                // They're all stats, so I can add all of them to this array
                // And I think I would be able to get away with just having an array of stats, EXCEPT:
                // Em is gonna be moving around a lot, which isn't necessarily a problem on its own
                // But the enemies will also move into position for melee attacks, so I need access to the enemy game objects on their turn
                // All of that to say, I should put everyone's GameObjects into an array and sort that in parallel with the stats array
                // Dual-array sorting
                // Insane

                // So, to do:
                // Make that GameObject array
                // Pass both arrays by reference to a method that sorts them
                // ...make the method that sorts them
                // From there, turnArray is already set up to govern turn order, so...yeah, that's it!


                currentTurn = 0;    // Set the current turn to 0 so the first actor goes

                StartCoroutine(DoBattleIntro());    // Use a coroutine to time visual elements (player motion, UI swap)           
            } // End of battle intro

            else if (battleActive && !activeCoroutine)  // Primary turn loop
            {
                if (!turnArray[currentTurn].getDowned()) // If the current turn taker is not downed/dead
                {
                    if (turnArray[currentTurn].name == PlayerManager.Instance.PlayerName())  // If it's the player's turn
                    {
                        if (status == MenuStatus.Inactive) // Show the action indicators
                        {
                            status = MenuStatus.Selecting;
                            StartCoroutine(indAction.DoFlashIn(true));    // Flash our action indicators in
                        }
                        else if (status == MenuStatus.Selecting) // Handles all the conditionals for choosing an action
                        {
                            indAction.enabled = true;
                            if (Input.GetButtonDown("Interact"))
                            {
                                if (indAction.GetLeadBox() == "ATK")
                                {
                                    status = MenuStatus.Attack;
                                    StartCoroutine(indAction.DoFlashOut(false));
                                }
                                else if (indAction.GetLeadBox() == "SPL")
                                {
                                    status = MenuStatus.Spell;
                                    StartCoroutine(indAction.DoFlashOut(true));
                                }
                                else if (indAction.GetLeadBox() == "ITM")
                                {
                                    status = MenuStatus.Inventory;
                                    StartCoroutine(indAction.DoFlashOut(false));
                                }
                                else if (indAction.GetLeadBox() == "RUN")
                                {
                                    status = MenuStatus.Run;
                                }
                            }
                        }
                        else if (status == MenuStatus.Attack) // If the player has chosen to attack
                        {
                            indAction.enabled = false;
                            if (Input.GetButtonDown("Inventory"))
                            {
                                StartCoroutine(indAction.DoFlashIn(false));
                                status = MenuStatus.Selecting;
                            }
                        }
                        else if (status == MenuStatus.Spell) // If the player has chosen to cast a spell
                        {
                            indAction.enabled = false;
                            
                        }
                        else if (status == MenuStatus.Inventory) // If the player has chosen to open the inventory
                        {
                            indAction.enabled = false;
                            if (Input.GetButtonDown("Inventory"))
                            {
                                StartCoroutine(indAction.DoFlashIn(false));
                                status = MenuStatus.Selecting;
                            }
                        }
                        else if (status == MenuStatus.Run) // If the player has chosen to run
                        {
                            indAction.enabled = false;
                            StartCoroutine(indAction.DoFlashOut(true));
                            endResult = EndStatus.Run;
                            battleActive = false;
                        }
                    }
                    else // If not the player (an enemy)
                    {
                        StartCoroutine(DoTurnAdvanceEnemyTemp());
                    }
                }
                else // If the one who's turn it is is dead
                {
                   // Advance the turn count I guess 
                }

            } // End of primary turn loop

            else if (!battleActive && !activeCoroutine)
            {
                if (endResult == EndStatus.Win)
                {

                }
                else if (endResult == EndStatus.Loss)
                {

                }
                else if (endResult == EndStatus.Run)
                {
                    StartCoroutine(DoBattleRun());
                }
            } // End of end conditions

        } // End of battle check
    } // End of update

    // Public methods ------------------------------------------------------
    public void SetTarget(GameObject enemy)
    {
        currentEnemy = enemy;
    }

    public float GetCamX()
    {
        return camX;
    }

    public float GetCamZ()
    {
        return camZ;
    }

    public MenuStatus GetPlayerStatus()
    {
        return status;
    }

    public string GetCurrentTurnName()
    {
        return combatants[currentTurn].name;
    }

    // Private methods -----------------------------------------------------

    private void CalculateTurnOrder(ref Stats[] stats, ref GameObject[] participants)
    {
        // sorting - ASCENDING ORDER
        // https://www.includehelp.com/cpp-programs/sort-an-array-in-ascending-order.aspx

        Stats temp;
        GameObject temp2;
        for (int i = 0; i < stats.Length; i++)
        {
            for (int j = i + 1; j < stats.Length; j++)
            {
                if (stats[i].GetSPD() < stats[j].GetSPD())
                {
                    temp = stats[i];
                    stats[i] = stats[j];
                    stats[j] = temp;

                    temp2 = participants[i];
                    participants[i] = participants[j];
                    participants[j] = temp2;
                }
            }
        }
        Debug.Log(combatants[0].name + " , SPD:" + stats[0].GetSPD());
        Debug.Log(combatants[1].name + " , SPD:" + stats[1].GetSPD());
    }

    private void CombatantDisable()
    {
        PlayerMovement playerM = PlayerManager.Instance.PlayerMovement();
        EnemyChaseMovement enemyCM = currentEnemy.GetComponent<EnemyChaseMovement>();

        playerM.enabled = false;
        enemyCM.enabled = false;
        if (mainEnemyPath != null)
        {
            mainEnemyPath.enabled = false;
        }
        if (mainEnemyRandom != null)
        {
            mainEnemyRandom.enabled = false;
        }
    }

    private void currentEnemyReenable()
    {

        if(mainEnemyPath != null)
        {
            mainEnemyPath.enabled = true;
        }
        if (mainEnemyRandom != null)
        {
            mainEnemyRandom.enabled = true;
        }

    }

    private void allEnemyReenable(GameObject visible)
    {

        foreach (GameObject enemy in enemies)
        {
            if (enemy.name != visible.name)
            {
                enemy.SetActive(true);
                //enemy.GetComponent<FadeEnemy>().FadeIn();

            }
        }
    }

    private void playerReenable()
    {
        PlayerMovement playerM = PlayerManager.Instance.PlayerMovement();

        playerM.enabled = true;
    }

    private void fadeSpawnedEnemies()
    {
        foreach (GameObject enemy in battlingEnemies)
        {
            if (!enemy.GetComponent<EnemyStats>().remainOnPlayerFlight) // Get rid of the lackeys
            {
                enemy.GetComponent<FadeEnemy>().FadeOut();
            }
        }
    }

    private void destroySpawnedEnemies()
    {
        foreach (GameObject enemy in battlingEnemies)
        {
            if (!enemy.GetComponent<EnemyStats>().remainOnPlayerFlight) // Get rid of the lackeys
            {
                Destroy(enemy);
            }
        }
    }

    private void BattleRecoil() // Launches the player and enemy up like they recoil from each other on battle start
    {
        CombatantDisable();

        if (PlayerManager.Instance.PlayerTransform().position.x <= currentEnemy.transform.position.x)    // If the player is left of the enemy
        {
            Debug.Log("Player is left of the enemy");
            playerRb.velocity = new Vector3(-10f, 3f, 0f);

            if(battlingEnemies.Length == 1)
            {
                battlingEnemies[0].GetComponent<Rigidbody>().velocity = new Vector3(10f, 3f, 0f);
            }
            else if(battlingEnemies.Length == 2)
            {
                battlingEnemies[0].GetComponent<Rigidbody>().velocity = new Vector3(10f, 3f, 3f);
                battlingEnemies[1].GetComponent<Rigidbody>().velocity = new Vector3(15f, 3f, -3f);
            }
            else if(battlingEnemies.Length == 3)
            {
                battlingEnemies[0].GetComponent<Rigidbody>().velocity = new Vector3(5f, 3f, 4f);
                battlingEnemies[1].GetComponent<Rigidbody>().velocity = new Vector3(10f, 3f, 1f);
                battlingEnemies[2].GetComponent<Rigidbody>().velocity = new Vector3(15f, 3f, -2f);
            }
        }
        else // If the player is right of the enemy
        {
            Debug.Log("Player is right of the enemy");
            playerRb.velocity = new Vector3(10f, 3f, 0f);
            enemyRb.velocity = new Vector3(-10f, 3f, 0f);
            // Nothing is currently configured to make this work
        }
    }

    private void EndTurnInventory()
    {
        Debug.Log("Successful turn end");
        StartCoroutine(DoTurnAdvanceInven());
    }

    // Coroutines --------------------------------------------
    IEnumerator DoBattleIntro()
    {
        activeCoroutine = true;

        BattleRecoil();                             // Correctly position the player and the enemy
        yield return new WaitForSeconds(.7f);       // Wait for Battle Recoil to finish
        playerRb.velocity = new Vector3(0f, 0f, 0f);
        foreach (GameObject enemy in battlingEnemies)
        {
            enemy.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        }

        if (GameManager.Instance.isInventory())
        {
            ViewManager.Show<BattleUIView>(false); // Inventory gets closed; we always return to in-game UI after a battle
        }
        else
        {
            ViewManager.Show<BattleUIView>(true);
        }
        
        battleUI = GameObject.Find("Battle UI");
        battleUI.GetComponent<FadeUI>().UIFadeIn();

        battleIntro = false;                        // Set battleIntro to false and battleActive to true 
        battleActive = true;
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoTurnAdvanceInven()
    {
        activeCoroutine = true;
        if (currentTurn != turnArray.Length - 1) // Advance the turn
        {
            currentTurn++;                  
        }
        else
        {
            currentTurn = 0;
        }
        StartCoroutine(indAction.DoFlashOutSelected());
        status = MenuStatus.Inactive;   // The player acts no more!

        activeCoroutine = false;
        yield return null;
    }
    IEnumerator DoTurnAdvanceEnemyTemp()
    {
        activeCoroutine = true;
        yield return new WaitForSeconds(5f);
        if (currentTurn != turnArray.Length - 1) // Advance the turn
        {
            currentTurn++;
        }
        else
        {
            currentTurn = 0;
        }
        status = MenuStatus.Inactive;   // The player acts no more!

        activeCoroutine = false;
        yield return null;
    }


    IEnumerator DoBattleRun()
    {
        activeCoroutine = true;

        yield return new WaitForSeconds(2f);                // Wait so the player can read the text box
        battleUI.GetComponent<FadeUI>().UIFadeOut();

        GameManager.Instance.Battle(false);                 // Tell the game manager that we're out of battle

        playerReenable();                                   // Reenable player movement
        allEnemyReenable(currentEnemy);                     // Reenable enemies that weren't in the fight
        fadeSpawnedEnemies();                               // Get rid of enemies spawned for the battle
        battleShowEnemies?.Invoke();
        yield return new WaitForSeconds(2f);                // Wait for a few moments before letting the current enemy loose again

        currentEnemyReenable();                             // Reenable the current enemy's movement, and destroy enemies spawn for the battle
        destroySpawnedEnemies();
        
        battleIntro = true;
        battleActive = false;
        activeCoroutine = false;
        mainEnemyPath = null;
        mainEnemyRandom = null;

        endResult = EndStatus.None;
        status = MenuStatus.Inactive;  

        yield return null;
    }

    // So, most of the other stuff I have that handles enemies works the way it does so that they can be re-activated remotely
    // Seeing as you can't reactivate something that isn't active from inside that thing, because it isn't active
}
