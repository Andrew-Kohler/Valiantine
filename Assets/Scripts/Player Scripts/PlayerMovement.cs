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

    bool activeCoroutine = false;
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
                rb.velocity = new Vector3(0f, 0f, 0f);
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
        
    }

    public void SetBattleIdlePosition()
    {
        idleBattlePosition = this.transform.position;
    }

    public Vector3 GetBattleIdlePosition()
    {
        return idleBattlePosition;
    }

    public void MovePlayerToPoint(Vector3 point)
    {
        StartCoroutine(MoveToPoint(point));
    }


    private IEnumerator MoveToPoint(Vector3 point)
    {
        activeCoroutine = true;
        direction = (point - this.transform.position).normalized * 800f;
        while (Vector3.Distance(this.transform.position, point) > 3f)
        {
            
            rb.velocity = new Vector3(direction.x, 0f, direction.z) * Time.deltaTime;
            yield return null;
        }

        activeCoroutine = false;
        
    }

}
