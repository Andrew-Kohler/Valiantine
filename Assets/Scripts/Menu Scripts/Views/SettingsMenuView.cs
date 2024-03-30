/*
Settings Menu View
Used on:    GameObject
For:    Marks a game object as the main menu version of the game's settings menu
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuView : View
{
    [SerializeField] private GameObject regular;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject mainConfirm;
    [SerializeField] private GameObject loadConfirm;
    [SerializeField] private GameObject quitConfirm;

    [SerializeField] private Toggle tutToggle;

    [SerializeField] private Slider master;
    [SerializeField] private Slider ui;
    [SerializeField] private Slider music;
    [SerializeField] private Slider environment;
    [SerializeField] private Slider entity;

    private bool allValSet = false;

    [SerializeField] private List<AudioClip> sounds;
    private AudioSource audioS;

    private bool initing;
    private bool store;
    private bool activeCoroutine;

    private float mVolStore;    // Temps to store the volume in when we turn it down for the pause menu
    private float eVolStore;
    private float envVolStore;

    public override void Initialize()
    {
        if(tutToggle.isOn && !GameManager.Instance.tutorialText)
        {
            tutToggle.isOn = false;
        }
        initing = true;
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
        audioS = GetComponent<AudioSource>();
        initing = false;


    }

    private void OnEnable()
    {
        master.value = GameManager.Instance.masterVolume * 10;
        ui.value = GameManager.Instance.uiVolume * 10;
        music.value = GameManager.Instance.musicVolume * 10;
        entity.value = GameManager.Instance.entityVolume * 10;
        environment.value = GameManager.Instance.environmentVolume * 10;

        allValSet = true;
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
        activeCoroutine = false;
        if (!initing && audioS != null)
        {
            audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
            /*mVolStore = GameManager.Instance.musicVolume;
            eVolStore = GameManager.Instance.entityVolume;
            envVolStore = GameManager.Instance.environmentVolume;

            GameManager.Instance.musicVolume = .1f;
            GameManager.Instance.entityVolume = 0;
            GameManager.Instance.environmentVolume = 0f;*/
            store = true;
        }
    }

    private void OnDisable()
    {
        allValSet = false;
        if (!initing && audioS != null && store)
        {
            /*GameManager.Instance.musicVolume = mVolStore;
            GameManager.Instance.entityVolume = eVolStore;
            GameManager.Instance.environmentVolume = envVolStore;*/
            store = false;
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.isSettings() && !activeCoroutine)
        {
            ViewManager.ShowLast();
        }
    }

    public void OptionsButton()
    {
        regular.SetActive(false);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(true);
    }

    public void ToggleTutorial()
    {
        if((tutToggle.isOn && !GameManager.Instance.tutorialText) || (!tutToggle.isOn && GameManager.Instance.tutorialText))
        {
            GameManager.Instance.tutorialText = !GameManager.Instance.tutorialText;
        }
    }

    public void SetMasterVolume()
    {
        if (allValSet)
        {
            GameManager.Instance.masterVolume = master.value / 10f;

            audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        }
    }

    public void SetUIVolume()
    {
        if (allValSet)
        {
            GameManager.Instance.uiVolume = ui.value / 10f;
            audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        }
            
    }
    public void SetMusicVolume()
    {
        if (allValSet)
        {
            GameManager.Instance.musicVolume = music.value / 10f;

            audioS.PlayOneShot(sounds[0], GameManager.Instance.musicVolume * GameManager.Instance.masterVolume);
        }
    }
    public void SetEnvironmentVolume()
    {
        if (allValSet)
        {
            GameManager.Instance.environmentVolume = environment.value / 10f;

            audioS.PlayOneShot(sounds[6], GameManager.Instance.environmentVolume * GameManager.Instance.masterVolume);
        }
    }

    public void SetEntityVolume()
    {
        if (allValSet)
        {
            GameManager.Instance.entityVolume = entity.value / 10f;

            audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        }
    }

    #region OTHER BUTTONS
    public void LoadLastSaveButton()
    {
        audioS.PlayOneShot(sounds[2], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        regular.SetActive(false);
        loadConfirm.SetActive(true);
    }

    public void LoadLastSave()
    {
        audioS.PlayOneShot(sounds[3], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        loadConfirm.SetActive(false);
        Time.timeScale = 1;
        GameManager.Instance.ReadInSaveData();
    }

    public void ReturnToMainButton()
    {
        audioS.PlayOneShot(sounds[2], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        regular.SetActive(false);
        mainConfirm.SetActive(true);
    }

    public void ReturnToMain()
    {
        mainConfirm.SetActive(false);
        Time.timeScale = 1;
        SceneLoader.Instance.OnForcedPlayerTransition("23_MainMenu");
    }

    public void QuitButton()
    {
        audioS.PlayOneShot(sounds[2], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        regular.SetActive(false);
        quitConfirm.SetActive(true);
    }

    public void Quit()
    {
        audioS.PlayOneShot(sounds[4], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        Application.Quit();
    }

    public void backToPrimary()
    {
        audioS.PlayOneShot(sounds[4], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
    }
    #endregion

    /*private IEnumerator CloseSettings()
    {
        activeCoroutine = true;
        audioS.PlayOneShot(sounds[1], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(.3f);
        ViewManager.ShowLast();
        activeCoroutine = false;
    }*/
}
