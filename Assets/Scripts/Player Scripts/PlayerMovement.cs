/*
Player Movement
Used on:    Player
For:    Allows for keyboard control of player while exploring
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    float movementSpeed = 7f;
    float horizontalInput;
    float verticalInput;

    public bool activeCoroutine = false;
    public bool GettingClose = false; // For animation purposes when moving to attack
    private Vector3 direction;
    private Vector3 idleBattlePosition; // Where the player landed when battle began
    private Vector3 orderedBattlePosition;  // Where the player is headed when ordered to move

    public delegate void OnInteractButton();
    public static event OnInteractButton onInteractButton;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.canMove()) // If we are allowed to move
        {
            // Uses old input system; I'd like to try the new one on my next project
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            //rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        }
        else // Freezes the player if something else is going on, like entering a menu
        {
            if (!activeCoroutine)
            {
                horizontalInput = 0;
                verticalInput = 0;
                //rb.velocity = new Vector3(0f, 0f, 0f);
            }
            
        }

        if (Input.GetButtonDown("Interact"))
        {
            onInteractButton?.Invoke();
        }


    }   // End of Update()

    private void FixedUpdate()
    {
        MovePlayer3D();
    }

    private void MovePlayer3D()
    {
        if (GameManager.Instance.canMove())
        {
            rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        
    }

    public void SetBattleIdlePosition()
    {
        idleBattlePosition = this.transform.position;
    }

    public Vector3 GetBattleIdlePosition()
    {
        return idleBattlePosition;
    }

    public void MovePlayerToPoint(Vector3 point, float distanceFromPoint)
    {
        StartCoroutine(MoveToPoint(point, distanceFromPoint));
    }


    private IEnumerator MoveToPoint(Vector3 point, float distanceFromPoint)
    {
        activeCoroutine = true;
        GettingClose = false;
        Vector3 direction = (point - this.transform.position);
        float yStop = this.gameObject.transform.position.y;

        // Method 2
        rb.AddForce(new Vector3(direction.x, 4f, direction.z), ForceMode.VelocityChange); //Mathf.Abs(direction.x * .27f)


        yield return new WaitUntil(()=> Mathf.Abs(this.transform.position.x - point.x) <= distanceFromPoint + .03f);
        GettingClose = true;

        activeCoroutine = false;
        yield return null;
        
    }

}
