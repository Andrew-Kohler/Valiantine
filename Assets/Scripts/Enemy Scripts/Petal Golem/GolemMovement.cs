using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMovement : EnemyMovement
{
    public bool sleeping = false;   // The primary toggle of overworld Golem movement - if true, they do not move,and just sleep

    [SerializeField] private bool moveRandomly;    // Whether or not we are moving, WITHIN THE CONFINES OF THE NORMAL MOVEMENT ROUTINE
                                                   // That is to say, is it time to move in a given random direction, or time to stop?
    public bool LurchingAround => moveRandomly;
    private float moveRandomlyTimer = 2f;                         // A timer that is used to change cycles
    [SerializeField] private float moveRandomCycleTime = 2f; // How long we stay in either phase of the movement pattern
    private bool goX = false;
    private bool reversed = false;  // Used to ping-pong the Golem off of the enemy bounds if they try to stray outside of them when not chasing

    private float currentXDir;  // Either 1 or -1
    private float currentZDir;

    private float currentSpeed;

    public bool spawnedToFight; // For disabling the attempts of the spawned enemies to move about
    Vector3 pursuitVector;

    bool battleMovementEnabled = false;
    void Start()
    {
        base.Start();
        moveRandomly = false;
        currentSpeed = normalMovementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnedToFight) // If I'm an overworld enemy
        {

            if (!sleeping && GameManager.Instance.enemyCanMove()) // If we are not sleeping
            {

                if (moveRandomlyTimer <= 0) // If our timer has run out, reset it and swap our movement style
                {
                    moveRandomlyTimer = moveRandomCycleTime;
                    moveRandomly = !moveRandomly;
                    goX = generateAxis();
                    if (moveRandomly) // If we need to generate a new direction
                    {
                        reversed = false;
                        if (goX)
                        {
                            currentXDir = generateDir();
                            currentZDir = 0;
                        }
                        else
                        {
                            currentZDir = generateDir();
                            currentXDir = 0;
                        }
                    }
                }

                if (!enemyFollow.inBounds() && !reversed)   // Keeps the Golem from wandering out of enemy bounds
                {
                    reversed = true;    // "Reversed" prevents us from getting stuck in a loop on the edge where we're just perpetually stuck within the edge turning
                    if (goX)
                    {
                        currentXDir = currentXDir * -1;
                    }
                    else
                    {
                        currentZDir = currentZDir * -1;
                    }
                }

                if (moveRandomly) // If we are moving, our dir vector actually exists
                {
                    direction = new Vector3(currentXDir, 0f, currentZDir).normalized;
                }
                else // Otherwise, our dir is a flat 0
                {
                    direction = new Vector3(0f, 0f, 0f);
                }


                moveRandomlyTimer -= Time.deltaTime; // Decrement the timer
            }
            else if (sleeping)
            {
                direction = new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enemyCanMove()) // If we can move
        {
            rb.velocity = direction * currentSpeed * Time.fixedDeltaTime;
        }
    }

    // Private methods---------------------------------------------------------------
    private bool generateAxis() // Generates which axis we'll be moving along
    {
        bool x = true;
        if (Random.Range(-1, 1) == 0)
        {
            x = false;
        }
        return x;
    }
    private int generateDir() // Generates a random dir for the Forgotten Lover to walk in
    {
        int num = Random.Range(-1, 1);
        if (num == 0)
        {
            num = 1;
        }
        return num;
    }
}
