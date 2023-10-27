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
                if(Mathf.Abs(rb.velocity.z) > Mathf.Abs(rb.velocity.x)) // Allows the higher velocity to always take priority
                {
                    if (rb.velocity.z < -.05) // Moving forwards
                    {
                        animationIndex = _WalkForwardsIndex;
                    }
                    else if (rb.velocity.z > 0.05) // Moving backwards
                    {
                        animationIndex = _WalkBackwardsIndex;
                    }
                    else // Moving left or right
                    {
                        if (rb.velocity.x < -0.05) // Moving left
                        {
                            animationIndex = _WalkLIndex;
                        }
                        else if (rb.velocity.x > 0.05) // Moving right
                        {
                            animationIndex = _WalkRIndex;
                        }
                    }
                }
                else
                {
                    if (rb.velocity.x < -0.05) // Moving left
                    {
                        animationIndex = _WalkLIndex;
                    }
                    else if (rb.velocity.x > 0.05) // Moving right
                    {
                        animationIndex = _WalkRIndex;
                    }
                    else 
                    {
                        if (rb.velocity.z < -.05) // Moving forwards
                        {
                            animationIndex = _WalkForwardsIndex;
                        }
                        else if (rb.velocity.z > 0.05) // Moving backwards
                        {
                            animationIndex = _WalkBackwardsIndex;
                        }
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
                else if(frame == 2)
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
    protected override IEnumerator DoBattleEnterAnim(int position, bool playerFromLeft)
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
        float yStop = gameObject.GetComponentInParent<Transform>().position.y;

        // Physical component (which angle the Skullmet jumps back at)
        if (playerFromLeft) // If the player came from the left
        {
            if (position == 1)
                rb.velocity = new Vector3(15f, 3f, -2f); // A little closer to cam (3 enemy fight)
            else if (position == 2)
                rb.velocity = new Vector3(15f, 3f, -3f); // A little closer to cam (2 enemy fight)
            else if (position == 3)
                rb.velocity = new Vector3(10f, 3f, 0f); // Directly parallel to player (1, 3 enemy fight) 
            else if (position == 4)
                rb.velocity = new Vector3(10f, 3f, 3f); // A little further from cam (2 enemy fight)
            else if (position == 5)
                rb.velocity = new Vector3(5f, 3f, 4f); // A little further from cam (3 enemy fight)
        }
        else // If the player came from the right
        {
            if (position == 1)
                rb.velocity = new Vector3(20f, 3f, -2f); // A little closer to cam (3 enemy fight)
            else if (position == 2)
                rb.velocity = new Vector3(20f, 3f, -3f); // A little closer to cam (2 enemy fight)
            else if (position == 3)
                rb.velocity = new Vector3(15f, 3f, 0f); // Directly parallel to player (1, 3 enemy fight) 
            else if (position == 4)
                rb.velocity = new Vector3(15f, 3f, 3f); // A little further from cam (2 enemy fight)
            else if (position == 5)
                rb.velocity = new Vector3(10f, 3f, 4f); // A little further from cam (3 enemy fight)
        }
        

        // Animated component (sprite-based motion corresponding to physical motion)
        int frame = 0;// (int)(deltaT * animationSpeed);
        yield return new WaitForSeconds(.1f);
        deltaT = 0;
        while (gameObject.GetComponentInParent<Transform>().position.y > yStop)
        {
            if (frame > 4)
            {
                deltaT = 0;
            }
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 1 + (int)(deltaT * (2f * animationSpeed));
            yield return null;  
        }
        //yield return new WaitForSeconds(.7f - deltaT);
        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        activeCoroutine = false;
        yield return null;
    }

    // MOVE 1: Bite (Standard attack anim)
    protected override IEnumerator DoMove1Anim()
    {
        // Startup stuff
        activeCoroutine = true;
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

        skullmetMovement.SetBattleIdlePosition();
        

        // PART 1: Walk to player position ----------------------------------------------------
        animationIndex = _WalkLIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        Vector3 playerPos = new Vector3(PlayerManager.Instance.PlayerTransform().position.x, GetComponentInParent<Transform>().position.y, PlayerManager.Instance.PlayerTransform().position.z);
        skullmetMovement.MoveToPoint(playerPos, 3.5f);

        skullmetMovement.enabled = true;

        int frame = 0;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if (frame == 0 || frame == 5)
            {
                skullmetMovement.BattleMovementToggle(false);
            }
            else if (frame == 2)
            {
                skullmetMovement.BattleMovementToggle(true);
            }

            deltaT += Time.deltaTime;
            frame = (int)(deltaT * animationSpeed);
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            
            yield return null;
        }

        skullmetMovement.BattleMovementToggle(false);                       // Once we arrive, stop forward motion
        skullmetMovement.arrived = false;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // PART 2: Play the attack animation ------------------------------------------
        animationIndex = _AttackIndex;
        animationSpeed = _NormalAnimationSpeed;
        frameLoop = 11;

        frame = 0;                  // Play the attack animation once
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 4)
            {
                dealDamage = true;
            }
            yield return null;
        }      

        // PART 3: Walk back to original position -------------------------------------
        animationIndex = _WalkRIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        skullmetMovement.MoveToPoint(skullmetMovement.GetBattleIdlePosition(), .1f);

        frame = 0;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if (frame == 0 || frame == 5)
            {
                skullmetMovement.BattleMovementToggle(false);
            }
            else if (frame == 2)
            {
                skullmetMovement.BattleMovementToggle(true);
            }

            deltaT += Time.deltaTime;
            frame = (int)(deltaT * animationSpeed);
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            yield return null;
        }

        skullmetMovement.BattleMovementToggle(false);                       // Once we arrive, stop forward motion
        skullmetMovement.arrived = false;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
        skullmetMovement.enabled = false;
        animationSpeed = _NormalAnimationSpeed;
        activeCoroutine = false;
        yield return null;
    }

    // MOVE 2: 
    protected override IEnumerator DoMove2Anim()
    {
        // Startup stuff
        activeCoroutine = true;
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

        skullmetMovement.SetBattleIdlePosition();


        // PART 1: Walk to player position ----------------------------------------------------
        animationIndex = _WalkLIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        Vector3 playerPos = new Vector3(PlayerManager.Instance.PlayerTransform().position.x, GetComponentInParent<Transform>().position.y, PlayerManager.Instance.PlayerTransform().position.z);
        skullmetMovement.MoveToPoint(playerPos, 3.5f);

        skullmetMovement.enabled = true;

        int frame = 0;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if (frame == 0 || frame == 5)
            {
                skullmetMovement.BattleMovementToggle(false);
            }
            else if (frame == 2)
            {
                skullmetMovement.BattleMovementToggle(true);
            }

            deltaT += Time.deltaTime;
            frame = (int)(deltaT * animationSpeed);
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);

            yield return null;
        }

        skullmetMovement.BattleMovementToggle(false);                       // Once we arrive, stop forward motion
        skullmetMovement.arrived = false;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // PART 2: Play the attack animation ------------------------------------------
        animationIndex = _AttackIndex;
        animationSpeed = _NormalAnimationSpeed;
        frameLoop = 11;

        frame = 0;                  // Play the attack animation twice
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 4)
            {
                dealDamage = true;
            }
            if(frame == 6)
            {
                dealDamage = false;
            }
            yield return null;
        }

        frame = 0;                 
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 4)
            {
                dealDamage = true;
            }
            yield return null;
        }

        // PART 3: Walk back to original position -------------------------------------
        animationIndex = _WalkRIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        skullmetMovement.MoveToPoint(skullmetMovement.GetBattleIdlePosition(), .1f);

        frame = 0;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if (frame == 0 || frame == 5)
            {
                skullmetMovement.BattleMovementToggle(false);
            }
            else if (frame == 2)
            {
                skullmetMovement.BattleMovementToggle(true);
            }

            deltaT += Time.deltaTime;
            frame = (int)(deltaT * animationSpeed);
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            yield return null;
        }

        skullmetMovement.BattleMovementToggle(false);                       // Once we arrive, stop forward motion
        skullmetMovement.arrived = false;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
        skullmetMovement.enabled = false;
        animationSpeed = _NormalAnimationSpeed;
        activeCoroutine = false;
        yield return null;
    }

    // MOVE 3: 
    protected override IEnumerator DoMove3Anim()
    {
        // Startup stuff
        activeCoroutine = true;
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

        skullmetMovement.SetBattleIdlePosition();


        // PART 1: Walk to player position ----------------------------------------------------
        animationIndex = _WalkLIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        Vector3 playerPos = new Vector3(PlayerManager.Instance.PlayerTransform().position.x, GetComponentInParent<Transform>().position.y, PlayerManager.Instance.PlayerTransform().position.z);
        skullmetMovement.MoveToPoint(playerPos, 3.5f);

        skullmetMovement.enabled = true;

        int frame = 0;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if (frame == 0 || frame == 5)
            {
                skullmetMovement.BattleMovementToggle(false);
            }
            else if (frame == 2)
            {
                skullmetMovement.BattleMovementToggle(true);
            }

            deltaT += Time.deltaTime;
            frame = (int)(deltaT * animationSpeed);
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);

            yield return null;
        }

        skullmetMovement.BattleMovementToggle(false);                       // Once we arrive, stop forward motion
        skullmetMovement.arrived = false;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // PART 2: Play the attack animation ------------------------------------------
        animationIndex = _SpellcastIndex;
        animationSpeed = _NormalAnimationSpeed;
        frameLoop = 13;

        frame = 0;                  // Play the attack animation once
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 5)
            {
                dealDamage = true;
            }
            yield return null;
        }

        // PART 3: Walk back to original position -------------------------------------
        animationIndex = _WalkRIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        skullmetMovement.MoveToPoint(skullmetMovement.GetBattleIdlePosition(), .1f);

        frame = 0;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if (frame == 0 || frame == 5)
            {
                skullmetMovement.BattleMovementToggle(false);
            }
            else if (frame == 2)
            {
                skullmetMovement.BattleMovementToggle(true);
            }

            deltaT += Time.deltaTime;
            frame = (int)(deltaT * animationSpeed);
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            yield return null;
        }

        skullmetMovement.BattleMovementToggle(false);                       // Once we arrive, stop forward motion
        skullmetMovement.arrived = false;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
        skullmetMovement.enabled = false;
        animationSpeed = _NormalAnimationSpeed;
        activeCoroutine = false;
        yield return null;
    }

    // MOVE 4: 
    protected override IEnumerator DoMove4Anim()
    {
        // Startup stuff
        activeCoroutine = true;
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

        animationIndex = _SpellcastIndex;
        animationSpeed = _NormalAnimationSpeed;
        frameLoop = 13;

        int frame = 0;                  // Play the attack animation once
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 12)
            {
                dealDamage = true;
            }
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // Getting hurt
    protected override IEnumerator DoHurtAnim()
    {
        // Startup stuff
        activeCoroutine = true;
        animationIndex = _HurtIndex; 
        frameLoop = 10;
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

        int frame = 0;                  
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // Dying
    protected override IEnumerator DoDieAnim()
    {
        // Startup stuff
        activeCoroutine = true;
        animationIndex = _DieIndex; // The main thing in all of this startup is changing the animation index and frame data
        frameLoop = 12;
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

        int frame = 0;
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            yield return null;
        }

        yield return null;
    }

    protected override IEnumerator DoDieReverseAnim()
    {
        // Startup stuff
        activeCoroutine = true;
        animationIndex = _DieIndex; // The main thing in all of this startup is changing the animation index and frame data
        frameLoop = 12;
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

        int frame = 11;
        deltaT = 0;
        while (frame >= 0)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 11 - (int)(deltaT * (animationSpeed));
            yield return null;
        }

        frameLoop = 13;
        animationSpeed = _NormalAnimationSpeed;
        animationIndex = _IdleIndex;
        activeCoroutine = false;
        yield return null;
    }
}
