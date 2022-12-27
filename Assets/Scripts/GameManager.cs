using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private bool _isGameOver = false;

    public static GameManager Instance
    {
        get {
            if (_instance == null)
                Debug.Log("GameManager instance is null!");

            return _instance; 
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    public void GameOver(bool flag) // Example setter and getter for if player is defeated
    {
        _isGameOver = flag;
    }

    public bool isGameOver()
    {
        return _isGameOver;
    }
}



// I see! So this thing is used for broadcasting important states to lots of things, like 
// game overs or major state changes (Like if the player is walking around vs. in battle vs. in a menu, all things
// that can happen within the confines of one scene)
