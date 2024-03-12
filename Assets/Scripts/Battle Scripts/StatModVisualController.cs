using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModVisualController : MonoBehaviour
{
    // So the way this one works is that we just enable and disable it at different times in the battle

    private bool activeCoroutine;

    [SerializeField] private ParticleSystem particleIndicatorUp;
    [SerializeField] private ParticleSystem particleIndicatorDown;
    [SerializeField] private float cycleTime;

    [SerializeField] private GameObject ATKText;    // Is this a little stupid? Sure. But I refuse to spawn/destroy all of them,
    [SerializeField] private GameObject DEFText;    // because that's just as silly in something so small-scale.
    [SerializeField] private GameObject SPDText;

    [SerializeField] private GameObject Arrow;
    [SerializeField] private GameObject BackerArrow1;
    [SerializeField] private GameObject BackerArrow2;
    private Animator arrowAnim;
    private Animator backer1Anim;
    private Animator backer2Anim;


    private Stats stats;    // Stats of whatever this is on

    private bool playerTurn = false;
    private bool enemyTurn = false;
    private bool emptyQueue;

    // So I need a queue of these things to burn through wheneve3r it isn't the player's turn
    // A queue of pairs, and so long as the queue has things in it, we're doing this
    // So I need another boolean to indicate that is the player's turn rather than enabling and disabling it

    (int Type, bool up) t;
    Queue<(int Type, bool up)> indicatorsToPlay;

    private void OnEnable()
    {
        activeCoroutine = false;
        //Debug.Log("Displays are on");
    }

    private void OnDisable()
    {
        //Debug.Log("Displays are off");
        StopAllCoroutines();
        HideAll();
    }

    private void Start()
    {
        stats = GetComponent<Stats>();
        arrowAnim = Arrow.GetComponent<Animator>();
        backer1Anim = BackerArrow1.GetComponent<Animator>();
        backer2Anim = BackerArrow2.GetComponent<Animator>();
        indicatorsToPlay = new Queue<(int Type, bool up)>();
        HideAll();
    }

    private void Update()
    {
        // If we are in battle and it is the player's turn, the coroutine should be on loop for EVERYBODY
        if (playerTurn)
        {
            if (!activeCoroutine)
            {
                StartCoroutine(DoIndicatorCycles());
            }
        }
/*        else
        {
            if (!activeCoroutine && indicatorsToPlay.Count > 0) // For playing buff and debuff FX generated by moves
            {
                emptyQueue = false;
                StartCoroutine(DoIndicatorBurst(indicatorsToPlay.Peek().Type, indicatorsToPlay.Peek().up));

                indicatorsToPlay.Dequeue();
                
            }
*//*
            else if (!activeCoroutine && indicatorsToPlay.Count == 0 && !emptyQueue)
            {
                StartCoroutine(CloseIndicatorBurst());
            }*//*
        }*/
        

        

    }

    // Public methods ---------------------------------------
    public void SetPlayerTurn(bool yes)
    {
        playerTurn = yes;
        particleIndicatorDown.Clear();
        particleIndicatorUp.Clear();
        if (!playerTurn)
        {
            StopAllCoroutines();
            HideAll();
            activeCoroutine = false;
        }
        else
        {
            activeCoroutine = false;
        }
    }


    /*    public void PlayStatChange(int stat, bool up) // A method for when whoever this is attatched to gets buffed or debuffed
        {
            //StartCoroutine(DoIndicatorBurst(stat, up));
            //t = (stat, up);
            indicatorsToPlay.Enqueue((stat, up));
        }*/

    // Private methods -------------------------------------
    private Color GetColorOfStat(int stat)
    {
        if(stat == 0)
            return Color.red;
        else if(stat == 1)
            return Color.green;
        else if(stat == 2)
            return Color.blue;

        return Color.white;
    }
    private void ToggleDirection(int stat, bool up)   // Toggles direction depending on whether the input is true or false
    {
        var toggleEmissionUp = particleIndicatorUp.emission;
        var toggleEmissionDown = particleIndicatorDown.emission;
        var UpColor = particleIndicatorUp.main;
        var DownColor = particleIndicatorDown.main;

        if (up)
        {
            toggleEmissionUp.enabled = true;
            toggleEmissionDown.enabled = false;
            UpColor.startColor = GetColorOfStat(stat);
            particleIndicatorUp.Play();
           

        }
        else
        {
            toggleEmissionUp.enabled = false;
            toggleEmissionDown.enabled = true;
            DownColor.startColor = GetColorOfStat(stat);
            particleIndicatorDown.Play();
        }

        
    }

    private void ShowArrow(int stat, bool up)   // Shows an up or down arrow corresponding to the given stat - ATK, DEF, SPD, 0, 1, 2
    {
        Arrow.SetActive(true); 

        if (up)
        {
            backer1Anim.Play("ATKUp", 0, 0);
            backer2Anim.Play("ATKUp", 0, 0);
            if (stat == 0)
                arrowAnim.Play("ATKUp", 0, 0);
            else if (stat == 1)
                arrowAnim.Play("DEFUp", 0, 0);
            else if (stat == 2)
                arrowAnim.Play("SPDUp", 0, 0);
        }
        else
        {
            backer1Anim.Play("ATKDown", 0, 0);
            backer2Anim.Play("ATKDown", 0, 0);
            if (stat == 0)
                arrowAnim.Play("ATKDown", 0, 0);
            else if (stat == 1)
                arrowAnim.Play("DEFDown", 0, 0);
            else if (stat == 2)
                arrowAnim.Play("SPDDown", 0, 0);
        }
    }

    private void HideAll() // Hides all of the particle FX, words, arrows
    {

        var toggleEmissionUp = particleIndicatorUp.emission;
        var toggleEmissionDown = particleIndicatorDown.emission;

        toggleEmissionUp.enabled = false;
        toggleEmissionDown.enabled = false;

        ATKText.SetActive(false);
        DEFText.SetActive(false);
        SPDText.SetActive(false);

        Arrow.SetActive(false);
    }

    // Coroutines --------------------------------------
    private IEnumerator DoIndicatorCycles() // A coroutine that cycles between displays
    {
        activeCoroutine = true;

        // If there is a difference between stat and raw stat
        if(stats.GetATK() != stats.GetATKRaw()) 
        {
            ToggleDirection(0, stats.GetATK() > stats.GetATKRaw());// Set the particles in the correct direction
            ATKText.SetActive(true);                            // Show the ATK graphic and hide the SPD one
            SPDText.SetActive(false);
            DEFText.SetActive(false);
            ShowArrow(0, stats.GetATK() > stats.GetATKRaw());   // Feed the boolean to a function that shows the right arrow
            yield return new WaitForSeconds(cycleTime);
            particleIndicatorUp.Clear();
            particleIndicatorDown.Clear();
        }
        else
        {
            HideAll();
        }

        // Repeat for DEF and SPD
        if (stats.GetDEF() != stats.GetDEFRaw())
        {
            ToggleDirection(1, stats.GetDEF() > stats.GetDEFRaw());
            DEFText.SetActive(true);                            
            ATKText.SetActive(false);
            SPDText.SetActive(false);
            ShowArrow(1, stats.GetDEF() > stats.GetDEFRaw());
            yield return new WaitForSeconds(cycleTime);
            particleIndicatorUp.Clear();
            particleIndicatorDown.Clear();
        }
        else
        {
            HideAll();
        }

        if (stats.GetSPD() != stats.GetSPDRaw())
        {
            ToggleDirection(2, stats.GetSPD() > stats.GetSPDRaw());
            SPDText.SetActive(true);
            ATKText.SetActive(false);
            DEFText.SetActive(false);
            ShowArrow(2, stats.GetSPD() > stats.GetSPDRaw());
            yield return new WaitForSeconds(cycleTime);
            particleIndicatorUp.Clear();
            particleIndicatorDown.Clear();
        }
        else
        {
            HideAll();
        }

        activeCoroutine = false;
    }

/*    private IEnumerator DoIndicatorBurst(int stat, bool up)
    {
        activeCoroutine = true;
        ToggleDirection(stat, up);
        yield return new WaitForSeconds(.2f);
        HideAll();

        activeCoroutine = false;
        yield return null;

    }

    private IEnumerator CloseIndicatorBurst()
    {
        activeCoroutine = true;
        emptyQueue = true;
        HideAll();
        yield return new WaitForSeconds(.4f);
        activeCoroutine = false;
    }*/

    
}
