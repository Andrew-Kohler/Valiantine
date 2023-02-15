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

    private bool _isGameOver;   // Has the player been defeated?
    private bool _isInventory;  // Are we in the inventory?
    private bool _isSettings;   // Are we in settings?
    private bool _isTransition; // Are we in a scene transition?
    private bool _isInteraction;// Are we in some kind of dialogue interaction (opening a chest, healing at a statue)?
    private bool _isBattle;     // Are we in a battle?
    // Other potential states of interest
    // isMainMenu

    private GameManager()
    {
        _isGameOver = false;
        _isInventory = false;
        _isSettings = false;
        _isTransition = false;
        _isInteraction = false;
        _isBattle = false;
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
        _isInventory = flag;
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
        _isBattle = flag;
    }

    public bool isBattle()
    {
        return _isBattle;
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

    
}



// I see! So this thing is used for broadcasting important states to lots of things, like 
// game overs or major state changes (Like if the player is walking around vs. in battle vs. in a menu, all things
// that can happen within the confines of one scene)
