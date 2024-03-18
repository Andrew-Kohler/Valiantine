using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenuManager : MonoBehaviour
{
    [SerializeField] FadeUI buttons;

    [SerializeField] FadeUI goText1;
    [SerializeField] FadeUI goText2;
    [SerializeField] FadeUI goText3;
    [SerializeField] FadeUI goText4;

    [SerializeField] Button b1;
    [SerializeField] Button b2;
    [SerializeField] Button b3;
    [SerializeField] FadeUI skipButton;

    bool activeCoroutine;

    float showTime = 4f;
    float showTimer = 0f;
    bool showing;

    // Start is called before the first frame update
    void Start()
    {
        activeCoroutine = true;
        StartCoroutine(DoGameOverSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (activeCoroutine)
        {   
            if (showTimer > 0)
            {
                showTimer -= Time.deltaTime;
            }
            if (Input.GetButtonDown("Inventory"))
            {
                showing = true;
                showTimer = showTime;
                // Fade the button in
                skipButton.UIFadeIn();
            }

            if (showing && showTimer <= 0)
            {
                // Fade the button out
                showing = false;
                skipButton.UIFadeOutPiece();
            }
        }
        else
        {
            if(showing)
                skipButton.UIFadeOutPiece();
            // Fade the button out for all time always

        }
    }

    public void LoadLastSave()
    {
        b1.interactable = false;
        b2.interactable = false;
        b3.interactable = false;
        GameManager.Instance.ReadInSaveData();
    }

    public void ReturnToMain()
    {
        b1.interactable = false;
        b2.interactable = false;
        b3.interactable = false;
        SceneLoader.Instance.OnForcedPlayerTransition("23_MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SkipSequence()
    {
        StopAllCoroutines();
        activeCoroutine = false;
        buttons.HurryUp();
        goText1.HurryUp();
        goText2.HurryUp();
        goText3.HurryUp();
        goText4.HurryUp();
    }

    private IEnumerator DoGameOverSequence()
    {
        activeCoroutine = true;
        int readtime = 5;
        yield return new WaitForSeconds(3f);

        goText1.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        goText2.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        goText3.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        goText4.UIFadeIn();
        yield return new WaitForSeconds(readtime);

        buttons.UIFadeIn();
        activeCoroutine = false;
    }
}
