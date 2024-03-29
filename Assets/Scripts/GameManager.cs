/*
Game Manager
Used on:    ---
For:    Manages the state of the game and tells everything else what's going on
*/

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
//using Extras;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance; // = new GameManager();   // Look at this again, b/c I'm pretty sure awake should be doing this?
    private bool activeCoroutine;
    public bool towerfall = false;
    public bool tutorialText = true;

    private bool _isGameOver;   // Has the player been defeated?
    private bool _isInventory;  // Are we in the inventory?
    private bool _isSettings;   // Are we in settings?
    private bool _isTransition; // Are we in a scene transition?
    private bool _isInteraction;// Are we in some kind of dialogue interaction (opening a chest, healing at a statue)?
    private bool _isBattle;     // Are we in a battle?
    private bool _isAnimating;  // Specifically for if we want to freeze player motion for a cutscene moment
    private bool _isCutscene;   // Specifically for if we want to move the player around during a cutscene
    [SerializeField] private bool _isWindy;      // ...Is it windy

    private bool _canInteract;

    private GameObject currentInteractable;

    public float entityVolume = 1;
    public float environmentVolume = 1;
    public float musicVolume = 1;
    public float uiVolume = 1;
    public float masterVolume = 1;

    // All the events that broadcast when a moment of change occurs (currently used primarily for camera)
    #region STATE CHANGE EVENTS
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
    public delegate void OnGateStateChange();
    public static event OnGateStateChange onGateStateChange;
    public delegate void OnPlaqueStateChange();
    public static event OnPlaqueStateChange onPlaqueStateChange;
    public delegate void OnRubbleStateChange();
    public static event OnRubbleStateChange onRubbleStateChange;

    public delegate void OnWindStateChange();
    public static event OnWindStateChange onWindStateChange;
    #endregion

    //Storing player stats between scenes
    [System.Serializable]
    public class SavedPlayerStats
    {
        public int HP;
        public int MaxHP;
        public int MP;
        public int MaxMP;

        public int ATK;
        public int DEF;
        public int SPD;

        public int LVL;
        public int XP;
        public int baseXPThreshold;

        /*public float ATKMod;
        public float DEFMod;
        public float SPDMod;
        public float MaxMPMod;
        public float MaxHPMod;
        public float XPMod;

        public float GemATKMod;
        public float GemDEFMod;
        public float GemSPDMod;
        public float GemMaxMPMod;
        public float GemMaxHPMod;*/

        public bool down;
    }
    [System.Serializable]
    public class SavedInventoryContents
    {
        public string[] itemNames; // The names of the items in the slots
        public int[] itemCounts;// How many items were in the correspoinding slot
    }
    [System.Serializable]
    public class SavedGems
    {
        public bool[] heldGems; // Whether we hold each of the 7 gems
        public int equippedGemIndex;    // The index in heldGems of the one we had equipped
    }

    [System.Serializable]
    private class SaveData
    {
        public string LastScene = "0_Exterior";   // The index of the last scene the player was in (default 0, Exterior)
        public int LastPoint = 0;   // The index of the spawn point the player was at (default 0, the origin of Scene 0)
        public bool[] openedChests; // Every chest has an index; it checks this list to see if it's been opened
        public bool[] openedGates;  // Every gate has an index; it checks this list to see if it's been opened
        public bool towerFallen = false;    // Whether towerfall has occurred
        public bool bossDefeated = false;   // Boss doesn't respawn
        
        public SavedPlayerStats PlayerStats;             // The inventory, stats, and gems of the player
        public SavedInventoryContents InventoryHolder;
        public SavedGems GemSystem;
    }

    
    private SavedPlayerStats savedPlayerStats;
    private SavedInventoryContents savedInventoryContents;
    private SavedGems savedGems;
    private SaveData saveData;
    

    public bool[] openedChests; // Every chest has an index; it checks this list to see if it's been opened
    public bool[] openedGates;  // Every gate has an index; it checks this list to see if it's been opened
    public bool bossDefeated;   // Boss doesn't respawn

    public int transitionDirection; // The way we were facing when we went into a scene

    public ArrayList lostWoodsEscapeTracker;    // The list of traversed gates we check to see if escape from lost woods is possible
    private ArrayList lostWoodsNormalEscape;// = new ArrayList { 1, 4, 2, 3, 1 };
    private ArrayList lostWoodsWeirdEscape;// = new ArrayList { 1, 3, 2, 4, 1 };
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

        openedChests = new bool[16];    // Indicies 0-15
        openedGates = new bool[4];     // Indicies 0-3

        lostWoodsEscapeTracker = new ArrayList();
        lostWoodsNormalEscape = new ArrayList { 3, 2, 4, 1, 3 };
        lostWoodsWeirdEscape = new ArrayList { 1, 4, 2, 3, 1 };
    }

    public static GameManager Instance 
    {
        get {
            if (_instance == null)
            {
                GameObject managerHolder = new GameObject("[Game Manager]");
                managerHolder.AddComponent<GameManager>();
                DontDestroyOnLoad(managerHolder);
                //_instance = managerHolder.GetComponent<GameManager>();          
            }
    
            return _instance; 
        }
    }
    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(this.gameObject);

        // initialize and load data
        saveData = new SaveData();
        savedPlayerStats = new SavedPlayerStats();
        savedInventoryContents = new SavedInventoryContents();
        savedGems = new SavedGems();
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            // read json file into data object
            string json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(json);
            bossDefeated = saveData.bossDefeated;
        }
        else // default save file configuration
        {
            saveData.LastScene = "0_Exterior";
            saveData.LastPoint = 0;
        }
    }

    private void Update()
    {
        if (!activeCoroutine)
        {
            StartCoroutine(DoWindCycle());
        }
    }

    // ALL of the state check methods
    #region STATE CHECK METHODS
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

    public void Animating(bool flag) // Setter and getter for state of player being in the settings menu
    {
        _isAnimating = flag;
    }
    public bool isAnimating()
    {
        return _isAnimating;
    }

    public void Interaction(bool flag)
    { 
        bool former = _isInteraction;
        _isInteraction = flag;
        GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>().standStill = !GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>().standStill;
        if (_isInteraction == true)  // Start the interaction
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
        if (currentInteractable.CompareTag("Gate") && flag != former)
        {
            onGateStateChange?.Invoke();
        }
        if (currentInteractable.CompareTag("Plaque") && flag != former)
        {
            onPlaqueStateChange?.Invoke();
        }
        if (currentInteractable.CompareTag("Save Point") && flag != former)
        {
            onSavePointStateChange?.Invoke(currentInteractable.GetComponent<SavePointInteractable>().pointNumber);
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

    public void Cutscene(bool flag)
    {
        _isCutscene = flag;
    }

    public bool isCutscene()
    {
        return _isCutscene;
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
        return !_isGameOver && !_isInventory && !_isSettings && !_isBattle && !_isTransition && !_isInteraction && !_isAnimating;
    }

    public bool enemyCanMove()
    {
        return !_isGameOver && !_isSettings && !_isBattle && !_isTransition && !_isInteraction;
    }

    public bool freeCam()
    {
        return !_isGameOver && !_isInventory && !_isSettings && !_isBattle && !_isInteraction && !_isCutscene;
    }
    #endregion

    // Save data methods
    public void WriteSaveData(string sceneName, int pointNumber) // Call this at save points
    {
        // Writing the simple values
        saveData.LastScene = sceneName;
        saveData.LastPoint = pointNumber;
        saveData.towerFallen = this.towerfall;
        saveData.bossDefeated = this.bossDefeated;
        saveData.openedChests = this.openedChests;
        saveData.openedGates = this.openedGates;

        // Writing in player stats
        PlayerStats stats = PlayerManager.Instance.PlayerStats();
        savedPlayerStats.HP = stats.GetHP();

        savedPlayerStats.MaxHP = stats.GetMaxHPRaw();
        savedPlayerStats.MP = stats.GetMP();
        savedPlayerStats.MaxMP = stats.GetMaxMPRaw();

        savedPlayerStats.ATK = stats.GetATKRaw();
        savedPlayerStats.DEF = stats.GetDEFRaw();
        savedPlayerStats.SPD = stats.GetSPDRaw();

        savedPlayerStats.LVL = stats.GetLVL();
        savedPlayerStats.XP = stats.GetXP();
        savedPlayerStats.baseXPThreshold = stats.GetXPThreshold();

        // We don't save the non-gem modifiers b/c they're battle exclusive
        // We don't save the gem modifiers b/c they'll be set when we re-equip the previously equipped gem
        savedPlayerStats.down = false;  // We do save this to make save loading resets easier

        saveData.PlayerStats = savedPlayerStats;

        // Writing in inventory contents
        ArrayList retrievedContents = PlayerManager.Instance.PlayerInventory().GetInventoryContents();
        ArrayList retrievedStacks = PlayerManager.Instance.PlayerInventory().GetInventoryStackSizes();

        savedInventoryContents.itemNames = new string[retrievedContents.Count];
        savedInventoryContents.itemCounts = new int[retrievedStacks.Count];

        for(int i = 0; i < retrievedStacks.Count; i++)
        {
            savedInventoryContents.itemNames[i] = (string)retrievedContents[i];
            savedInventoryContents.itemCounts[i] = (int)retrievedStacks[i];
        }

        saveData.InventoryHolder = savedInventoryContents;

        // Writing in gem contents
        savedGems.heldGems = PlayerManager.Instance.GemSystem().GetGemContents();
        savedGems.equippedGemIndex = PlayerManager.Instance.GemSystem().currentGemIndex;

        saveData.GemSystem = savedGems;

        //saveData.PlayerStats = PlayerManager.Instance.PlayerStats();
        //saveData.InventoryHolder = PlayerManager.Instance.PlayerInventory();
        //saveData.GemSystem = PlayerManager.Instance.GemSystem();

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedata.json", json);
    }

    public void WipeSaveData() // For starting a new game
    {
        saveData = new SaveData();
        savedPlayerStats = new SavedPlayerStats();
        savedInventoryContents = new SavedInventoryContents();
        savedGems = new SavedGems();

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savedata.json", json);
    }

    public void ReadInSaveData() // Call this when continuing a game or loading to a previous save point
    {
        towerfall = saveData.towerFallen;
        bossDefeated = saveData.bossDefeated;
        openedChests = saveData.openedChests;
        openedGates = saveData.openedGates;
        
        // Read in player stats, inventory contents, and gems
        SceneLoader.Instance.SetCurrentPlayerData(saveData.PlayerStats, saveData.InventoryHolder, saveData.GemSystem);

        SceneLoader.Instance.OnForcedTransition(saveData.LastScene);
    }

    public string SavedScene()
    {
        return saveData.LastScene;
    }

    public bool LostWoodsClearCheck()
    {
        for(int i = 0; i < 5; i++)
        {
            
            if (lostWoodsEscapeTracker[i].ToString() != lostWoodsNormalEscape[i].ToString())
            {
                //Debug.Log(lostWoodsEscapeTracker[i] + " is not equal to " + lostWoodsNormalEscape[i]);
                return false;
            }
                
        }
        return true;
    }

    public bool LostWoodsSecretCheck()
    {
        for (int i = 0; i < 5; i++)
        {
            if (lostWoodsEscapeTracker[i].ToString() != lostWoodsWeirdEscape[i].ToString())
                return false;
        }
        return true;
    }


    // Coroutines
    IEnumerator DoWindCycle()
    {
        activeCoroutine = true;
        // Generate a random number for the wind to NOT play
        float downtime = Random.Range(30f, 75f);
        // Generate a random number for the wind to play for
        float uptime = Random.Range(15f, 23f);

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
