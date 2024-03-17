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

    public override void Initialize()
    {
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
    }

    private void OnEnable()
    {
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
    }

    private void Update()
    {
        if (!GameManager.Instance.isSettings())
        {
            ViewManager.ShowLast();
        }
    }

    public void LoadLastSaveButton()
    {
        regular.SetActive(false);
        loadConfirm.SetActive(true);
    }

    public void LoadLastSave()
    {
        loadConfirm.SetActive(false);
        Time.timeScale = 1;
        GameManager.Instance.ReadInSaveData();
    }

    public void ReturnToMainButton()
    {
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
        regular.SetActive(false);
        quitConfirm.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void backToPrimary()
    {
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
    }
}
