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
            rb.velocity = new Vector3(0f, 0f, 0f);
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
        rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);
    }



}
