using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    // This combines everything I've learned up to this point, aside from a vastly complex inventory and stats system
    // And view managing and scene loading
    // It's very cool in that way

    [SerializeField] private float arrowSpeed = 2f; // How fast the arrow moves
    //[SerializeField] private float delay = 2f;      // How long we hold before the arrow gets to go willy nilly if the player is holding an input
    [SerializeField] private float heightAdd = 2.5f;   // How high above the enemy the arrow is
    [SerializeField] private float rotationSpeed = .5f;

    private GameObject[] options;   // The things we are choosing between

    private float verticalInput;  // Vertical input from the player
    private int currentPosition;
    private bool activeCoroutine;
    private bool keepGoingCheck;
    float rotationStep = 0f;

    public int type;


    // Start is called before the first frame update
    void Start()
    {
        activeCoroutine = false;
    }

    private void Update()
    {
        //verticalInput = Input.GetAxis("Vertical");

        if (GameManager.Instance.isBattle() && !activeCoroutine)
        {
            this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.y + rotationStep, 0f);
            rotationStep += rotationSpeed * Time.deltaTime;
            if(rotationStep > 360)
            {
                rotationStep = 0f;
            }
            if (Input.GetButtonDown("Inventory Up")) // Indicates an upward movement
            {
                StartCoroutine(UpCoroutine());
            }
            else if (Input.GetButtonDown("Inventory Down")) // Indicates a downward movement
            {
                StartCoroutine(DownCoroutine());
            }
            else if (Input.GetButtonDown("Interact"))
            {
                BattleManager.Instance.SetAttackTarget(options[currentPosition]);
            }
            /*if (verticalInput == 0)
            {
                keepGoingCheck = true;
            }*/
        }
    }

    // Public methods ----------------------------------------------------------

    public void SetValues(GameObject[] options, int currentPosition)
    {
        this.options = options;
        this.currentPosition = currentPosition;
    }

    // Private methods ----------------------------------------------------------

    // Coroutines ----------------------------------------------------------

    IEnumerator UpCoroutine() // The timed sequence which moves the selection arrow up (back from the camera)
    {
        activeCoroutine = true;

        Vector3 newPos;
        if(currentPosition != 0)    // If we aren't going to negative overflow ourselves
        {
            currentPosition = currentPosition - 1;
            if (options[currentPosition].GetComponent<Stats>().getDowned())
            {
                StartCoroutine(UpCoroutine());
            }

        }
        else    // If we ARE going to negative overflow ourselves
        {
            currentPosition = options.Length - 1;
            if (options[currentPosition].GetComponent<Stats>().getDowned())
            {
                StartCoroutine(UpCoroutine());
            }
        }
        newPos = new Vector3(options[currentPosition].transform.position.x, options[currentPosition].transform.position.y + heightAdd, options[currentPosition].transform.position.z - .01f);

        while (Vector3.Distance(this.transform.position, newPos) > .01)    // Move the indicators towards their positions to make it feel natural
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, newPos, arrowSpeed * Time.deltaTime);

            yield return null;
        }
        activeCoroutine = false;

        yield return null;
    }

    IEnumerator DownCoroutine() 
    {
        activeCoroutine = true;

        Vector3 newPos;
        if (currentPosition != options.Length - 1)    // If we aren't going to overflow ourselves
        {
            currentPosition = currentPosition + 1;
            if (options[currentPosition].GetComponent<Stats>().getDowned())
            {
                StartCoroutine(DownCoroutine());
            }

        }
        else    // If we ARE going to overflow ourselves
        {
            currentPosition = 0;
            if (options[currentPosition].GetComponent<Stats>().getDowned())
            {
                StartCoroutine(DownCoroutine());
            }
        }
        newPos = new Vector3(options[currentPosition].transform.position.x, options[currentPosition].transform.position.y + heightAdd, options[currentPosition].transform.position.z - .01f);

        while (Vector3.Distance(this.transform.position, newPos) > .01)    // Move the indicators towards their positions to make it feel natural
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, newPos, arrowSpeed * Time.deltaTime);

            yield return null;
        }

        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rotationStep = 0f;

        activeCoroutine = false;

        yield return null;
    }

   /* IEnumerator KeepGoingCheckCoroutine()
    {
        yield return new WaitForSeconds(delay);
        if (verticalInput != 0)
        {
            keepGoingCheck = false;
        }
        activeCoroutine = false;
    }*/
}
