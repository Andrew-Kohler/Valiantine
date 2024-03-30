using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoverAnimatorS : EnemyAnimatorS
{
    [SerializeField] private GameObject vinePrefab;
    private LoverMovement loverMovement;

    private int _AttackIndex = 12;
    private int _BattleEnterIndex = 11;
    private int _DieIndex = 10;
    private int _HurtIndex = 9;

    private int _IdleBackwardsIndex = 8;
    private int _IdleForwardsIndex = 7;
    private int _IdleLIndex = 6;
    private int _IdleRIndex = 5;

    private int _SpellcastIndex = 4;

    private int _WalkBackwardsIndex = 3;
    private int _WalkForwardsIndex = 2;
    private int _WalkLIndex = 1;
    private int _WalkRIndex = 0;

    private bool stepNoise = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        loverMovement = GetComponentInParent<LoverMovement>();
        rb = GetComponentInParent<Rigidbody>();
        audioS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!activeCoroutine && !GameManager.Instance.isSettings())   // If one of our special animations isn't going on and we aren't paused
        {
            if (!GameManager.Instance.isBattle())
            {
                // Determining which animation should be playing ----------------------------------------------------------------
                if (GameManager.Instance.enemyCanMove())    // Non-battle animation
                {
                    frameLoop = 7;
                    if (!loverMovement.LurchingAround) // If we are stationary
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

                    else // If we are moving, we do the moving animations
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
                frameLoop = 7;
                animationSpeed = _NormalAnimationSpeed;
                animationIndex = _IdleLIndex;
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
                if (loverMovement.ChasingPlayer)
                {
                    animationSpeed = _SpedUpAnimationSpeed;
                }
                else
                {
                    animationSpeed = _NormalAnimationSpeed;
                }

                if (loverMovement.LurchingAround) // If we are moving, we lurch
                {
                    if (frame < 2)
                    {
                        loverMovement.MovementToggle(true);
                    }
                    else if (frame >= 2)
                    {
                        loverMovement.MovementToggle(false);
                    }
                    if(frame == 2 && !stepNoise)
                    {
                        stepNoise = true;
                        audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                    }

                    if (frame == 4 && stepNoise)
                    {
                        stepNoise = false;
                        audioS.PlayOneShot(sounds[0], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
                    }
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

    protected override IEnumerator DoBattleEnterAnim(int position, bool playerFromLeft)
    {
        // Startup stuff
        activeCoroutine = true;
        animationIndex = _BattleEnterIndex; // The main thing in all of this startup is changing the animation index and frame data
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

        float yStop = gameObject.GetComponentInParent<Transform>().position.y;
        int frame = 0;// (int)(deltaT * animationSpeed);

       

        // Play the startup animation
        while (frame < 4)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (4f * animationSpeed));
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
            if (frame > 5)
            {
                deltaT = 0;
            }
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 4 + (int)(deltaT * (animationSpeed));
            yield return null;
        }

        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Play the landing animation
        audioS.PlayOneShot(sounds[6], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 6 + (int)(deltaT * (1.4f * animationSpeed));
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
        bool animB = false;
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
        animationIndex = _AttackIndex;
        animationSpeed = 5.4f;
        frameLoop = 18;

        // Content ----------------------------------------------
        // Play the animation
        audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        int frame = 0;
        while (frame < 10)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 4 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[6], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            
            yield return null;
        }

        // Spawn the vine for a cool effect
        GameObject vine = Instantiate(vinePrefab,
            new Vector3(PlayerManager.Instance.PlayerTransform().position.x + .96f,
            PlayerManager.Instance.PlayerTransform().position.y + 2.66f,
            PlayerManager.Instance.PlayerTransform().position.z - .4f), vinePrefab.transform.rotation);
        dealDamage = true;
        yield return new WaitForSeconds(.3f);

        deltaT = 0;
        audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 10 + (int)(deltaT * (animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // 2nd combat move
    protected override IEnumerator DoMove2Anim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool animB = false;
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
        frameLoop = 23;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        while (frame < 17)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 3 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 5 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[8], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 9 && !animB)
            {
                animB = true;
                audioS.Stop();
                audioS.PlayOneShot(sounds[9], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 12 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[7], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        //dealDamage = true;
        frame = 0;
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 13 + (int)(deltaT * (animationSpeed));
            if (frame == 17 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 20 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // 3rd combat move
    protected override IEnumerator DoMove3Anim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool animB = false;
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
        animationIndex = _AttackIndex;
        animationSpeed = 5.4f;
        frameLoop = 18;

        // Content ----------------------------------------------
        // Play the animation
        audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        int frame = 0;
        while (frame < 10)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 4 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[6], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }

            yield return null;
        }

        // Spawn the vine for a cool effect
        GameObject vine = Instantiate(vinePrefab,
            new Vector3(PlayerManager.Instance.PlayerTransform().position.x + .96f,
            PlayerManager.Instance.PlayerTransform().position.y + 2.66f,
            PlayerManager.Instance.PlayerTransform().position.z - .4f), vinePrefab.transform.rotation);
        dealDamage = true;
        yield return new WaitForSeconds(.3f);

        deltaT = 0;
        audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 10 + (int)(deltaT * (animationSpeed));
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
        bool animB = false;
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
        frameLoop = 23;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        while (frame < 17)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 3 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 5 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[8], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 9 && !animB)
            {
                animB = true;
                audioS.Stop();
                audioS.PlayOneShot(sounds[9], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 12 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[7], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        dealDamage = true;
        frame = 0;
        deltaT = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 13 + (int)(deltaT * (animationSpeed));
            if (frame == 17 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 20 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
        bool anim = false;
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
        frameLoop = 19;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 3 && !anim)
            {
                anim = true;
                audioS.PlayOneShot(sounds[3], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if(frame == 7 && anim)
            {
                anim = false;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if(frame == 13 && !anim)
            {
                anim = true;
                audioS.PlayOneShot(sounds[5], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }
    //Dying
    protected override IEnumerator DoDieAnim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        deltaT = 0;
        bool animB = false;
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
        animationIndex = _DieIndex;
        animationSpeed = 5.4f;
        frameLoop = 26;

        // Content ----------------------------------------------
        // Play the animation
        audioS.PlayOneShot(sounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        int frame = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 2 && !animB)
            {
                animB = true;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 6 && animB)
            {
                animB = false;
                audioS.PlayOneShot(sounds[10], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 16 && !animB) // Body drop 1
            {
                animB = true;
                audioS.PlayOneShot(sounds[12], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if (frame == 23 && animB) // Body drop 2
            {
                animB = false;
                audioS.PlayOneShot(sounds[11], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
        frameLoop = 26;
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

        int frame = 25;
        deltaT = 0;
        animationSpeed = _NormalAnimationSpeed * 1.2f;
        while (frame >= 0)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 25 - (int)(deltaT * (animationSpeed));
            if (frame == 10 && !heal)
            {
                heal = true;
                audioS.PlayOneShot(sounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            if(frame == 1 && heal)
            {
                heal = false;
                audioS.PlayOneShot(sounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
