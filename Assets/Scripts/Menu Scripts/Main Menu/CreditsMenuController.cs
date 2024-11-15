using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenuController : MonoBehaviour
{
    float showTime = 4f;
    float showTimer = 0f;

    float creditsTime = 120f;
    float creditsTimer;

    [SerializeField] FadeUI button;
    [SerializeField] AudioSource jukebox;
    [SerializeField] AudioClip lastDraw;

    bool showing = false;
    void Start()
    {
        creditsTimer = creditsTime;
        jukebox.PlayOneShot(lastDraw, GameManager.Instance.masterVolume);
    }

    // Update is called once per frame
    void Update()
    {
        jukebox.volume = GameManager.Instance.masterVolume * GameManager.Instance.musicVolume;
        creditsTimer -= Time.deltaTime;

        if(creditsTimer < creditsTime - 1.5f && !jukebox.isPlaying)
        {
            jukebox.Play();
        }

        if(creditsTimer > 0)
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
                button.UIFadeIn();
            }

            if (showing && showTimer <= 0)
            {
                // Fade the button out
                showing = false;
                button.UIFadeOutPiece();
            }
        }
        else
        {
            Debug.Log("Time's up");
            if(!showing)
            {
                // Fade the button in for all time always
                button.UIFadeIn();
            }
        }
        
    }

    public void SkipButton()
    {
        SceneLoader.Instance.OnForcedPlayerTransition("23_MainMenu");
    }
}
