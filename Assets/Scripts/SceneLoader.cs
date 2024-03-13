/*
Scene Loader
Used on:    ---
For:    A singleton that handles how scene transitions play out and contains methods for facilitating
        such a purpose
*/

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    private string lastGate;
    private int loadCount = 0;

    GameObject transitionPanel;
    FadeScene fade;

    private PlayerStats playerS = null;
    private InventoryHolder playerI = null;
    private GemSystem playerG = null;

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

    public void OnEnterGateway(string gateName, string levelToLoad, bool lostWoods)
    {
        lastGate = gateName;
        GameManager.Instance.Transition(true);  // Informs the game manager we're in a transition state

        // Save all the player's stuff
        playerS = PlayerManager.Instance.PlayerStats();
        playerI = PlayerManager.Instance.PlayerInventory();
        playerG = PlayerManager.Instance.GemSystem();


        GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>().standStill = true;
        GameManager.Instance.transitionDirection = GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>().GetDirection();
        transitionPanel = GameObject.Find("Black Panel");   // Fade out to black before loading the next scene
        fade = transitionPanel.GetComponent<FadeScene>();
        if (lostWoods)
        {
            GameManager.Instance.lostWoodsEscapeTracker.Add(Int32.Parse(gateName));
            //Debug.Log(Int32.Parse(gateName));
            if (GameManager.Instance.lostWoodsEscapeTracker.Count == 5)
            {
                lastGate = "3";
                if (GameManager.Instance.LostWoodsClearCheck())
                {
                    fade.SceneFadeIn("18_BeforeBoss");
                }
                else if (GameManager.Instance.LostWoodsSecretCheck())
                {

                    fade.SceneFadeIn("22_GreatPatience");
                }
                else
                {
                    
                    fade.SceneFadeIn("2_Fountain");
                }
                GameManager.Instance.lostWoodsEscapeTracker.Clear();
            }
            else
            {
                
                fade.SceneFadeIn(levelToLoad);
            }
        }
        else
        {
            fade.SceneFadeIn(levelToLoad);
        }
        
    }

    public void OnForcedTransition(string levelToLoad) // For when we need to load a scene WITHOUT caring about a player!
    {
        transitionPanel = GameObject.Find("Black Panel");   // Fade out to black before loading the next scene
        fade = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeIn(levelToLoad);
    }

    public void OnForcedPlayerTransition(string levelToLoad) // For when we need to care a little about a player
    {
        // Save all the player's stuff
        playerS = PlayerManager.Instance.PlayerStats();
        playerI = PlayerManager.Instance.PlayerInventory();
        playerG = PlayerManager.Instance.GemSystem();

        transitionPanel = GameObject.Find("Black Panel");   
        fade = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeIn(levelToLoad);
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
        GameManager.Instance.Cutscene(false);
        GameObject player = GameObject.Find("Player");  // Find the player gameObject
        GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>().SetDirection(GameManager.Instance.transitionDirection);
        // Set the player's stats and inventory to be correct
        
        if(loadCount > 0 && player != null)
            StartCoroutine(DoLoadScene());

        Gateway[] allGates = FindObjectsOfType<Gateway>();  // Find all gateways
        foreach(Gateway gateway in allGates)    // Iterate through them
        {
            if(gateway.gatewayName == lastGate) // If a gateway name matches lastGate
            {
                player.transform.position = gateway.spawnPoint.position;  // player.transform.position = position of that gate's spawn point
            }
        }

        if (SceneManager.GetActiveScene().name == "0_Exterior")
        {
            StartCoroutine(DoIntroCutscene());
        }
        else
        {
            transitionPanel = GameObject.Find("Black Panel");   // Fade us into this new room of adventure!
            fade = transitionPanel.GetComponent<FadeScene>();
            fade.SceneFadeOut();
        }
        loadCount++;


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

    private IEnumerator DoIntroCutscene()
    {
        yield return new WaitForEndOfFrame();
        ViewManager.GetView<InGameUIView>().playIntroConvo();
        yield return new WaitForSeconds(24f);

        transitionPanel = GameObject.Find("Black Panel");   // Fade us into this new room of adventure!
        fade = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeOut();
        yield return null;
    }

}

// https://answers.unity.com/questions/998780/how-do-i-spawn-player-in-certain-places-between-sc.html
// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
