using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullmetAnimatorS : EnemyAnimatorS
{
    private SkullmetMovement skullmetMovement;

    private int _SpellcastIndex = 9;    // The row indicies of all the Skullmet animations
    private int _DieIndex = 8;
    private int _HurtIndex = 7;
    private int _WalkBackwardsIndex = 6;
    private int _WalkForwardsIndex = 5;
    private int _WalkRIndex = 4;
    private int _WalkLIndex = 3;
    private int _AttackIndex = 2;
    private int _BattleEnterIndex = 1;
    private int _IdleIndex = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        skullmetMovement = GetComponentInParent<SkullmetMovement>();
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeCoroutine && !GameManager.Instance.isSettings())   // If one of our special animations isn't going on and we aren't paused
        {
            // Determining which animation should be playing ----------------------------------------------------------------
            if (GameManager.Instance.enemyCanMove())    // Non-battle animation
            {
                frameLoop = 7;
                if (rb.velocity.z < -.05) // Moving forwards
                {
                    animationIndex = _WalkForwardsIndex;
                }
                else if(rb.velocity.z > 0.05) // Moving backwards
                {
                    animationIndex = _WalkBackwardsIndex;
                }
                else // Moving left or right
                {
                    if(rb.velocity.x < -0.05) // Moving left
                    {
                        animationIndex = _WalkLIndex;
                    }
                    else if(rb.velocity.x > 0.05) // Moving right
                    {
                        animationIndex = _WalkRIndex;
                    }
                }
            }

            else if (GameManager.Instance.isBattle())
            {
                //  Battle idle
                frameLoop = 13;
                animationSpeed = _NormalAnimationSpeed;
                animationIndex = _IdleIndex;
            }

            // Actually playing the chosen animation ------------------------------------------------------------------
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
            if (GameManager.Instance.enemyCanMove())
            {
                if (skullmetMovement.ChasingPlayer)
                {
                    animationSpeed = _SpedUpAnimationSpeed;
                }
                else
                {
                    animationSpeed = _NormalAnimationSpeed;
                }


                if(frame == 0 || frame == 5)
                {
                    skullmetMovement.MovementToggle(false);
                }
                else if(frame == 1)
                {
                    skullmetMovement.MovementToggle(true);
                }
            }

            deltaT += Time.deltaTime;
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
    }

    // Coroutines -------------------------------------------------------

    // Entering the battle
    protected override IEnumerator DoBattleEnterAnim(int position)
    {
        // Startup stuff
        activeCoroutine = true;
        animationIndex = _BattleEnterIndex; // The main thing in all of this startup is changing the animation index and frame data
        frameLoop = 7;
        deltaT = 0;
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

        // Physical component (which angle the Skullmet jumps back at)
        if (position == 1)
            rb.velocity = new Vector3(15f, 3f, -2f); // A little closer to cam (3 enemy fight)
        else if (position == 2)
            rb.velocity = new Vector3(15f, 3f, -3f); // A little closer to cam (2 enemy fight)
        else if (position == 3)
            rb.velocity = new Vector3(10f, 3f, 0f); // Directly parallel to player (1, 3 enemy fight) 
        else if(position == 4)
            rb.velocity = new Vector3(10f, 3f, 3f); // A little further from cam (2 enemy fight)
        else if(position == 5)
            rb.velocity = new Vector3(5f, 3f, 4f); // A little further from cam (3 enemy fight)

        // Animated component (sprite-based motion corresponding to physical motion)
        int frame = 0;// (int)(deltaT * animationSpeed);
        while(frame < frameLoop)
        {
            
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (2f * animationSpeed));
            yield return null;  
        }
        yield return new WaitForSeconds(.7f - deltaT);
        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        activeCoroutine = false;
        yield return null;
    }

    // Attacking
    IEnumerator DoAttackAnim()
    {
        yield return null;
    }

    // Casting a spell
    IEnumerator DoSpellcastAnim()
    {
        yield return null;
    }

    // Getting hurt
    IEnumerator DoHurtAnim()
    {
        yield return null;
    }

    // Dying
    IEnumerator DoDieAnim()
    {
        yield return null;
    }
}
