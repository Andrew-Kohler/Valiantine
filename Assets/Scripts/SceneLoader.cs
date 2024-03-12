/*
Scene Loader
Used on:    ---
For:    A singleton that handles how scene transitions play out and contains methods for facilitating
        such a purpose
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    private string lastGate;

    GameObject transitionPanel;
    FadeScene fade;

    private PlayerStats playerS;
    private InventoryHolder playerI;
    private GemSystem playerG;

    public static SceneLoader Instance
    {
        get { 
            if(_instance == null)   
            {
                Debug.Log("Scene Loader is null!");
                Debug.Log("New Scene Loader 2");
                GameObject loaderHolder = new GameObject("[Scene Loader]");
                loaderHolder.AddComponent<SceneLoader>();
            }
            return _instance; 
        }
    }

    public void OnEnterGateway(string gateName, string levelToLoad)
    {
        lastGate = gateName;
        GameManager.Instance.Transition(true);  // Informs the game manager we're in a transition state

        // Save all the player's stuff
        playerS = PlayerManager.Instance.PlayerStats();
        playerI = PlayerManager.Instance.PlayerInventory();
        playerG = PlayerManager.Instance.GemSystem();

        transitionPanel = GameObject.Find("Black Panel");   // Fade out to black before loading the next scene
        fade = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeIn(levelToLoad);
    }

    public void OnForcedTransition(string levelToLoad)
    {

    }

    private void Awake()
    {
        // _instance = this;
        if (_instance == null)                              // We can't have the Scene Loader destroyed between scenes
        {                                                   // on account of it handling both sides of the transition
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) // Not really sure about these paramenters, but they're required, so sure?
    {
        // When the level is loaded:
        GameObject player = GameObject.Find("Player");  // Find the player gameObject
        // Set the player's stats and inventory to be correct
        
        StartCoroutine(DoLoadScene());

        Gateway[] allGates = FindObjectsOfType<Gateway>();  // Find all gateways
        foreach(Gateway gateway in allGates)    // Iterate through them
        {
            if(gateway.gatewayName == lastGate) // If a gateway name matches lastGate
            {
                player.transform.position = gateway.spawnPoint.position;  // player.transform.position = position of that gate's spawn point
            }
        }

        transitionPanel = GameObject.Find("Black Panel");   // Fade us into this new room of adventure!
        fade = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeOut();

    }

    private void OnDisable()
    {    
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator DoLoadScene()
    {
        yield return new WaitForEndOfFrame();
        //if(playerS != null)
        PlayerManager.Instance.PlayerStats().SetStats(playerS);
        //if (playerI != null)
        PlayerManager.Instance.PlayerInventory().RefillInventory(playerI.InventorySystem);
        //if (playerG != null)
        PlayerManager.Instance.GemSystem().RefillGems(playerG);
    }

}

// https://answers.unity.com/questions/998780/how-do-i-spawn-player-in-certain-places-between-sc.html
// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
