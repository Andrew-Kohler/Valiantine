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

    private bool walkSound;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        skullmetMovement = GetComponentInParent<SkullmetMovement>();
        rb = GetComponentInParent<Rigidbody>();
        audioS = GetComponent<AudioSource>();
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

                if ((frame == 5) && !walkSound)
                {
                    walkSound = true;
                    playFootstep();
                }
                if (frame == 0 || frame == 5)
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
                walkSound = false;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
    }

    private void playFootstep()
    {
        int rand = Random.Range(0, 2);
        audioS.Stop();
        if(rand == 0)
            audioS.PlayOneShot(sounds[0], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        else
            audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
    }

    private void playHurt()
    {
        int rand = Random.Range(0, 2);
        audioS.Stop();
        if (rand == 0)
            audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        else
            audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
    }

    private void playBone()
    {
        int rand = Random.Range(0, 2);
        audioS.Stop();
        if (rand == 0)
            audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        else
            audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
        animationSpeed = _NormalAnimationSpeed * 1.3f;
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

        frame = 5;
        deltaT = 0;
        playFootstep();
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 5 + (int)(deltaT * (1.1f * animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // MOVE 1: Bite (Standard attack anim)
    protected override IEnumerator DoMove1Anim()
    {
        // Startup stuff
        activeCoroutine = true;
        deltaT = 0;
        bool fall = false;
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
        walkSound = false;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if ((frame == 5) && !walkSound)
            {
                walkSound = true;
                playFootstep();
            }
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
                walkSound = false;
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
            if(frame == 4 && !dealDamage)
            {
                dealDamage = true;
                playBone();
            }
            if(frame == 7 && !fall)
            {
                fall = true;
                playFootstep();
            }
            yield return null;
        }      

        // PART 3: Walk back to original position -------------------------------------
        animationIndex = _WalkRIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        skullmetMovement.MoveToPoint(skullmetMovement.GetBattleIdlePosition(), .1f);

        frame = 0;
        walkSound = false;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if ((frame == 5) && !walkSound)
            {
                walkSound = true;
                playFootstep();
            }
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
                walkSound = false;
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
        bool fall = false;
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
        walkSound = false; 
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if ((frame == 5) && !walkSound)
            {
                walkSound = true;
                playFootstep();
            }
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
                walkSound = false;
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
            if (frame == 4 && !dealDamage)
            {
                dealDamage = true;
                playBone();
            }
            if(frame == 6)
            {
                dealDamage = false;
            }
            if (frame == 7 && !fall)
            {
                fall = true;
                playFootstep();
            }
            yield return null;
        }

        fall = false;
        frame = 0;                 
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 4 && !dealDamage)
            {
                playBone();
                dealDamage = true;
            }
            if (frame == 7 && !fall)
            {
                fall = true;
                playFootstep();
            }
            yield return null;
        }

        // PART 3: Walk back to original position -------------------------------------
        animationIndex = _WalkRIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        skullmetMovement.MoveToPoint(skullmetMovement.GetBattleIdlePosition(), .1f);

        frame = 0;
        walkSound = false;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if ((frame == 5) && !walkSound)
            {
                walkSound = true;
                playFootstep();
            }
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
                walkSound = false;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            yield return null;
        }
        walkSound = false;
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
        walkSound = false;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if ((frame == 5) && !walkSound)
            {
                walkSound = true;
                playFootstep();
            }
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
                walkSound = false;
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
            if (frame == 5 && !dealDamage)
            {
                dealDamage = true;
                playFootstep();
            }
            if (frame == 11 && dealDamage)
            {
                dealDamage = false;
                playFootstep();
            }
            yield return null;
        }

        // PART 3: Walk back to original position -------------------------------------
        animationIndex = _WalkRIndex;
        animationSpeed = _SpedUpAnimationSpeed;
        frameLoop = 7;

        skullmetMovement.MoveToPoint(skullmetMovement.GetBattleIdlePosition(), .1f);

        frame = 0;
        walkSound = false;
        while (!skullmetMovement.arrived)                       // Wait until the skullmet gets where it is going
        {
            if ((frame == 5) && !walkSound)
            {
                walkSound = true;
                playFootstep();
            }
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
                walkSound = false;
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
            if (frame == 5)
            {
                playFootstep();
            }
            if (frame == 11)
            {
                playFootstep();
            }
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
        bool playHurt2 = false;
        bool playHurt3 = false;
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
        playHurt();
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 3 && !playHurt2)
            {
                playHurt();
                playHurt2 = true;
            }
            if (frame == 7 && !playHurt3)
            {
                playFootstep();
                playHurt3 = false;
            }
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
        playHurt();
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 3)
            {
                playFootstep();

            }
            if (frame == 7)
            {
                playBone();
                
            }
            if (frame == 9)
            {
                playBone();

            }
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
            if (frame == 3)
            {
                playBone();

            }
            if (frame == 7)
            {
                playBone();

            }
            yield return null;
        }
        playFootstep();

        frameLoop = 13;
        animationSpeed = _NormalAnimationSpeed;
        animationIndex = _IdleIndex;
        activeCoroutine = false;
        yield return null;
    }
}
