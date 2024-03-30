using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardinalController : MonoBehaviour
{
    [SerializeField] private GameObject cardinalSprite;
    [SerializeField] private Collider trigger;

    private CardinalAnimatorS cardinalAnimator;
    private CardinalStateChange cardinalStateChange;
    
    Rigidbody rb;
    [SerializeField] private List<AudioClip> sounds;
    private AudioSource audioS;

    public bool flyingAway;

    private bool activeCoroutine;
    private float dir;

    private float xHopVelocity = 1f;    // Speeds governing how fast/high the cardinal hops and flies away
    private float yHopVelocity = 2f;
    private float xFlightVelocity = 6f;
    private float yFlightVelocity;      // Starting value for y flight velocty
    private float yVelocityMultiplier = 1.4f;   // Rate at which y velocity increases
    private float maxYVelocity = 15f;   // Cap on growth of y velocity
    private float flightTime = 4.5f;    // Time the cardinal flies for in seconds before deloading

    void Start()
    {
        activeCoroutine = false;
        flyingAway = false;
        rb = GetComponent<Rigidbody>();
        audioS = GetComponent<AudioSource>();
        cardinalAnimator = cardinalSprite.GetComponent<CardinalAnimatorS>();
        cardinalStateChange = trigger.GetComponent<CardinalStateChange>();

        yFlightVelocity = 1.2f;
    }

    void Update()
    {
        if (cardinalStateChange.flyAway)
        {
            StopAllCoroutines();
            StartCoroutine(DoFlyAway());
            cardinalStateChange.flyAway = false;
        }
            

        if (!activeCoroutine && !GameManager.Instance.isSettings())
        {
            if (!flyingAway)
            {
                StartCoroutine(DoHop());
                
            }
        }
        
    }

    IEnumerator DoHop() // Hopping about routine
    {
        activeCoroutine = true;

        yield return new WaitForSeconds(Random.Range(0f, 1f)); // Randomizes hop time to de-sync groups of birds

        dir = Random.Range(0, 2);   // Randomly decide between left and right
        if(dir == 0)    // Right
        {
            cardinalAnimator.dir = 0;
            rb.velocity = new Vector3(xHopVelocity, yHopVelocity, 0f);
        }
        else // Left
        {
            cardinalAnimator.dir = 1;
            rb.velocity = new Vector3(-xHopVelocity, yHopVelocity, 0f);
        }
        dir = Random.Range(0, 3);
        audioS.PlayOneShot(sounds[(int)dir], GameManager.Instance.environmentVolume * GameManager.Instance.masterVolume);

        yield return new WaitForSeconds(.5f);
        rb.velocity = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(Random.Range(2f, 6f));    // Randomizes hop time to de-sync groups of birds


        activeCoroutine = false;
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
        }
        else // Left
        {
            cardinalAnimator.dir = 1;
            signF = -1f;
        }

        float count = flightTime / .05f;
        yFlightVelocity = 1.2f;
        audioS.PlayOneShot(sounds[3], GameManager.Instance.environmentVolume * GameManager.Instance.masterVolume);
        while (count > 0) // Flies the cardinal away until it's out of camera view and good to de-load
        {
            rb.velocity = new Vector3(xFlightVelocity * signF, yFlightVelocity, 0f);
            
            if(yFlightVelocity < maxYVelocity)
                yFlightVelocity = yFlightVelocity * yVelocityMultiplier;

            count--;
            yield return new WaitForSeconds(.05f);
        }
        activeCoroutine = false;
        this.gameObject.SetActive(false);
        
        yield return null;
    }
}