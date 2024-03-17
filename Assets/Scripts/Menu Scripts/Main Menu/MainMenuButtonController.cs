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


    private void Start()
    {
        if (GameManager.Instance.SavedScene().CompareTo("0_Exterior") == 0)
        {
            contText.color = new Color(.6f, .6f, .6f);
        }
        GameManager.Instance.Cutscene(false);
    }
    private void Update()
    {
        
    }

    public void NewGame()
    {
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
        TrueNewGame();
    }

    public void DenyNewGame()
    {
        doubleCheckPanel.UIFadeOutPiece();
    }

    public void TrueNewGame()
    {
        StartCoroutine(LoadNewGame());
    }

    public void ContinueGame()
    {
        if(GameManager.Instance.SavedScene().CompareTo("0_Exterior") != 0)
        {
            StartCoroutine(LoadSavedGame());
        }
           
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        camControl.CamToCredits();
        creditsAnimator.Play("CreditScroll", 0, 0);
    }

    public void Options()
    {
        camControl.CamToOptions();
    }

    public void BacktoMain()
    {
        camControl.CamToMain();
    }

    private IEnumerator LoadNewGame()
    {
        // Add a method to GameManager to do some kind of overwrite on existing save data and call it here
        GameManager.Instance.WipeSaveData();
        camControl.CamToNew();
        yield return new WaitForSeconds(.35f);
        SceneLoader.Instance.OnForcedTransition("0_Exterior");
        yield return null;
    }

    private IEnumerator LoadSavedGame()
    {
        camControl.CamToCont();
        yield return new WaitForSeconds(.35f);
        GameManager.Instance.ReadInSaveData();
        yield return null;
    }

}
