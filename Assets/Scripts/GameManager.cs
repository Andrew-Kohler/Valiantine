using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = new GameManager();   // Look at this again, b/c I'm pretty sure awake should be doing this?
    //private static GameManager _instance;

    private bool _isGameOver;
    private bool _isMenu;
    // States of interest
    // isExplore
    // isBattle
    // isMenu
    // isMainMenu

    private GameManager()
    {
        _isGameOver = false;
        _isMenu = false;
        Debug.Log("Instance is created");
    }

    public static GameManager Instance
    {
        get {
            /*if (_instance == null)
                Debug.LogError("GameManager instance is null!");*/
    
            return _instance; 
        }
    }
    private void Awake()
    {
        _instance = this;
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

    // So, player presses esc, and this sets menu to true. We have the menu check for when menu is true, and that's when it's visible
    // It seems good practice is to manage menuing in here? Maybe I'll do that

    // Step 1: Make a script where pressing esc toggles the menu on and off, and attatch it to the player
    // Step 2: Go into playerMovement and make it so that when a menu is open, they can't move
        // Step 2a: Also make it so that they can't interact with chests and stuff
    // Step 3:
}



// I see! So this thing is used for broadcasting important states to lots of things, like 
// game overs or major state changes (Like if the player is walking around vs. in battle vs. in a menu, all things
// that can happen within the confines of one scene)
