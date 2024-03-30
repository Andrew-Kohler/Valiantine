using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimatorS : EnemyAnimatorS
{
    private GolemMovement golemMovement;
    private GolemMoves golemMoves;

    private int _AttackIndex = 15;

    private int _IdleBackwardsIndex = 14;
    private int _IdleForwardsIndex = 13;
    private int _IdleLIndex = 12;
    private int _IdleRIndex = 11;

    private int _SitIndex = 10;
    private int _BattleEnterIndex = 9;
    private int _DieIndex = 8;
    private int _HurtIndex = 7;

    private int _SpellcastIndex = 6;
    private int _WakeUpIndex = 5;
    private int _SleepIndex = 4;

    private int _WalkBackwardsIndex = 3;
    private int _WalkForwardsIndex = 2;
    private int _WalkLIndex = 1;
    private int _WalkRIndex = 0;
    private bool step1;
    private bool step2;

    private int frame;

    private float footFrontYOffset = .2f; // Positions for whether a foot is in front of or behind a golem
    //private float footNeutralYOffset = 0f;
    private float footBehindYOffset = -.2f;

    private Vector3 frontFootPos = new Vector3(0, .2f, 0);
    private Vector3 midFootPos;
    private Vector3 backFootPos = new Vector3(0, -.2f, 0);

    [SerializeField] private Transform frontFoot;
    [SerializeField] private Transform backFoot;
    [SerializeField] private GameObject throwableFoot;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        golemMovement = GetComponentInParent<GolemMovement>();
        golemMoves = GetComponentInParent<GolemMoves>();
        rb = GetComponentInParent<Rigidbody>();
        audioS = GetComponent<AudioSource>();  

        frontFootPos = new Vector3(0, footFrontYOffset, 0);
        midFootPos = Vector3.zero;
        backFootPos = new Vector3(0, footBehindYOffset, 0);
        if (golemMovement.sleeping) // If the golem is sleeping
        {
            animationIndex = _SleepIndex;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeCoroutine && !GameManager.Instance.isSettings())   // If one of our special animations isn't going on and we aren't paused
        {
            // Determining which animation should be playing ----------------------------------------------------------------
            if (GameManager.Instance.enemyCanMove())    // Non-battle animation
            {
                frameLoop = 8;
                if (golemMovement.sleeping) // If the golem is sleeping
                {
                    frameLoop = 13;
                    animationIndex = _SleepIndex;
                }

                else // If we are moving, we do the moving animations
                {
                    if (!golemMovement.LurchingAround) // If we are at the stationary part
                    {
                        if (animationIndex == _WalkForwardsIndex)
                        {
                            animationIndex = _IdleForwardsIndex;
                        }
                        if (animationIndex == _WalkBackwardsIndex)
                        {
                            animationIndex = _IdleBackwardsIndex;
                        }
                        if (animationIndex == _WalkLIndex)
                        {
                            animationIndex = _IdleLIndex;
                        }
                        if (animationIndex == _WalkRIndex)
                        {
                            animationIndex = _IdleRIndex;
                        }
                    }
                    else // If we are at the walking around part
                    {
                        if (Mathf.Abs(rb.velocity.z) >= Mathf.Abs(rb.velocity.x)) // Allows the higher velocity to always take priority
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
                        else if (Mathf.Abs(rb.velocity.z) < Mathf.Abs(rb.velocity.x))
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

                    
                }
            }

            else if (GameManager.Instance.isBattle())
            {
                //  Battle idle
                
                animationSpeed = _NormalAnimationSpeed;
                if (golemMoves.asleep)
                {
                    frameLoop = 14;
                    animationIndex = _SleepIndex;
                }
                    
                else
                {
                    frameLoop = 7;
                    animationIndex = _IdleLIndex;
                }
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
            frame = (int)(deltaT * animationSpeed);

            if (GameManager.Instance.enemyCanMove()) // Correctly positions feet based on animation
            {
                // If we're doing an L/R animation
                if (animationIndex == _WalkLIndex || animationIndex == _WalkRIndex || animationIndex == _IdleLIndex || animationIndex == _IdleRIndex || animationIndex == _SleepIndex)
                {
                    frontFoot.localPosition = frontFootPos;
                    backFoot.localPosition = backFootPos;
                }
                // If we're in a forward or backwards idle
                else if (animationIndex == _IdleForwardsIndex || animationIndex == _IdleBackwardsIndex)
                {
                    frontFoot.localPosition = midFootPos;
                    backFoot.localPosition = midFootPos;
                }
                // If we're doing forward or backwards walk (the intensive ones)
                else if (animationIndex == _WalkForwardsIndex)
                {
                    if (frame < 2)
                    {
                        frontFoot.localPosition = backFootPos;
                        backFoot.localPosition = frontFootPos;
                    }
                    else if ((frame >= 2 && frame < 4) || (frame >= 6 && frame < 8))
                    {
                        frontFoot.localPosition = midFootPos;
                        backFoot.localPosition = midFootPos;
                    }
                    else
                    {
                        frontFoot.localPosition = frontFootPos;
                        backFoot.localPosition = backFootPos;
                    }
                }
                else if (animationIndex == _WalkBackwardsIndex)
                {
                    if (frame < 2)
                    {
                        frontFoot.localPosition = frontFootPos;
                        backFoot.localPosition = backFootPos;
                    }
                    else if ((frame >= 2 && frame < 4) || (frame >= 6 && frame < 8))
                    {
                        frontFoot.localPosition = midFootPos;
                        backFoot.localPosition = midFootPos;
                    }
                    else
                    {
                        frontFoot.localPosition = backFootPos;
                        backFoot.localPosition = frontFootPos;
                    }
                }
            }

            deltaT += Time.deltaTime;
            if(!GameManager.Instance.isBattle() && golemMovement.LurchingAround)
            {
                if (frame == 1 && !step1)
                {
                    step1 = true;
                    audioS.PlayOneShot(sounds[0], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                if (frame == 5 && !step2)
                {
                    step2 = true;
                    audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
            }
            
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
                step1 = false;
                step2 = false;

            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        }
    } // End of Update

    // Public methods---------------------------------------------------------------------------
    public int getGolemFrame()
    {
        return frame;
    }

    public int getGolemAnimationIndex()
    {
        return animationIndex;
    }

    // Coroutines ---------------------------------------------------------------------------
    protected override IEnumerator DoBattleEnterAnim(int position, bool playerFromLeft)
    {
        // Startup stuff
        activeCoroutine = true;
        animationIndex = _BattleEnterIndex; // The main thing in all of this startup is changing the animation index and frame data
        frameLoop = 13;
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
        frame = 0;// (int)(deltaT * animationSpeed);
        frontFoot.localPosition = frontFootPos;
        backFoot.localPosition = backFootPos;
        // Play the startup animation
        while (frame < 3)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (2f * animationSpeed));
            yield return null;
        }

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

        // Play the in-air animation
        yield return new WaitForSeconds(.1f);
        deltaT = 0;
        while (gameObject.GetComponentInParent<Transform>().position.y > yStop)
        {
            if(frame > 5)
            {
                deltaT = 0;
            }
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 3 + (int)(deltaT * (animationSpeed));
            yield return null;
        }

        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Ok, so there's one problem left, and that's that the feet sometimes desync on the back third of the animation?
        // How is that even possible

        // Play the landing animation
        audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 6 + (int)(deltaT * (1.4f *animationSpeed));
            yield return null;
        }


        activeCoroutine = false;
        yield return null;
    }

    // 1st combat move
    protected override IEnumerator DoMove1Anim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool animBool = false;
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

        animationSpeed = 5.4f;
        animationIndex = _AttackIndex;
        // Content ----------------------------------------------
        // Play the animation
        deltaT = 0;
        frame = 0;
        frameLoop = 9;
        GameObject rock;
        ThrowableFoot damager = null;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 4 && animBool == false)
            {
                animBool = true;
                audioS.PlayOneShot(sounds[7], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        audioS.PlayOneShot(sounds[7], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        meshRenderer.material.SetFloat(frameKey, 10);
        rock = Instantiate(throwableFoot, GetComponentInParent<Transform>(), true);
            damager = rock.GetComponent<ThrowableFoot>();
            damager.SetValues(PlayerManager.Instance.PlayerTransform(), GetComponentInParent<Transform>());

        yield return new WaitUntil(() => damager.damage);
        dealDamage = true;

        yield return new WaitUntil(() => damager.end);

        // After we play it to frame 10, we spawn a prefab at a given offset
        // This prefab goes from that offset, to an offset from the player, back to that offset
        // Once that prefab has accomplished its goal, we play the rest of the animation
        deltaT = 0;
        frame = 10;
        frameLoop = 17;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 10 + (int)(deltaT * (animationSpeed));
            if (frame == 10 && animBool == true)
            {
                animBool = false;
                audioS.PlayOneShot(sounds[0], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 12 && !animBool)
            {
                animBool = true;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }
        audioS.PlayOneShot(sounds[0], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);

        activeCoroutine = false;
        yield return null;
    }

    // 2nd combat move
    protected override IEnumerator DoMove2Anim()
    {
        // Setup ----------------------------------------------
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
        
        animationSpeed = 5.4f;
        


        if (golemMoves.asleep) // If we're asleep, that means we should be waking up
        {
            golemMoves.asleep = false;
            animationIndex = _WakeUpIndex;
            frameLoop = 24;
            deltaT = 0;
            frame = 0;
            bool wake = false;
            while (frame < frameLoop)
            {
                deltaT += Time.deltaTime;
                meshRenderer.material.SetFloat(clipKey, animationIndex);
                meshRenderer.material.SetFloat(frameKey, frame);
                frame = (int)(deltaT * (animationSpeed));
                if (!wake && frame == 7)
                {
                    wake = true;
                    audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                if (wake && frame == 9)
                {
                    wake = false;
                    audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                if (!wake && frame == 15)
                {
                    wake = true;
                    audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                if (wake && frame == 20)
                {
                    wake = false;
                    audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                yield return null;
            }
        }
        else // Otherwise, zzz
        {
            golemMoves.asleep = true;
            animationIndex = _SitIndex;
            frameLoop = 9;
            bool sit = false;

            deltaT = 0;
            frame = 0;
            while (frame < frameLoop)
            {
                deltaT += Time.deltaTime;
                meshRenderer.material.SetFloat(clipKey, animationIndex);
                meshRenderer.material.SetFloat(frameKey, frame);
                frame = (int)(deltaT * (animationSpeed));
                if (!sit && frame == 2)
                {
                    sit = true;
                    audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                if (sit && frame == 6)
                {
                    sit = false;
                    audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                }
                yield return null;
            }
        }
        activeCoroutine = false;
        yield return null;
    }

    // 3rd combat move
    protected override IEnumerator DoMove3Anim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        deltaT = 0;
        bool sit = false;
        bool wake = false;
        bool up = false;
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
        animationIndex = _SitIndex;
        animationSpeed = 5.4f;
        frameLoop = 9;

        // Content ----------------------------------------------
        // Play the animation
        // Sit down
        frame = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(!sit && frame == 2)
            {
                sit = true;
                audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (sit && frame == 6)
            {
                sit = false;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        // Do a sleep cycle
        animationIndex = _SleepIndex;
        frameLoop = 14;
        deltaT = 0;
        frame = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            
            yield return null;
        }

        // Wake up
        animationIndex = _WakeUpIndex;
        frameLoop = 24;
        deltaT = 0;
        frame = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (!wake && frame == 7)
            {
                wake = true;
                audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (wake && frame == 9)
            {
                wake = false;
                audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (!wake && frame == 15)
            {
                wake = true;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (!up && frame == 20)
            {
                up = true;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }
        activeCoroutine = false;
        yield return null;
    }

    // 4th combat move
    protected override IEnumerator DoMove4Anim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool hop = false;
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
        animationIndex = _SpellcastIndex;
        animationSpeed = 5.4f;
        frameLoop = 8;

        // Content ----------------------------------------------
        // Play the animation
        frame = 0;
        while (frame < frameLoop - 1)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 4 && !hop)
            {
                hop = true;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        frame = 0;
        hop = false;
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 4 && !hop)
            {
                hop = true;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // Getting hurt
    protected override IEnumerator DoHurtAnim()
    {
        // Setup ----------------------------------------------
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
        animationIndex = _HurtIndex;
        animationSpeed = 5.4f;
        frameLoop = 4;

        // Content ----------------------------------------------
        // Play the animation
        frame = 0;
        audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        golemMoves.asleep = false; // If you hurt the golem, you wake it up
        yield return null;
    }
    //Dying
    protected override IEnumerator DoDieAnim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        deltaT = 0;
        string clipKey, frameKey;
        bool fall = false;
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
        animationIndex = _DieIndex;
        animationSpeed = 5.4f;
        frameLoop = 24;

        // Content ----------------------------------------------
        // Play the animation
        frame = 0;
        audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 7 &!fall)
            {
                fall = true;
                audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 10 & fall)
            {
                fall = false;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
        frameLoop = 24;
        deltaT = 0;
        bool heal = false;
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

        int frame = 23;
        deltaT = 0;
        animationSpeed = _NormalAnimationSpeed * 2;
        while (frame >= 0)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 23 - (int)(deltaT * (animationSpeed));
            if (frame == 6 && !heal)
            {
                heal = true;
                audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }


        frameLoop = 8;
        animationSpeed = _NormalAnimationSpeed;
        animationIndex = _IdleRIndex;
        activeCoroutine = false;
        yield return null;
    }
}
