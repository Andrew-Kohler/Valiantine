using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;
    
    float horizontalInput;
    float verticalInput;
    bool isMoving;
    int direction;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.canMove())
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            // Code for determining the direction the player is facing
            if (horizontalInput > 0 && verticalInput == 0)
            {
                direction = 0; // Right
                sr.flipX = false;
            }
            else if (horizontalInput < 0 && verticalInput == 0)
            {
                direction = 1; // Left
                sr.flipX = true;
            }
            else if (horizontalInput == 0 && verticalInput > 0)
            {
                direction = 2; // Up - away from camera
            }
            else if (horizontalInput == 0 && verticalInput < 0)
            {
                direction = 3; // Down - towards camera
            }

            if(horizontalInput != 0 || verticalInput != 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            anim.SetInteger("direction", direction);
            anim.SetBool("isMoving", isMoving);
        } 
        else if (GameManager.Instance.isBattle())
        {

        }
    }
}
