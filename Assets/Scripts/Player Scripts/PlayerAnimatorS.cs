using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorS : MonoBehaviour
{
    public enum AnimationAxis {Rows, Columns}

    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 5.4f;
    [SerializeField] private int animationIndex = 0;

    private float deltaT;
    float horizontalInput;
    float verticalInput;
    int direction;

    private int idleLeftIndex = 6;
    private int idleRightIndex = 7;
    private int idleForwardsIndex = 5;
    private int idleBackIndex = 4;
    private int walkLeftIndex = 2;
    private int walkRightIndex = 3;
    private int walkForwardsIndex = 1;
    private int walkBackIndex = 0;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        deltaT = 0;
    }

    private void Update()
    {
        

        // The logic that plays the animation
        // Select shader property names
        if (!GameManager.Instance.isSettings())
        {
            // The logic that determines what animation should be played
            if (GameManager.Instance.canMove()) // Logic for idle and walk cycles that occur during normal exploration
            {
                animationSpeed = 8f;
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");

                if (isMoving(horizontalInput, verticalInput))
                {
                    setDirection(horizontalInput, verticalInput);
                    if (direction == 0)
                    {
                        animationIndex = walkRightIndex;
                    }
                    else if (direction == 1)
                    {
                        animationIndex = walkLeftIndex;
                    }
                    else if (direction == 2)
                    {
                        animationIndex = walkBackIndex;
                    }
                    else if (direction == 3)
                    {
                        animationIndex = walkForwardsIndex;
                    }
                }
                else
                {
                    animationSpeed = 5.4f;
                    if (direction == 0)
                    {
                        animationIndex = idleRightIndex;
                    }
                    else if (direction == 1)
                    {
                        animationIndex = idleLeftIndex;
                    }
                    else if (direction == 2)
                    {
                        animationIndex = idleBackIndex;
                    }
                    else if (direction == 3)
                    {
                        animationIndex = idleForwardsIndex;
                    }
                }
            }

            else if (GameManager.Instance.isBattle())
            {
                animationSpeed = 5.4f;
                animationIndex = idleRightIndex;

            }

            string clipKey, frameKey;
            if (axis == AnimationAxis.Rows)
            {
                clipKey = rowProperty;
                frameKey = colProperty;
            }
            else
            {
                clipKey = colProperty;
                frameKey = rowProperty;
            }

            // Animate
            int frame = (int)(deltaT * animationSpeed);

            deltaT += Time.deltaTime;
            if (frame >= 8) // Might be messing with this soon!
            {
                deltaT = 0;
                frame = 0;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
        
    }

    private bool isMoving(float hInput, float vInput)
    {
        if(horizontalInput != 0 || verticalInput != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void setDirection(float hInput, float vInput)
    {
        if (horizontalInput > 0 && verticalInput == 0) // Right
        {
            direction = 0;
        }
        else if (horizontalInput < 0 && verticalInput == 0) // Left
        {
            direction = 1; 
        }
        else if (horizontalInput == 0 && verticalInput > 0) // Up
        {
            direction = 2;
        }
        else if (horizontalInput == 0 && verticalInput < 0) // Down
        {
            direction = 3;
        }
    }
}


