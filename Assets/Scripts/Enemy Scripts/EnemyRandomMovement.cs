/*
Enemy Random Movement
Used on:    Enemies
For:    Behavior of walking around set enemy boundaries randomly
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomMovement : MonoBehaviour
{
    EnemyFollow followCheck;
    EnemyChaseMovement chaseMovement;
    Rigidbody rb;
    Transform center;

    bool coroutineRunning;
    bool goX;
    int turn;
    int xDir;
    int zDir;
    int count;
    float movementSpeed = 6f;

    void Start()
    {
        followCheck = GetComponent<EnemyFollow>(); 
        chaseMovement = GetComponent<EnemyChaseMovement>();
        rb = GetComponent<Rigidbody>();
        center = GameObject.Find("Center E").transform;

        coroutineRunning = false;
        turn = 1;
    }

    void Update()
    {
        if (!followCheck.isFollow() && !coroutineRunning && GameManager.Instance.canMove())  // If we aren't chasing the player and the corutine isn't running, start it again
        {   
          StartCoroutine(behaviorLoop()); 
        }
        else if (followCheck.isFollow()) // If we are chasing the player
        {
            StopCoroutine(behaviorLoop());
            coroutineRunning = false;
            count = 0;
            rb.velocity = new Vector3(0f, 0f, 0f);
            chaseMovement.enabled = true;
            this.enabled = false;
        }
        else if(!followCheck.isFollow() && !GameManager.Instance.canMove())// If we aren't chasing the player but the GameManager says we can't move (battle time)
        {
            StopCoroutine(behaviorLoop());
            coroutineRunning = false;
            count = 0;
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
        else if (!GameManager.Instance.canMove())
        {
            StopCoroutine(behaviorLoop());
            coroutineRunning = false;
            count = 0;
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
        
    }
    IEnumerator behaviorLoop()
    {
        
        coroutineRunning = true;
        if (!followCheck.inBounds())  // If we somehow start the co-routine OOB
        {
            while (!followCheck.inBounds())   // While we aren't in-bounds, move back towards being in-bounds
            {
                transform.position = Vector3.MoveTowards(transform.position, center.position, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else                    // If we start the coroutine in-bounds
        {
            if (Random.Range(1, 5) > 2)  // Logic that randomly picks a direction value to change
            {
                goX = true;
                if (Random.Range(0, 2) == 0)
                {
                    xDir = 1;
                }
                else
                {
                    xDir = -1;
                }
            }
            else
            {
                goX = false;
                if (Random.Range(0, 2) == 0)
                {
                    zDir = 1;
                }
                else
                {
                    zDir = -1;
                }
            }

            while(count <= 60)  // Walk in our chosen direction for 1 second
            {
                if (!GameManager.Instance.canMove())
                {
                    count = 0;
                    rb.velocity = new Vector3(0f, 0f, 0f);
                    yield break;
                }
                if (goX)
                {
                    if (!followCheck.inBounds())
                    {
                        turn = turn * -1; // This is the problem: if we are on the bounds line, we jitter
                    }
                    rb.velocity = new Vector3(turn * xDir * movementSpeed, 0, 0);
                }
                else
                {
                    if (!followCheck.inBounds())
                    {
                        turn = turn * -1;
                    }
                    rb.velocity = new Vector3(0, 0, turn * zDir * movementSpeed);


                }
                count++;
                yield return new WaitForSeconds(.017f);
            }


        }

        rb.velocity = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(2f);    // Wait for two seconds
        count = 0;
        coroutineRunning = false;
    }

    private void OnCollisionEnter(Collision collision)  // This is where we check for if we touch another enemy and need to turn around
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            turn = turn * -1;
        }
    }
}

// Ah, I see. No return mechanism like the path follower: it tries to continue its routine wherever
// it gets left. Let's just make a center!
// Also we're getting stuck inside the boundary b/c of how changing dir currently works, maybe throw a <= and >= in

// Looks like the important clause isn't kicking in every time
