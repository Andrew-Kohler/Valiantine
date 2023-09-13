using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoverAnimatorS : EnemyAnimatorS
{
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

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        loverMovement = GetComponentInParent<LoverMovement>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        if (!activeCoroutine && !GameManager.Instance.isSettings())   // If one of our special animations isn't going on and we aren't paused
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
