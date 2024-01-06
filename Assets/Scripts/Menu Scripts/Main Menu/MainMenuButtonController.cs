using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonController : MonoBehaviour
{
    [SerializeField] MainMenuCam camControl;
    [SerializeField] Animator creditsAnimator;

    public void NewGame()
    {
        camControl.CamToNew();
    }

    public void ContinueGame()
    {
        camControl.CamToCont();
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

}
