using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Arrangement: An arrow and a gameObject with the text and backboard, all under one thingy
// My attempt to make a single class for the selector arrows rather than having them spread across 2.5 different implementations

public class InventoryArrow : MonoBehaviour
{
    [SerializeField] private Image arrow;
    [SerializeField] private GameObject usePrompt;
    [SerializeField] private bool hideArrowToggle;  // Whether we want the arrow to be hidden when something is selected
    [SerializeField] private string animName;
    //[SerializeField] private Vector2 startPos;

    private Animator arrowAnim;


    private bool selected;

    private void Start()
    {
        arrowAnim = arrow.GetComponent<Animator>();
    }
    private void OnEnable()
    {
        arrow.color = Color.white;
        selected = false;
        usePrompt.SetActive(false);
        //moveArrow(startPos, .01f);
    }

    public void moveArrow(Vector2 targetPos, float step, bool spin)    // For the smooth movement of the arrow
    {
        StopAllCoroutines();
        StartCoroutine(DoPosition(targetPos, step));
        if (spin)
        {
            arrowAnim.Play(animName, 0, 0);
        }
            
    }

    public void selectorSwap()  // Swaps between selected and unselected versions of arrow
    {
        if (selected)
        {
            arrow.color = Color.white;
            selected = false;
            usePrompt.SetActive(false);
        }
        else
        {
            if (hideArrowToggle)
            {
                arrow.color = Color.clear;
            }
            selected = true;
            usePrompt.SetActive(true);
        }
    }

    IEnumerator DoPosition(Vector2 targetPos, float step)
    {
        Vector2 velocity = Vector2.zero;    // Initial velocity values for the damping functions

        while (Vector2.Distance(transform.position, targetPos) >= .01f)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref velocity, step); // Move line position

            yield return null;
        }

        yield return null;
    }
}
