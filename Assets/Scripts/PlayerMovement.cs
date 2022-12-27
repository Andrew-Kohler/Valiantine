using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    float movementSpeed = 10f;
    string direction = "Right";

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Hello from Start");
        // ^ Good for print debugging

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Uses old input system; I'd like to try the new one on my next project

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        // Code for determining the direction the player is facing
        if(horizontalInput > 0 && verticalInput == 0)
        {
            direction = "Right";
        }
        else if(horizontalInput < 0 && verticalInput == 0)
        {
            direction = "Left";
        }
        else if(horizontalInput == 0 && verticalInput > 0)
        {
            direction = "Up";
        }
        else if(horizontalInput == 0 && verticalInput < 0)
        {
            direction = "Down";
        }

        //Debug.Log(direction);

    }   // End of Update()



}
