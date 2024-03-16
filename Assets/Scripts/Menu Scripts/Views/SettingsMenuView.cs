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
    public bool goMain;

    /*public delegate void GoToMain();
    public static event GoToMain goToMain;*/

    public override void Initialize()
    {
        regular.SetActive(true);
        quitConfirm.SetActive(false);
        loadConfirm.SetActive(false);
        mainConfirm.SetActive(false);
        options.SetActive(false);
        goMain = false;
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

    }

    public void ReturnToMainButton()
    {
        regular.SetActive(false);
        mainConfirm.SetActive(true);
    }

    public void ReturnToMain()
    {
        goMain = true;
        //goToMain?.Invoke();
        mainConfirm.SetActive(false);
        Time.timeScale = 1;
        GameManager.Instance.Settings(false);
        
        
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
