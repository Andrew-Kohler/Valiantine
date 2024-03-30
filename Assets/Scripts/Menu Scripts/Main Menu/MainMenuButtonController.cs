using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuButtonController : MonoBehaviour
{
    [SerializeField] MainMenuCam camControl;
    [SerializeField] Animator creditsAnimator;
    [SerializeField] FadeUI doubleCheckPanel;
    [SerializeField] TextMeshProUGUI contText;
    [SerializeField] GameObject logo;
    [SerializeField] GameObject golem;

    [SerializeField] private Toggle tutToggle;

    [SerializeField] private Slider master;
    [SerializeField] private Slider ui;
    [SerializeField] private Slider music;
    [SerializeField] private Slider environment;
    [SerializeField] private Slider entity;

    [SerializeField] private AudioSource jukebox;

    private bool allValSet = false;

    [SerializeField] private List<AudioClip> sounds;
    private AudioSource audioS;

    private void OnEnable()
    {
        if (tutToggle.isOn && !GameManager.Instance.tutorialText)
        {
            tutToggle.isOn = false;
        }

        master.value = GameManager.Instance.masterVolume * 10;
        ui.value = GameManager.Instance.uiVolume * 10;
        music.value = GameManager.Instance.musicVolume * 10;
        entity.value = GameManager.Instance.entityVolume * 10;
        environment.value = GameManager.Instance.environmentVolume * 10;

        allValSet = true;
    }

    private void OnDisable()
    {
        allValSet = false;
    }

    private void Start()
    {
        if (GameManager.Instance.SavedScene().CompareTo("0_Exterior") == 0)
        {
            contText.color = new Color(.6f, .6f, .6f);
        }
        GameManager.Instance.Cutscene(false);
        audioS = GetComponent<AudioSource>();
        StartCoroutine(DoLogoIntro());
        if (GameManager.Instance.bossDefeated)
        {
            Destroy(golem);
        }
    }
    private void Update()
    {
        jukebox.volume = GameManager.Instance.masterVolume * GameManager.Instance.musicVolume;
    }

    #region MAIN MENU NAVIGATION
    public void NewGame()
    {
        audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        if (GameManager.Instance.SavedScene().CompareTo("0_Exterior") == 0)
        {
            TrueNewGame();
        }
        else
        {
            doubleCheckPanel.UIFadeIn();
        }
        
    }

    public void ConfirmNewGame()
    {
        doubleCheckPanel.UIFadeOutPiece();
        audioS.PlayOneShot(sounds[2], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        TrueNewGame();
    }

    public void DenyNewGame()
    {
        doubleCheckPanel.UIFadeOutPiece();
        audioS.PlayOneShot(sounds[3], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
    }

    public void TrueNewGame()
    {
        StartCoroutine(LoadNewGame());
        
    }

    public void ContinueGame()
    {
        if(GameManager.Instance.SavedScene().CompareTo("0_Exterior") != 0)
        {
            audioS.PlayOneShot(sounds[2], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
            StartCoroutine(LoadSavedGame());
        }
           
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        camControl.CamToCredits();
        creditsAnimator.Play("CreditScroll", 0, 0);
    }

    public void Options()
    {
        audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        camControl.CamToOptions();
    }

    public void BacktoMain()
    {
        audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        camControl.CamToMain();
    }
    #endregion

    public void ToggleTutorial()
    {
        if ((tutToggle.isOn && !GameManager.Instance.tutorialText) || (!tutToggle.isOn && GameManager.Instance.tutorialText))
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

            audioS.PlayOneShot(sounds[7], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        }
    }

    private IEnumerator LoadNewGame()
    {
        // Add a method to GameManager to do some kind of overwrite on existing save data and call it here
        Destroy(logo);
        GameManager.Instance.WipeSaveData();
        camControl.CamToNew();
        yield return new WaitForSeconds(.35f);
        SceneLoader.Instance.OnForcedTransition("0_Exterior");
        yield return null;
    }

    private IEnumerator LoadSavedGame()
    {
        Destroy(logo);
        camControl.CamToCont();
        yield return new WaitForSeconds(.35f);
        GameManager.Instance.ReadInSaveData();
        yield return null;
    }

    private IEnumerator DoLogoIntro()
    {
        yield return new WaitForSeconds(.41f);
        audioS.PlayOneShot(sounds[4], GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(2.02f);
        audioS.PlayOneShot(sounds[5], GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(7f);
        jukebox.Play();
        yield return new WaitForSeconds(3f);
        Destroy(logo);
        yield return null;
    }

}
