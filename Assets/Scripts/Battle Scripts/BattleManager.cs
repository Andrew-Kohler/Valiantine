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
    private int attackType;         // 0 = regular attack, 1 = Will spell, 2 = Courage spell
    private bool newLoop;           // A boolean for doing things once at the start of a new turn cycle
    private bool playerTurn;        // A boolean for doing things once at the start of a player's turn

    private bool cunningSecondTurn; // A boolean for applying the special effect of the Gem of Cunning
    private int patienceCounter = -1;
    public int PatienceCounter => patienceCounter;
    private int greatPatienceCounter = -1;
    public int GreatPatienceCounter => greatPatienceCounter;

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
    GameObject[] combatants;        // ALL combatants' GameObjects, for moving them around the field
    Stats[] turnArray;          // The stats of every combatant, which get sorted by speed from greatest to least to determine turn order

    GameObject actInds;
    GameObject enemySelectArrow;

    PlayerStats playerStats;
    PlayerMoves playerMoves;
    GemSystem playerGemSys;
    EnemyStats enemyStats;
    EnemyGroup enemyGroup;
    IndicatorAction indAction;

    EnemyMovement mainEnemyMovement;    // The movement script of the enemy that was in the overworld

    Rigidbody playerRb;
    Rigidbody enemyRb;

    GameObject battleUI;

    public delegate void BattleHideEnemies();
    public static event BattleHideEnemies battleHideEnemies;
    public delegate void BattleShowEnemies();
    public static event BattleShowEnemies battleShowEnemies;

    public delegate void BattleNewTurn();
    public static event BattleNewTurn battleNewTurn;
    public delegate void BattlePlayerTurn();
    public static event BattleNewTurn battlePlayerTurn;

    private BattleManager()
    {
        battleIntro = true;
        battleActive = false;
        activeCoroutine = false;
        currentTurn = 0;
        newLoop = true;
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
        PlayerStats.onDefeat += TriggerLoss;
    }

    private void OnDisable()
    {
        InventoryMenuView.onBattleInvenExit -= EndTurnInventory;
        PlayerStats.onDefeat -= TriggerLoss;
    }

    private void Start()
    {
        battleIntro = true;
        battleActive = false;
        newLoop = true;
        playerTurn = false;

        actInds = GameObject.Find("Action Indicators");
        indAction = actInds.GetComponent<IndicatorAction>();

        status = MenuStatus.Inactive;
        endResult = EndStatus.None;

        StopAllCoroutines();
        activeCoroutine = true;
        if(!PlayerManager.Instance.PlayerStats().getDowned())
            StartCoroutine(DoBattleIntro());
    }

    private void Update()
    {
        if (GameManager.Instance.isBattle())
        {
            // ----------------- The turn loop -----------------------------

            if (battleActive && !activeCoroutine)  // Primary turn loop - else if
            {
                if (CheckForWin()) // Check if player wins
                {
                    battleActive = false;
                    endResult = EndStatus.Win;
                }

                else // Actual turns going on
                {
                    if (currentTurn == 0 && newLoop) // If the current turn is 0, we're at the start of a full loop, and need to decrement some timers
                    {
                        // Send out an event so that buffs and debuffs know that it's time to check their timers
                        battleNewTurn?.Invoke();
                        // Gem of Cunning 2nd turn reset
                        if(playerGemSys.CurrentGem != null)
                        {
                            if (playerGemSys.CurrentGem.name == "Cunning")
                            {
                                cunningSecondTurn = true;
                            }
                        }
                        

                        // If the Patience timers are greater than -1, decrement them
                        // The -1 is just so that they have an idle that doesn't trigger the effects every turn
                        if(patienceCounter > -1)
                            patienceCounter--;
                        if(greatPatienceCounter > -1)
                            greatPatienceCounter--;

                        // if the Patience timers hit 0 after being decremented, apply their buffs
                        if (patienceCounter == 0)
                        {
                            playerStats.UpdateStatMods(new StatMod(1, 0, 1));
                            playerStats.UpdateStatMods(new StatMod(1, 1, 1));
                            playerStats.UpdateStatMods(new StatMod(1, 2, 1));
                        }

                        if (greatPatienceCounter == 0)
                        {
                            playerStats.UpdateStatMods(new StatMod(1, 0, 3));
                            playerStats.UpdateStatMods(new StatMod(1, 1, 3));
                            playerStats.UpdateStatMods(new StatMod(1, 2, 3));
                        }

                        
                        newLoop = false;
                    }
                    if (currentTurn == 1)
                    {
                        newLoop = true;
                    }

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
                                if (!playerTurn)
                                {
                                    playerTurn = true;
                                    //toggleStatDisplaysE(false);
                                    toggleStatDisplays(true);
                                }
                                indAction.enabled = true;
                                if (Input.GetButtonDown("Interact"))
                                {
                                    if (indAction.GetLeadBox() == "ATK")
                                    {
                                        ViewManager.GetView<BattleUIView>().setTutorialText("W & S to move the selection arrow // E to attack // Q to back out");
                                        ViewManager.GetView<BattleUIView>().setText("Which enemy will you attack?");
                                        status = MenuStatus.Attack;
                                        CreateSelectionArrow(); // Create the arrow for picking your fight

                                        StartCoroutine(indAction.DoFlashOut(false));
                                    }
                                    else if (indAction.GetLeadBox() == "SPL")
                                    {
                                        
                                        if(playerGemSys.CurrentGem.name == "Will")
                                        {
                                            if(playerStats.GetMP() >= 5)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                                CreateSelectionArrow(); // Create the arrow for picking your fight
                                                enemySelectArrow.GetComponent<TargetArrow>().type = 1;
                                                ViewManager.GetView<BattleUIView>().setText("Which enemy will you cast Heart Aflame on?");
                                                ViewManager.GetView<BattleUIView>().setTutorialText("W & S to move the selection arrow // E to attack // Q to back out");
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                           
                                        }
                                        else if(playerGemSys.CurrentGem.name == "Courage")
                                        {
                                            if (playerStats.GetMP() >= 6)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                                CreateSelectionArrow(); // Create the arrow for picking your fight
                                                enemySelectArrow.GetComponent<TargetArrow>().type = 2;
                                                ViewManager.GetView<BattleUIView>().setText("Which enemy will you cast Seize the Blade against?");
                                                ViewManager.GetView<BattleUIView>().setTutorialText("W & S to move the selection arrow // E to attack // Q to back out");
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                            
                                        }
                                        else if (playerGemSys.CurrentGem.name == "Constitution")
                                        {
                                            if (playerStats.GetMP() >= 6)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                                //ViewManager.GetView<BattleUIView>().setText("Temp - Constitution is an insta cast");
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                            
                                        }
                                        else if (playerGemSys.CurrentGem.name == "Patience")
                                        {
                                            if (playerStats.GetMP() >= 4)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                                //ViewManager.GetView<BattleUIView>().setText("Temp - Patience is an insta cast");
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                        }
                                        else if (playerGemSys.CurrentGem.name == "Great Patience")
                                        {
                                            if (playerStats.GetMP() >= 6)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                                //ViewManager.GetView<BattleUIView>().setText("Temp - GP is an insta cast");
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                            
                                        }
                                        else if (playerGemSys.CurrentGem.name == "Cunning")
                                        {
                                            if (playerStats.GetMP() >= 8)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                                //ViewManager.GetView<BattleUIView>().setText("Temp - GP is an insta cast");
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                        }
                                        else if (playerGemSys.CurrentGem.name == "Heart")
                                        {
                                            if (playerStats.GetMP() >= 4)
                                            {
                                                status = MenuStatus.Spell;
                                                StartCoroutine(indAction.DoFlashOut(true));
                                            }
                                            else
                                            {
                                                ViewManager.GetView<BattleUIView>().setText("Not enough mana for that spell!");
                                            }
                                            
                                        }

                                    }
                                    else if (indAction.GetLeadBox() == "ITM")
                                    {
                                        status = MenuStatus.Inventory;
                                        StartCoroutine(indAction.DoFlashOut(false));
                                        ViewManager.GetView<BattleUIView>().setText("Take a turn to use an item or switch out your equipped gem.");
                                    }
                                    else if (indAction.GetLeadBox() == "RUN")
                                    {
                                        ViewManager.GetView<BattleUIView>().setText("You fled the battle...");
                                        status = MenuStatus.Run;
                                    }
                                }
                            }
                            else if (status == MenuStatus.Attack) // If the player has chosen to attack
                            {
                                indAction.enabled = false;
                                if (Input.GetButtonDown("Inventory"))   // For backing out - TODO, change backout input
                                {
                                    DestroySelectionArrow();
                                    StartCoroutine(indAction.DoFlashIn(false));
                                    status = MenuStatus.Selecting;
                                }
                                // Making a choice is actually handled by the arrow itself, so I don't need an if else here

                            }
                            else if (status == MenuStatus.Spell) // If the player has chosen to cast a spell
                            {
                                indAction.enabled = false;
                                if (Input.GetButtonDown("Inventory"))   
                                {
                                    if(enemySelectArrow != null)
                                        DestroySelectionArrow();
                                    StartCoroutine(indAction.DoFlashIn(false));
                                    status = MenuStatus.Selecting;
                                }
                                // All spells should have a target enum so I know what to do here
                                if (playerGemSys.CurrentGem.name == "Will")
                                {
                                    // Will spawns an arrow - DONE
                                }
                                else if (playerGemSys.CurrentGem.name == "Courage")
                                {
                                    // Courage spawns an arrow - DONE
                                }
                                else if (playerGemSys.CurrentGem.name == "Constitution")
                                {
                                    StartCoroutine(DoTurnAdvanceSpell(null));

                                }
                                else if (playerGemSys.CurrentGem.name == "Patience")
                                {
                                    StartCoroutine(DoTurnAdvanceSpell(null));
                                }
                                else if (playerGemSys.CurrentGem.name == "Great Patience")
                                {
                                    StartCoroutine(DoTurnAdvanceSpell(null));
                                }
                                else if (playerGemSys.CurrentGem.name == "Cunning")
                                {
                                    StartCoroutine(DoTurnAdvanceSpell(null));
                                }
                                else if (playerGemSys.CurrentGem.name == "Heart")
                                {
                                    StartCoroutine(DoTurnAdvanceSpell(null));
                                }

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
                            toggleStatDisplays(false);
                            playerTurn = false;
                            StartCoroutine(DoTurnAdvanceEnemyTemp());
                        }
                    }
                    else // If the one who's turn it is is dead
                    {
                        toggleStatDisplays(false);
                        playerTurn = false;
                        if (currentTurn != turnArray.Length - 1) // Advance the turn
                        {
                            currentTurn++;
                        }
                        else
                        {
                            currentTurn = 0;
                        }
                    }
                }
                

            } // End of primary turn loop

            else if (!battleActive && !activeCoroutine)
            {
                if (endResult == EndStatus.Win)
                {
                    StartCoroutine(DoBattleWin());
                }
                else if (endResult == EndStatus.Loss)
                {
                    StartCoroutine(DoBattleLoss());
                }
                else if (endResult == EndStatus.Run)
                {
                    StartCoroutine(DoBattleRun());
                }
            } // End of end conditions

        } // End of battle check

    } // End of update

    // Public methods ------------------------------------------------------
    #region GETTERS
    public float GetCamX()  // For setting initial camera position in battle
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
        return combatants[currentTurn].GetComponent<EnemyStats>().enemyName;
    }

    public GameObject[] GetBattlingEnemies()    // For when enemies need to buff their pals <3
    {
        return battlingEnemies;
    }
    #endregion

    #region SETTERS
    public void SetTarget(GameObject enemy)
    {
        currentEnemy = enemy;
    }

    public void SetAttackTarget(GameObject targetedEnemy)   // Sets the target for the player's attack
    {
        int type = enemySelectArrow.GetComponent<TargetArrow>().type;
        DestroySelectionArrow();
        // For testing purposes, let's find a way to change the text in the battle UI
        //Debug.Log(targetedEnemy.name); 

        if(type == 0)
            StartCoroutine(DoTurnAdvancePlayerAttack(targetedEnemy));   // Pass this along to a coroutine for the player attacking

        else if(type == 1 || type == 2)
        {
            StartCoroutine(DoTurnAdvanceSpell(targetedEnemy));
            
        }
            
    }

    public void SetPatienceTimer()
    {
        patienceCounter = 2;
    }

    public void SetGreatPatienceTimer()
    {
        greatPatienceCounter = 1;
    }
    #endregion

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
    }

    private void CombatantDisable() // Disables the movement scripts on both primary combatants
    {
        PlayerMovement playerM = PlayerManager.Instance.PlayerMovement();

        playerM.enabled = false;
        mainEnemyMovement.enabled = false;
    }

    private void currentEnemyReenable()
    {
        mainEnemyMovement.enabled = true;
        enemyStats.isBattling = false;
    }

    private void allEnemyReenable(GameObject visible)
    {
        if (visible != null)
        {
            foreach (GameObject enemy in enemies)
            {
                if(enemy != null)
                {
                    if (enemy.name != visible.name)
                    {
                        enemy.SetActive(true);
                        /*if (enemy.GetComponent<BattleStart>() != null)
                        {
                            enemy.GetComponent<BattleStart>().enabled = false;
                        }*/

                        //enemy.GetComponent<FadeEnemy>().FadeIn();

                    }
                }
                
            }
        }
        else
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {

                        enemy.SetActive(true);
                        /*if (enemy.GetComponent<BattleStart>() != null)
                        {
                            enemy.GetComponent<BattleStart>().enabled = false;
                        }*/

                        //enemy.GetComponent<FadeEnemy>().FadeIn();

                }

            }
        }
        
    }

    private void allEnemyReenableBattleStart()
    {

        foreach (GameObject enemy in enemies)
        {
            if(enemy != null)
            {

                    if (enemy.GetComponent<BattleStart>() != null)
                    {
                        enemy.GetComponent<BattleStart>().enabled = true;
                    }
                    //enemy.GetComponent<FadeEnemy>().FadeIn();

                
            }
            
        }
    }

    private void playerReenable()
    {
        PlayerMovement playerM = PlayerManager.Instance.PlayerMovement();

        playerM.enabled = true;
        playerM.ForceDeactiveCoroutine();
    }

    private int totalXpGain()
    {
        int xpGain = 0;
        foreach (GameObject enemy in battlingEnemies)
        {
            xpGain += enemy.GetComponent<EnemyStats>().GetXPValue();
        }

        return (int)(xpGain * playerStats.GetXPMod());
    }

    #region END OF BATTLE ENEMY REMOVAL
    private void fadeSpawnedEnemies()
    {
        foreach (GameObject enemy in battlingEnemies)
        {
            if (!enemy.GetComponent<EnemyStats>().remainOnPlayerFlight && enemy.GetComponent<FadeEnemy>() != null) // Get rid of the lackeys
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

    private void fadeAllBattlingEnemies()
    {
        foreach (GameObject enemy in battlingEnemies)
        {
            enemy.GetComponent<FadeEnemy>().FadeOut();

        }
    }

    private void destroyAllBattlingEnemies()
    {
        foreach (GameObject enemy in battlingEnemies)
        {
                Destroy(enemy);
            
        }
    }
    #endregion

    private void toggleStatDisplays(bool on) // Turns buff and debuff displays on and off
    {
        foreach(GameObject combatant in combatants)
        {
            if (combatant.GetComponent<StatModVisualController>() != null && !combatant.GetComponent<Stats>().getDowned())
                combatant.GetComponent<StatModVisualController>().SetPlayerTurn(on);
        }
    }

/*    private void toggleStatDisplaysE(bool on) // Turns buff and debuff displays on and off
    {
        foreach (GameObject combatant in combatants)
        {
            if (combatant.GetComponent<StatModVisualController>() != null)
                combatant.GetComponent<StatModVisualController>().SetEnemyTurn(on);
        }
    }*/

    private void clearStatMods()
    {
        foreach (Stats stats in turnArray)
        {
            if (stats != null)
                stats.ClearStatMods();
        }
    }

    private void BattleRecoil() // Launches the player and enemy up like they recoil from each other on battle start
    {
        CombatantDisable();
        playerRb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        if (PlayerManager.Instance.PlayerTransform().position.x <= currentEnemy.transform.position.x)    // If the player is left of the enemy
        {
            //Debug.Log("Player is left of the enemy");
            PlayerManager.Instance.GetComponentInChildren<PlayerAnimatorS>().PlayBattleEnter(true);

            if (battlingEnemies.Length == 1)
            {
                battlingEnemies[0].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(3, true);
            }
            else if(battlingEnemies.Length == 2)
            {
                battlingEnemies[0].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(4, true);
                battlingEnemies[1].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(2, true);
            }
            else if(battlingEnemies.Length == 3)
            {
                battlingEnemies[0].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(3, true);
                battlingEnemies[1].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(5, true);
                battlingEnemies[2].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(1, true);
            }
        }
        else // If the player is right of the enemy
        {
            //Debug.Log("Player is right of the enemy");
            PlayerManager.Instance.GetComponentInChildren<PlayerAnimatorS>().PlayBattleEnter(false);
            if (battlingEnemies.Length == 1)
            {
                battlingEnemies[0].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(3, false);
            }
            else if (battlingEnemies.Length == 2)
            {
                battlingEnemies[0].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(4, false);
                battlingEnemies[1].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(2, false);
            }
            else if (battlingEnemies.Length == 3)
            {
                battlingEnemies[0].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(3, false);
                battlingEnemies[1].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(5, false);
                battlingEnemies[2].GetComponentInChildren<EnemyAnimatorS>().PlayBattleEntrance(1, false);
            }
        }
    }

    private void EndTurnInventory()
    {
        StartCoroutine(DoTurnAdvanceInven());
    }

    private void TriggerLoss()
    {
        battleActive = false;
        endResult = EndStatus.Loss;

        StartCoroutine(DoBattleLoss());
    }

    private void CreateSelectionArrow() // Creates an arrow to let the player select which enemy to attack
    {
        int enemyNum = 0;
        if (battlingEnemies[0].GetComponent<Stats>().getDowned()) // This works because the player will never be able to attempt a bad selection after they kill the last enemy
        {
            if (battlingEnemies[1].GetComponent<Stats>().getDowned())
            {
                enemyNum = 2;
            }
            else
            {
                enemyNum = 1;
            }
        }
        enemySelectArrow = Instantiate((GameObject)Resources.Load("Target Arrow"), new Vector3(battlingEnemies[enemyNum].transform.position.x, battlingEnemies[enemyNum].transform.position.y + 2f, battlingEnemies[enemyNum].transform.position.z - .01f), Quaternion.identity);
        enemySelectArrow.GetComponent<TargetArrow>().SetValues(battlingEnemies, enemyNum);
    }

    private void DestroySelectionArrow() // Destroys said arrow
    {
        Destroy(enemySelectArrow);
    }

    private GameObject GetFirstAliveEnemy()
    {
        GameObject livingEnemy = null;
        foreach(GameObject enemy in battlingEnemies)
        {
            if (!enemy.GetComponent<Stats>().getDowned())
            {
                livingEnemy = enemy;
                break;
            }
        }
        return livingEnemy;
    }

    private bool CheckForWin()
    {
        foreach (GameObject enemy in battlingEnemies)
        {
            if (!enemy.GetComponent<EnemyStats>().getDowned()) // If even one enemy is still standing, we haven't won yet
            {
                return false;
            }
        }
        return true;
    }

    /*private bool CheckForLoss()
    {

    }*/

    // Coroutines --------------------------------------------
    IEnumerator DoBattleIntro()
    {
        activeCoroutine = true;
        playerStats = PlayerManager.Instance.PlayerStats();                 // Player stats
        playerMoves = playerStats.gameObject.GetComponent<PlayerMoves>();   // Player moves
        playerGemSys = playerStats.gameObject.GetComponent<GemSystem>();

        enemyStats = currentEnemy.GetComponent<EnemyStats>(); // Enemy stats
        enemyGroup = currentEnemy.GetComponent<EnemyGroup>(); // The enemy group that spawns the Gang

        playerRb = PlayerManager.Instance.PlayerRigidbody();    // Get player and enemy RBs
        enemyRb = currentEnemy.GetComponent<Rigidbody>();

        mainEnemyMovement = currentEnemy.GetComponent<EnemyMovement>();

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
        foreach (GameObject enemy in enemies)   // Add all the battling enemies' stats to turn array
        {
            if (enemy.GetComponent<EnemyStats>().isBattling)
            {
                turnArray[i] = enemy.GetComponent<EnemyStats>();
                turnArray[i].SetLVL(enemyGroup.additionalEnemyLevels[i]);
                combatants[i] = enemy;
                i++;
            }
        }
        turnArray[i] = playerStats;
        combatants[i] = PlayerManager.Instance.GameObject();
        CalculateTurnOrder(ref turnArray, ref combatants);
        currentTurn = 0;    // Set the current turn to 0 so the first actor goes
        // -----------------------------------------------------------------------------------------------
        MusicBox.Instance.StartBattleFade();
        BattleRecoil();                             // Correctly position the player and the enemy
        yield return new WaitForSeconds(.7f);       // Wait for Battle Recoil to finish
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
        //playerRb.velocity = new Vector3(0f, 0f, 0f);// TODO: This will eventually go in the player animator

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
        ViewManager.GetView<BattleUIView>().setTutorialText("A & D to choose an action // E to select an action // Q to back out of an action");

        PlayerManager.Instance.PlayerMovement().ZeroOutMovement();
        battleIntro = false;                        // Set battleIntro to false and battleActive to true 
        battleActive = true;
        activeCoroutine = false;
        yield return null;
    }

    #region PLAYER TURN COROUTINES

    IEnumerator DoTurnAdvancePlayerAttack(GameObject targetedEnemy) // The sequence wherein the player attacks an enemy
    {
        activeCoroutine = true;
        toggleStatDisplays(false);
        StartCoroutine(indAction.DoFlashOutSelected()); // Flash out the attack indicator

        // TODO
        // The text box fades out and the HP/MP bar lowers, letting us get a better look at the action

        playerMoves.Attack(targetedEnemy);
        ViewManager.GetView<BattleUIView>().setText("You leap into the fray against " + targetedEnemy.GetComponent<EnemyStats>().enemyName +"!");
        ViewManager.GetView<BattleUIView>().setTutorialText("");
        yield return new WaitUntil(() => combatants[currentTurn].GetComponent<PlayerMoves>().moveInProgress == false);

        // The text box returns as the HP/MP bar comes back up
        // Is anyone still alive? 
        // If yes, advance the turn
        // If no, we win! Enemies are faded out and destroyed, we get XP, all that jazz.

        yield return new WaitForSeconds(.5f);

        if (cunningSecondTurn)
        {
            cunningSecondTurn = false;
        }
        else
        {
            if (currentTurn != turnArray.Length - 1) // Advance the turn
            {
                currentTurn++;
            }
            else
            {
                currentTurn = 0;
            }
        }
        

        status = MenuStatus.Inactive;  // Get rid of the menu
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoTurnAdvanceSpell(GameObject targetedEnemy)
    {
        activeCoroutine = true;
        toggleStatDisplays(false);
        StartCoroutine(indAction.DoFlashOutSelected()); // Flash out the attack indicator
        ViewManager.GetView<BattleUIView>().setTutorialText("");
        // Starting bit ^ ------------------------------------------------------------
        if (playerGemSys.CurrentGem.name == "Cunning")
        {
            playerStats.SetMP(-8);
            ViewManager.GetView<BattleUIView>().setText("You make a Clever Ploy, and cast another spell!");
            int anotherSpell = playerGemSys.GetRandomGemIndex();
            if(anotherSpell == 0)
            {
                playerMoves.SpellOfWill(GetFirstAliveEnemy());
            }
            else if (anotherSpell == 1)
            {
                playerMoves.SpellOfCourage(GetFirstAliveEnemy());
            }
            else if (anotherSpell == 2)
            {
                playerMoves.SpellOfConstitution(battlingEnemies);
            }
            else if (anotherSpell == 3)
            {
                playerMoves.SpellOfPatience();
            }
            else if (anotherSpell == 5)
            {
                playerMoves.SpellOfGreatPatience();
            }
            else if (anotherSpell == 6)
            {
                playerMoves.SpellOfHeart();
            }


        }
        else if (playerGemSys.CurrentGem.name == "Will")
        {
            playerStats.SetMP(-5);
            ViewManager.GetView<BattleUIView>().setText("The magic within you sets your Heart Aflame!");
            playerMoves.SpellOfWill(targetedEnemy);
        }
        else if (playerGemSys.CurrentGem.name == "Courage")
        {
            playerStats.SetMP(-6);
            ViewManager.GetView<BattleUIView>().setText("You Seize the Blade against your foe!");
            playerMoves.SpellOfCourage(targetedEnemy);
        }
        else if (playerGemSys.CurrentGem.name == "Constitution")
        {
            playerStats.SetMP(-6);
            ViewManager.GetView<BattleUIView>().setText("You steel yourself to Weather The Storm ahead!");
            playerMoves.SpellOfConstitution(battlingEnemies);
        }
        else if (playerGemSys.CurrentGem.name == "Patience")
        {
            playerStats.SetMP(-4);
            ViewManager.GetView<BattleUIView>().setText("Good Things Come to those who wait!");
            playerMoves.SpellOfPatience();
        }
        else if (playerGemSys.CurrentGem.name == "Great Patience")
        {
            playerStats.SetMP(-6);
            ViewManager.GetView<BattleUIView>().setText("Great Things Come to brave treasure hunters!");
            playerMoves.SpellOfGreatPatience();
        }
        else if (playerGemSys.CurrentGem.name == "Heart")
        {
            playerStats.SetMP(-4);
            ViewManager.GetView<BattleUIView>().setText("You Take Heart against your foes!");
            playerMoves.SpellOfHeart();
        }
        yield return new WaitUntil(() => combatants[currentTurn].GetComponent<PlayerMoves>().moveInProgress == false);
        yield return new WaitForSeconds(.5f);

        // Ending bit \/ --------------------------------------
        if (cunningSecondTurn)
        {
            cunningSecondTurn = false;
        }
        else
        {
            if (currentTurn != turnArray.Length - 1) // Advance the turn
            {
                currentTurn++;
            }
            else
            {
                currentTurn = 0;
            }
        }


        status = MenuStatus.Inactive;  // Get rid of the menu
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoTurnAdvanceInven()    // Advancing a turn when the player uses their turn on an inventory action
    {
        activeCoroutine = true;
        toggleStatDisplays(false);
        if (cunningSecondTurn)
        {
            cunningSecondTurn = false;
        }
        else
        {
            if (currentTurn != turnArray.Length - 1) // Advance the turn
            {
                currentTurn++;
            }
            else
            {
                currentTurn = 0;
            }
        }
        StartCoroutine(indAction.DoFlashOutSelected());
        status = MenuStatus.Inactive;   // The player acts no more!

        activeCoroutine = false;
        yield return null;
    }
    #endregion

    IEnumerator DoTurnAdvanceEnemyTemp()
    {
        activeCoroutine = true;
        //toggleStatDisplaysE(true);
        ViewManager.GetView<BattleUIView>().setTutorialText("");

        yield return new WaitUntil(() => combatants[currentTurn].GetComponentInChildren<EnemyAnimatorS>().activeCoroutine == false);

        combatants[currentTurn].GetComponent<EnemyMoves>().Move(playerStats);

        yield return new WaitUntil(() => combatants[currentTurn].GetComponent<EnemyMoves>().moveInProgress == false);

        if (currentTurn != turnArray.Length - 1) // Advance the turn
        {
            currentTurn++;
        }
        else
        {
            currentTurn = 0;
        }
        status = MenuStatus.Inactive;   // The enemy acts no more!
        
        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoBattleWin()
    {
        activeCoroutine = true;
        toggleStatDisplays(false);
        clearStatMods();
        MusicBox.Instance.EndBattleFade();
        PlayerManager.Instance.GetComponentInChildren<PlayerAnimatorS>().PlayBattleWin();

        int xpGain = totalXpGain();
        ViewManager.GetView<BattleUIView>().setText("You win, and gain " + xpGain + " experience points!");
        yield return new WaitUntil(() => Input.GetButtonDown("Interact")); // Wait so the player can read the text box
        if (playerStats.SetXP(xpGain))
        {
            ViewManager.GetView<BattleUIView>().setText(playerStats.GetLVLUpText());
            yield return new WaitForSeconds(.1f);
            yield return new WaitUntil(() => Input.GetButtonDown("Interact"));
            
        }

        battleUI.GetComponent<FadeUI>().UIFadeOut();
        GameManager.Instance.Battle(false);                 // Tell the game manager that we're out of battle (this also pulls the camera back)
        PlayerManager.Instance.GetComponentInChildren<PlayerAnimatorS>().PlayBattleExit();
        yield return new WaitForSeconds(1.2f);
        MusicBox.Instance.ReturnToNormal();
        playerReenable();                                   // Reenable player movement
                           // Reenable enemies that weren't in the fight
        battleShowEnemies?.Invoke();
        fadeAllBattlingEnemies();
        

        yield return new WaitForSeconds(1f);


        destroyAllBattlingEnemies();
        // Fade out and destroy the enemies you fought with
        // Readout for XP gain
        // If LVL Up: Readout for LVL Up

        
        battleIntro = true;         // Resetting everything for if the player gets into a tangle in the same scene
        battleActive = false;
        activeCoroutine = false;
        mainEnemyMovement = null;

        endResult = EndStatus.None;
        status = MenuStatus.Inactive;

        allEnemyReenable(currentEnemy);
        //allEnemyReenableBattleStart();
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator DoBattleLoss()
    {
        activeCoroutine = true;
        toggleStatDisplays(false);
        clearStatMods();
        battleUI.GetComponent<FadeUI>().UIFadeOut();
        MusicBox.Instance.HardStop();
        //Play the defeat animation
        //PlayerManager.Instance.GetComponentInChildren<PlayerAnimatorS>().PlayBattleLost();
        yield return new WaitUntil(()=> PlayerManager.Instance.GetComponentInChildren<PlayerAnimatorS>().defeated);

        // Once that's done, load the credits scene
        SceneLoader.Instance.OnForcedTransition("25_GameOver");
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator DoBattleRun()
    {
        activeCoroutine = true;
        toggleStatDisplays(false);
        clearStatMods();
        enemyStats.Resurrect();
        MusicBox.Instance.EndBattleFade();
        yield return new WaitForSeconds(2f);                // Wait so the player can read the text box
        battleUI.GetComponent<FadeUI>().UIFadeOut();
        MusicBox.Instance.ReturnToNormal();
        GameManager.Instance.Battle(false);                 // Tell the game manager that we're out of battle

        playerReenable();                                   // Reenable player movement
                             // Reenable enemies that weren't in the fight
        fadeSpawnedEnemies();                               // Get rid of enemies spawned for the battle
       
        yield return new WaitForSeconds(2f);                // Wait for a few moments before letting the current enemy loose again
        battleShowEnemies?.Invoke();

        currentEnemyReenable();                             // Reenable the current enemy's movement, and destroy enemies spawn for the battle
        destroySpawnedEnemies();
        
        battleIntro = true;         // Resetting everything for if the player gets into a tangle in the same scene
        battleActive = false;
        activeCoroutine = false;
        mainEnemyMovement = null;

        endResult = EndStatus.None;
        status = MenuStatus.Inactive;

        allEnemyReenable(currentEnemy);
        //allEnemyReenableBattleStart();
        Destroy(gameObject);

        yield return null;
    }
}
