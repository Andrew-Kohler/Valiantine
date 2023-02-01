/*
Indicator Action
Used on:    GameObject - Action indicator group
For:    Control class for the action indicators in battle
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAction : Indicator
{
    [SerializeField] float delay = .2f;         // 
    [SerializeField] float moveSpeed = .04f;    // How fast the indicators move over the shorter side of the rectangle
    float moveSpeed2;                           // How fast the indicators move over the longer side of the rectangle

    [SerializeField] float rotationSpeed;       // How fast a rectangle spins when it moves to the front/bottom position
    float rotationStep;                         // The value that gets incremented by rotation speed       

    float horizontalInput;                      // Horizontal input from the player
    bool activeCoroutine;                       // Tells us if a coroutine is active so we don't try and start it every update loop
    bool keepGoingCheck;                        // Boolean that allows us to check if the player has kept holding the horizontal input because they want to spin the indicators

    new void Start()
    {
        activeCoroutine = false;
        keepGoingCheck = true;
        moveSpeed2 = moveSpeed * 1.582f;    // The long side of the 'rectangle' the indicators form is 1.582 times longer than the short side, so to travel it in a way
                                            // that looks proportional, indicators must travel 1.582 times as fast
        rotationSpeed = 360f * moveSpeed / 3.5f; // Establishes rotation speed as proportional to moveSpeed so that only one full rotation occurs (that's why the 3.5 is there)
        rotationStep = 0;

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        SetColors();
        horizontalInput = Input.GetAxis("Horizontal");

        if (GameManager.Instance.isBattle() && !activeCoroutine)
        {
            SetPositions();

            if (horizontalInput > 0) // Indicates a rightward movement
            {
                StartCoroutine(RightCoroutine());
            }
            else if (horizontalInput < 0) // Indicates a leftward movement
            {
                StartCoroutine(LeftCoroutine());
            }
            if (horizontalInput == 0)
            {
                keepGoingCheck = true;
            }
        }
    }

    void SetColors()    // If an indicator is in the front, it is set to its normal brightness; otherwise, it is greyed out to highlight the current selection
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if (i != 0)
            {
                indicators[i].GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f);
            }
            else
            {
                indicators[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    public string GetLeadBox()  // Method used by BattleManager and the Battle UI View to determine which action is being considered
    {
        if (activeCoroutine)
        {
            return "";
        }
        else
        {
            return indicators[0].name;
        }

    }

    IEnumerator LeftCoroutine() // The timed sequence which rotates the indicators a quarter-circle left
    {
        activeCoroutine = true;

        while (Vector3.Distance(indicators[0].transform.position, onePos) > .01)    // Move the indicators towards their positions to make it feel natural
        {
            indicators[0].transform.position = Vector3.MoveTowards(indicators[0].transform.position, onePos, moveSpeed2);
            indicators[1].transform.position = Vector3.MoveTowards(indicators[1].transform.position, twoPos, moveSpeed);
            indicators[2].transform.position = Vector3.MoveTowards(indicators[2].transform.position, threePos, moveSpeed2);
            indicators[3].transform.position = Vector3.MoveTowards(indicators[3].transform.position, zeroPos, moveSpeed);

            indicators[3].transform.rotation = Quaternion.Euler(0f, indicators[3].transform.rotation.y + rotationStep, 0f);
            rotationStep += rotationSpeed;

            yield return null;
        }

        GameObject temp = indicators[3];    // Formally assigning the indicators their new positions
        for (int i = indicators.Length - 2; i >= 0; i--)
        {
            indicators[i + 1] = indicators[i];
        }
        indicators[0] = temp;

        indicators[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rotationStep = 0f;

        if (keepGoingCheck)
        {
            StartCoroutine(KeepGoingCheckCoroutine());
        }
        else
        {
            activeCoroutine = false;
        }

        yield return null;
    }

    IEnumerator RightCoroutine() // The timed sequence which rotates the indicators a quarter-circle right
    {
        activeCoroutine = true;

        while (Vector3.Distance(indicators[0].transform.position, threePos) > .01)    // Move the indicators towards their positions to make it feel natural
        {
            indicators[0].transform.position = Vector3.MoveTowards(indicators[0].transform.position, threePos, moveSpeed);
            indicators[1].transform.position = Vector3.MoveTowards(indicators[1].transform.position, zeroPos, moveSpeed2);
            indicators[2].transform.position = Vector3.MoveTowards(indicators[2].transform.position, onePos, moveSpeed);
            indicators[3].transform.position = Vector3.MoveTowards(indicators[3].transform.position, twoPos, moveSpeed2);

            indicators[1].transform.rotation = Quaternion.Euler(0f, indicators[1].transform.rotation.y + rotationStep, 0f);
            rotationStep -= rotationSpeed;

            yield return null;
        }

        GameObject temp = indicators[0];    // Formally assing the indicators their new positions
        for (int i = 1; i <= indicators.Length - 1; i++)
        {
            indicators[i - 1] = indicators[i];
        }
        indicators[3] = temp;

        indicators[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rotationStep = 0f;

        if (keepGoingCheck)
        {
            StartCoroutine(KeepGoingCheckCoroutine());
        }
        else
        {
            activeCoroutine = false;
        }


        yield return null;
    }

    IEnumerator KeepGoingCheckCoroutine()
    {
        yield return new WaitForSeconds(delay);
        if (horizontalInput != 0)
        {
            keepGoingCheck = false;
        }
        activeCoroutine = false;
    }

    public IEnumerator DoFlashIn()
    {
        IndicatorFlash flash = GameObject.Find("Flash").GetComponent<IndicatorFlash>();

        activeCoroutine = true;
        flash.enabled = true;
        StartCoroutine(flash.DoFlashOut()); // Start the coroutine for fading the flash effect out (this is what create the flash effect)

        alpha = 0;              // In order to always have this coroutine flash the indicators in, reset alpha to 0
        while (alpha <= 1)
        {
            for (int i = 0; i < 4; i++)  // Move the indicators towards full opacity
            {
                sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, alpha);
            }
            alpha += alphaStep;
            Debug.Log("Action alpha step:" + alphaStep);
            yield return null;
        }

        enabled = true; // Enable this script (for when you first enter the battle)
        activeCoroutine = false;

        yield return null;
    }
}
