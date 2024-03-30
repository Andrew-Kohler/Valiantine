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

    private float masterVolumeTemp;

    GameObject transitionPanel;
    FadeScene fade;

    private PlayerStats playerS;
    private InventoryHolder playerI;
    private GemSystem playerG;

    private GameManager.SavedPlayerStats savedPlayerStats = null;
    private GameManager.SavedInventoryContents savedInventory = null;
    private GameManager.SavedGems savedGems = null; 

    public static SceneLoader Instance
    {
        get { 
            if(_instance == null)   
            {
                Debug.Log("Scene Loader is null! Making a new one...");
                GameObject loaderHolder = new GameObject("[Scene Loader]");
                loaderHolder.AddComponent<SceneLoader>();
            }
            return _instance; 
        }
    }
    #region SCENE TRANSITION METHODS
    public void OnEnterGateway(string gateName, string levelToLoad, bool lostWoods)
    {
        lastGate = gateName;
        masterVolumeTemp = GameManager.Instance.masterVolume;
        StartCoroutine(DoVolumeDown());
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
        masterVolumeTemp = GameManager.Instance.masterVolume;
        StartCoroutine(DoVolumeDown());
        transitionPanel = GameObject.Find("Black Panel");   // Fade out to black before loading the next scene
        fade = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeIn(levelToLoad);
    }

    public void OnForcedPlayerTransition(string levelToLoad) // For when we need to care a little about a player
    {
        masterVolumeTemp = GameManager.Instance.masterVolume;
        StartCoroutine(DoVolumeDown());
        // Save all the player's stuff
        if (levelToLoad != "23_MainMenu" && levelToLoad != "24_Credits")
        {
            playerS = PlayerManager.Instance.PlayerStats();
            playerI = PlayerManager.Instance.PlayerInventory();
            playerG = PlayerManager.Instance.GemSystem();
        }

        transitionPanel = GameObject.Find("Black Panel");
        fade = transitionPanel.GetComponent<FadeScene>();

        fade.SceneFadeIn(levelToLoad);
    }
    #endregion

    private void Awake()
    {
        // _instance = this;
        if (_instance == null)                              // We can't have the Scene Loader destroyed between scenes
        {                                                   // on account of it handling both sides of the transition
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        /*else if(Instance != this)
        {
            Destroy(gameObject);
        }*/
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) // Not really sure about these paramenters, but they're required, so sure?
    {
        // When the level is loaded:
        GameManager.Instance.Cutscene(false);
        GameManager.Instance.Inventory(false);
        GameManager.Instance.Battle(false);
        GameManager.Instance.Settings(false);
        StartCoroutine(DoVolumeUp());
        // We don't need to worry about resetting BattleManager because it isn't persistent between scenes

        if (SceneManager.GetActiveScene().name != "23_MainMenu" && SceneManager.GetActiveScene().name != "24_Credits" && SceneManager.GetActiveScene().name != "25_GameOver")
        {
            GameObject player = GameObject.Find("Player");  // Find the player gameObject
            GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>().SetDirection(GameManager.Instance.transitionDirection);

            // Set the player's stats and inventory to be correct

            if (loadCount > 0 && player != null)
                StartCoroutine(DoLoadScene());

            Gateway[] allGates = FindObjectsOfType<Gateway>();  // Find all gateways
            foreach (Gateway gateway in allGates)    // Iterate through them
            {
                if (gateway.gatewayName == lastGate) // If a gateway name matches lastGate
                {
                    player.transform.position = gateway.spawnPoint.position;  // player.transform.position = position of that gate's spawn point
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "0_Exterior")
        {
            StartCoroutine(DoIntroCutscene());
        }
        else if (SceneManager.GetActiveScene().name != "23_MainMenu")
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

    public void SetCurrentPlayerData(GameManager.SavedPlayerStats stats, GameManager.SavedInventoryContents holder, GameManager.SavedGems sys)
    {
        savedPlayerStats = stats;
        savedInventory = holder;
        savedGems = sys;
    }

    // Private methods -------------------------------------------------

    private IEnumerator DoLoadScene()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (savedPlayerStats != null)
        {
            PlayerManager.Instance.PlayerStats().SetStatsFromSaveData(savedPlayerStats);
            PlayerManager.Instance.PlayerInventory().RefillInventoryFromSaveData(savedInventory);
            PlayerManager.Instance.GemSystem().RefillGemsFromSaveData(savedGems);

            savedPlayerStats = null;
            savedInventory = null;
            savedGems = null;
        }
        else
        {
            PlayerManager.Instance.PlayerStats().SetStats(playerS);
            PlayerManager.Instance.PlayerInventory().RefillInventory(playerI.InventorySystem);
            PlayerManager.Instance.GemSystem().RefillGems(playerG);
            /*if (playerS != null)
            {
                Debug.Log("Stats weren't null");
                PlayerManager.Instance.PlayerStats().SetStats(playerS);
            }
            else{
                Debug.Log("Stats were null");
                Debug.Log(playerS.GetATK());
            }
                
            if (playerI != null)
                PlayerManager.Instance.PlayerInventory().RefillInventory(playerI.InventorySystem);
            else
                Debug.Log("Inven was null");
            if (playerG != null)
                PlayerManager.Instance.GemSystem().RefillGems(playerG);
            else
                Debug.Log("Gems were null");*/
        }
        
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

    private IEnumerator DoVolumeDown()
    {
        while (GameManager.Instance.masterVolume > 0f)
        {
            GameManager.Instance.masterVolume -= Time.deltaTime  / 2f;
            yield return null;
        }
    }

    private IEnumerator DoVolumeUp()
    {
        while (GameManager.Instance.masterVolume < masterVolumeTemp)
        {
            GameManager.Instance.masterVolume += Time.deltaTime  / 2f;
            yield return null;
        }
    }

}

// https://answers.unity.com/questions/998780/how-do-i-spawn-player-in-certain-places-between-sc.html
// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
