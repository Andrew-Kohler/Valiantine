using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuButtonController : MonoBehaviour
{
    [SerializeField] MainMenuCam camControl;
    [SerializeField] Animator creditsAnimator;
    [SerializeField] FadeUI doubleCheckPanel;
    [SerializeField] TextMeshProUGUI contText;
    [SerializeField] GameObject logo;

    [SerializeField] private List<AudioClip> sounds;
    private AudioSource audioS;

    private void Start()
    {
        if (GameManager.Instance.SavedScene().CompareTo("0_Exterior") == 0)
        {
            contText.color = new Color(.6f, .6f, .6f);
        }
        GameManager.Instance.Cutscene(false);
        audioS = GetComponent<AudioSource>();
        StartCoroutine(DoLogoIntro());
    }
    private void Update()
    {
        
    }

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
        audioS.PlayOneShot(sounds[4], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(2.02f);
        audioS.PlayOneShot(sounds[5], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(10f);
        Destroy(logo);
        yield return null;
    }

}
