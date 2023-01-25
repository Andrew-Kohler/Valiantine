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

    private enum activeMenu { Upper, Attack, Spell, Inventory};

    // The enemy in question
    string enemyName;

    // Instances of all other necessary classes
    GameObject currentEnemy;
    GameObject player;
    PlayerStats playerStats;
    EnemyStats enemyStats;
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

    }

    private void Update()
    {
        if (GameManager.Instance.isBattle())
        {
            if (battleIntro)    // The fun intro sequence that happens when the battle begins
            {
                if (!activeCoroutine)
                {
                    // Get the instances of:
                    player = GameObject.Find("Player");                 // Player
                    playerStats = player.GetComponent<PlayerStats>();   // Player stats
                    enemyStats = currentEnemy.GetComponent<EnemyStats>(); // Enemy stats
                    playerRb = player.GetComponent<Rigidbody>();
                    enemyRb = currentEnemy.GetComponent<Rigidbody>();

                    // Correctly position the camera (currently done by just leaving it in the middle)  
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
                }
                

            } // End of battle intro

            else if (battleActive)  // Primary turn loop
            {
                if (!turnArray[currentTurn].getDowned())
                {

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

        // Problem being that this just starts all the coroutines and then goes ahead and deactivates all the enemies

        /*foreach (GameObject enemy in enemies)
        {
            if (enemy.name != show.name)
            {
                enemy.SetActive(false);

            }
        }*/
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
        // We wanna start the coroutine right here

        battleIntro = false;                        // Set battleIntro to false and battleActive to true 
        battleActive = true;
        activeCoroutine = false;
        yield return null;
    }
}
