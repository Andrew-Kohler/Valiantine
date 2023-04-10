using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardinalController : MonoBehaviour
{
    public bool flyingAway;
    private bool activeCoroutine;
    private CardinalAnimatorS cardinalAnimator;
    private CardinalStateChange cardinalStateChange;
    [SerializeField] private GameObject cardinalSprite;
    [SerializeField] private Collider trigger;
    Rigidbody rb;
    private float dir;

    void Start()
    {
        activeCoroutine = false;
        flyingAway = false;
        rb = GetComponent<Rigidbody>();
        cardinalAnimator = cardinalSprite.GetComponent<CardinalAnimatorS>();
        cardinalStateChange = trigger.GetComponent<CardinalStateChange>();
    }

    void Update()
    {
        if (cardinalStateChange.flyAway)
        {
            StopAllCoroutines();
            StartCoroutine(DoFlyAway());
            cardinalStateChange.flyAway = false;
        }
            

        if (!activeCoroutine)
        {
            if (!flyingAway)
            {
                // Hop back and forth errantly
                // Randomly pick left or right and hop in that direction, and communicate that direction to the animator
                StartCoroutine(DoHop());
                
            }
        }
        
    }

    IEnumerator DoHop() // Hopping about routine
    {
        activeCoroutine = true;
        dir = Random.Range(0, 2);
        if(dir == 0)    // Right
        {
            cardinalAnimator.dir = 0;
            rb.velocity = new Vector3(1f, 2f, 0f);
        }
        else // Left
        {
            cardinalAnimator.dir = 1;
            rb.velocity = new Vector3(-1f, 2f, 0f);
        }
        yield return new WaitForSeconds(.5f);
        rb.velocity = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(1f);

        activeCoroutine=false;
        yield return null;
    }
    IEnumerator DoFlyAway() // Flying away routine
    {
        activeCoroutine = true;
        cardinalAnimator.flyingAway = true;
        float signF = 1f;
        if (dir == 0)    // Right
        {
            cardinalAnimator.dir = 0;

            //rb.velocity = new Vector3(2f, 4f, 0f);
        }
        else // Left
        {
            cardinalAnimator.dir = 1;
            signF = -1f;
            //rb.velocity = new Vector3(-2f, 4f, 0f);
        }

        int count = 90;
        while(count > 0)
        {
            rb.velocity = new Vector3(4f *signF, 10f, 0f);
            count--;
            yield return new WaitForSeconds(.05f);
        }

        //yield return new WaitForSeconds(5f);
        activeCoroutine = false;
        this.gameObject.SetActive(false);
        
        yield return null;
    }
}
