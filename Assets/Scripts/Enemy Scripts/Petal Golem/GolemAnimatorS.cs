using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimatorS : EnemyAnimatorS
{
    private GolemMovement golemMovement;

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

    private int frame;

    private float footFrontYOffset = .2f; // Positions for whether a foot is in front of or behind a golem
    private float footNeutralYOffset = 0f;
    private float footBehindYOffset = -.2f;

    private Vector3 frontFootPos;
    private Vector3 midFootPos;
    private Vector3 backFootPos;

    [SerializeField] private Transform frontFoot;
    [SerializeField] private Transform backFoot;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        golemMovement = GetComponentInParent<GolemMovement>();
        rb = GetComponentInParent<Rigidbody>();

        frontFootPos = new Vector3(0, footFrontYOffset, 0);
        midFootPos = Vector3.zero;
        backFootPos = new Vector3(0, footBehindYOffset, 0);
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
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
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
    protected override IEnumerator DoBattleEnterAnim(int position)
    {
        activeCoroutine = true;


        activeCoroutine = false;
        yield return null;
    }

    // 1st combat move
    protected override IEnumerator DoMove1Anim()
    {
        activeCoroutine = true;


        activeCoroutine = false;
        yield return null;
    }

    // 2nd combat move
    protected override IEnumerator DoMove2Anim()
    {
        yield return null;
    }

    // 3rd combat move
    protected override IEnumerator DoMove3Anim()
    {
        yield return null;
    }

    // 4th combat move
    protected override IEnumerator DoMove4Anim()
    {
        yield return null;
    }

    // Getting hurt
    protected override IEnumerator DoHurtAnim()
    {
        yield return null;
    }
    //Dying
    protected override IEnumerator DoDieAnim()
    {
        yield return null;
    }
}
