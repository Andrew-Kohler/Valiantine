using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoverMovement : EnemyMovement
{
    bool active = false; // Whether or not the Lover is chasing the player
    public bool ChasingPlayer => active;
    bool movementEnabled = true; // For making the appearance of the corpse lurching

    [SerializeField] private bool moveRandomly;    // Whether or not we are moving, WITHIN THE CONFINES OF THE NORMAL MOVEMENT ROUTINE
                                  // That is to say, is it time to move in a given random direction, or time to stop?
    public bool LurchingAround => moveRandomly;
    private float moveRandomlyTimer = 2f;                         // A timer that is used to change cycles
    [SerializeField] private float moveRandomCycleTime = 2f; // How long we stay in either phase of the movement pattern
    private bool goX = false;
    private bool reversed = false;  // Used to ping-pong the Lover off of the enemy bounds if they try to stray outside of them when not chasing

    private float currentXDir;  // Either 1 or -1
    private float currentZDir;

    private float currentSpeed;

    public bool spawnedToFight; // For disabling the attempts of the spawned enemies to move about
    Vector3 pursuitVector;

    bool battleMovementEnabled = false;

    private void Start()
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
            active = enemyFollow.isFollow();    // Are we following the player or not?

            if (!active && GameManager.Instance.enemyCanMove()) // If we are not chasing the player
            {

                if(moveRandomlyTimer <= 0) // If our timer has run out, reset it and swap our movement style
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

                if (!enemyFollow.inBounds() && !reversed)   // Keeps the Lover from wandering out of enemy bounds
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
            else if (active && GameManager.Instance.enemyCanMove()) // If chase has been enabled
            {
                Transform playerTransform = PlayerManager.Instance.PlayerTransform();
                pursuitVector = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);

                direction = (pursuitVector - this.transform.position).normalized;
                currentSpeed = chaseMovementSpeed;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enemyCanMove()) // If we are not chasing the player
        {
            rb.velocity = direction * currentSpeed * Time.fixedDeltaTime;

        }
    }

    // Public methods---------------------------------------------------------------
    public void MovementToggle(bool enabled)    // Toggles the overworld movement on and off
    {
        //movementEnabled = enabled;
        if (enabled)
        {
            if (active)
            {
                currentSpeed = chaseMovementSpeed;
            }
            else
            {
                currentSpeed = normalMovementSpeed;
            }
            
        }
        else
        {
            if (active)
            {
                currentSpeed = .4f * chaseMovementSpeed;
            }
            else
            {
                currentSpeed = .4f * normalMovementSpeed;
            }
        }
    }

    public void BattleMovementToggle(bool enabled)    // Toggles the battle movement on and off
    {
        battleMovementEnabled = enabled;
    }

    // Private methods---------------------------------------------------------------
    private bool generateAxis() // Generates which axis we'll be moving along
    {
        bool x = true;
        if(Random.Range(-1, 1) == 0)
        {
            x = false;
        }
        return x;
    }
    private int generateDir() // Generates a random dir for the Forgotten Lover to walk in
    {
        int num = Random.Range(-1, 1);
        if(num== 0)
        {
            num = 1;
        }
        return num;
    }
}
