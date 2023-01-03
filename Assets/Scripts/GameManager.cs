using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; // = new GameManager();   // Look at this again, b/c I'm pretty sure awake should be doing this?

    private bool _isGameOver;   // Has the player been defeated?
    private bool _isMenu;       // Are we in a menu?
    private bool _isTransition; // Are we in a scene transition?
    private bool _isInteraction;// Are we in some kind of dialogue interaction (opening a chest, healing at a statue)?
    private bool _isBattle;     // Are we in a battle?
    // Other potential states of interest
    // isMainMenu

    private GameManager()
    {
        _isGameOver = false;
        _isMenu = false;
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

    public void Menu(bool flag) // Setter and getter for state of player being in the menu
    {
        _isMenu = flag;
    }
    public bool isMenu()
    {
        return _isMenu;
    }

    public void Transition(bool flag)
    {
        _isTransition = flag;
    }

    public bool isTransition()
    {
        return _isTransition;
    }

    public bool canMove()   // If we can move, nothing else should be going on
    {
        return !_isGameOver && !_isMenu && !_isBattle && !_isTransition && !_isInteraction;
    }
}



// I see! So this thing is used for broadcasting important states to lots of things, like 
// game overs or major state changes (Like if the player is walking around vs. in battle vs. in a menu, all things
// that can happen within the confines of one scene)
