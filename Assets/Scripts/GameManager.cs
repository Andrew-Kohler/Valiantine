/*
Game Manager
Used on:    ---
For:    Manages the state of the game and tells everything else what's going on
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; // = new GameManager();   // Look at this again, b/c I'm pretty sure awake should be doing this?
    private bool activeCoroutine;

    private bool _isGameOver;   // Has the player been defeated?
    private bool _isInventory;  // Are we in the inventory?
    private bool _isSettings;   // Are we in settings?
    private bool _isTransition; // Are we in a scene transition?
    private bool _isInteraction;// Are we in some kind of dialogue interaction (opening a chest, healing at a statue)?
    private bool _isBattle;     // Are we in a battle?
    [SerializeField] private bool _isWindy;      // ...Is it windy
    // Other potential states of interest
    // isMainMenu
    private bool _canInteract;

    private GameObject currentInteractable;

    // All the events that broadcast when a moment of change occurs (currently used primarily for camera)
    public delegate void OnInventoryStateChange();
    public static event OnInventoryStateChange onInventoryStateChange;
    public delegate void OnBattleStateChange();
    public static event OnBattleStateChange onBattleStateChange;
    public delegate void OnSaveStatueStateChange();
    public static event OnSaveStatueStateChange onSaveStatueStateChange;
    public delegate void OnSavePointStateChange(int currentPoint);
    public static event OnSavePointStateChange onSavePointStateChange;
    public delegate void OnChestStateChange();
    public static event OnChestStateChange onChestStateChange;

    public delegate void OnWindStateChange();
    public static event OnWindStateChange onWindStateChange;

    private GameManager()
    {
        activeCoroutine = false;

        _isGameOver = false;
        _isInventory = false;
        _isSettings = false;
        _isTransition = false;
        _isInteraction = false;
        _isBattle = false;
        _isWindy = false;
        _canInteract = false;
        currentInteractable = null;
    }

    public static GameManager Instance
    {
        get {
            if (_instance == null)
            {
                GameObject managerHolder = new GameObject("[Game Manager]");
                managerHolder.AddComponent<GameManager>();
            }
    
            return _instance; 
        }
    }
    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (!activeCoroutine)
        {
            StartCoroutine(DoWindCycle());
        }
    }

    public void GameOver(bool flag) // Setter and getter for state of player defeat
    {
        _isGameOver = flag;
    }
    public bool isGameOver()
    {
        return _isGameOver;
    }

    public void Inventory(bool flag) // Setter and getter for state of player being in the inventory menu
    {
        
        bool former = _isInventory; 
        _isInventory = flag;
        if (flag != former)
        {
            onInventoryStateChange?.Invoke();   // And then THIS messes with the camera
        }

    }
    public bool isInventory()
    {
        return _isInventory;
    }

    public void Settings(bool flag) // Setter and getter for state of player being in the settings menu
    {
        _isSettings = flag;
    }
    public bool isSettings()
    {
        return _isSettings;
    }

    public void Interaction(bool flag)
    { 
        bool former = _isInteraction;
        _isInteraction = flag;
        if(_isInteraction == true)  // Start the interaction
        {
            currentInteractable.GetComponent<Interactable>().Interact();
        }
        if (currentInteractable.CompareTag("Save Statue") && flag != former)
        {
            onSaveStatueStateChange?.Invoke();
        }
        if (currentInteractable.CompareTag("Chest") && flag != former)
        {
            onChestStateChange?.Invoke();
        }
        if (currentInteractable.CompareTag("Save Point") && flag != former)
        {
            onSavePointStateChange?.Invoke(0);
        }
        // Ok, write it down now -> diff event for each type of interactable, they all do the same stuff for initial pos in animator but all do diff stuff to cam

    }

    public bool isInteraction()
    {
        return _isInteraction;
    }

    public void Transition(bool flag)   // Getter and setter for state of a scene transition in progress
    {
        _isTransition = flag;
    }

    public bool isTransition()
    {
        return _isTransition;
    }

    public void Battle(bool flag)   // Setter and getter for if we are in battle!
    {
        
        bool former = _isBattle;        // This little song and dance lets me check if a change occurs
        _isBattle = flag;
        if (flag != former)
        {
            onBattleStateChange?.Invoke();
        }

    }

    public bool isBattle()
    {
        return _isBattle;
    }

    public void Windy(bool wind)       // Setter and getter for if...it is windy.
    {
        bool former = _isWindy;
        _isWindy = wind;
        if (wind != former)
        {
            onWindStateChange?.Invoke(); 
        }
    }

    public bool IsWindy()
    {
        return _isWindy;
    }

    public void CanInteract(bool flag)   // Setter and getter for if we can interact with something!
    {
        _canInteract = flag;
        if(_canInteract == false)
        {
            currentInteractable = null;
        }
    }

    public void CurrentInteractable(GameObject interactable)
    {
        currentInteractable = interactable;
    }

    public GameObject getCurrentInteractable()
    {
        return currentInteractable;
    }

    public bool isCanInteract()
    {
        return _canInteract;
    }

    public bool canMove()   // If we can move, nothing else should be going on
    {
        return !_isGameOver && !_isInventory && !_isSettings && !_isBattle && !_isTransition && !_isInteraction;
    }

    public bool enemyCanMove()
    {
        return !_isGameOver && !_isSettings && !_isBattle && !_isTransition && !_isInteraction;
    }

    public bool freeCam()
    {
        return !_isGameOver && !_isInventory && !_isSettings && !_isBattle && !_isInteraction;
    }

    //Coroutines

    IEnumerator DoWindCycle()
    {
        activeCoroutine = true;
        // Generate a random number for the wind to NOT play
        float downtime = 10f;//Random.Range(30f, 75f);
        // Generate a random number for the wind to play for
        float uptime = 10f;//Random.Range(5f, 17f);

        // Let the wind not play for a while
        yield return new WaitForSeconds(downtime);
        // Switch wind to true
        Windy(true);
        // Wait for the play time
        yield return new WaitForSeconds(uptime);
        // Set it to false
        Windy(false);

        activeCoroutine = false;
    }

    
}



// I see! So this thing is used for broadcasting important states to lots of things, like 
// game overs or major state changes (Like if the player is walking around vs. in battle vs. in a menu, all things
// that can happen within the confines of one scene)
