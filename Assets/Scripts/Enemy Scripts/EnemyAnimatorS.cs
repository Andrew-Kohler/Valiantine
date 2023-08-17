using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorS : MonoBehaviour
{
    public enum AnimationAxis { Rows, Columns } // Enum creation for whether we're animating along rows or down columns

    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected Rigidbody rb;

    [SerializeField] protected string rowProperty = "_CurrRow", colProperty = "_CurrCol"; // Properties of the material

    [SerializeField] protected AnimationAxis axis; // Whether we're animating along rows or down columns
    [SerializeField] protected float animationSpeed = 5.4f;
    protected float _NormalAnimationSpeed = 5.4f;      // Normal speed of animation (I think this constant roughly corresponds to 6 FPS)
    protected float _SpedUpAnimationSpeed = 11.88f;    // 2.2x faster than normal to correspond with chase velocity
    [SerializeField] protected int animationIndex = 0;        // The index of the animation that's currently playing

    protected int frameLoop = 0;  // A value to hold the number of the frame that the current animation loops on (e.g. after frame 13, loop it)
    protected int frameReset = 0; // A value to hold the number of the frame that the current animation loops back to (e.g. the loop starts on frame 0)
    protected float deltaT = 0;   // A cumulative delta time value
    public bool activeCoroutine = false;    // The classic boolean to use when Update() needs to be quiet during a coroutine
    public bool dealDamage = false;         // A boolean flipped on and off to indicate when in the animation damage should be dealt to the player to keep things lined up

    // Public methods ---------------------------------------------------------
    public void PlayBattleEntrance(int position)        // Position docs:   5 = Furthest back (3 enemy battle)
    {                                                                   //  4 = Less far back (2 enemy battle)
        StartCoroutine(DoBattleEnterAnim(position));                    //  3 = Straight (1, 3 enemy battle)
    }                                                                   //  2 = A bit forward (2 enemy battle)       
                                                                        //  1 = Very forward (3 enemy battle)   
    public void PlayMove1()
    {
        StartCoroutine(DoMove1Anim());
    }

    public void PlaySpellcast()
    {
        StartCoroutine(DoMove2Anim());
    }

    public void PlayMove3()
    {
        StartCoroutine(DoMove3Anim());
    }

    public void PlayMove4()
    {
        StartCoroutine(DoMove4Anim());
    }

    public void PlayHurt()
    {
        StartCoroutine(DoHurtAnim());
    }

    public void PlayDie()
    {
        StartCoroutine(DoDieAnim());
    }

    // Coroutines --------------------------------------------------------------
    // Entering the battle
    protected virtual IEnumerator DoBattleEnterAnim(int position)
    {
        yield return null;
    }

    // 1st combat move
    protected virtual IEnumerator DoMove1Anim()
    {
        yield return null;
    }

    // 2nd combat move
    protected virtual IEnumerator DoMove2Anim()
    {
        yield return null;
    }

    // 3rd combat move
    protected virtual IEnumerator DoMove3Anim()
    {
        yield return null;
    }

    // 4th combat move
    protected virtual IEnumerator DoMove4Anim()
    {
        yield return null;
    }

    // Getting hurt
    protected virtual IEnumerator DoHurtAnim()
    {
        yield return null;
    }
    protected virtual IEnumerator DoDieAnim()
    {
        yield return null;
    }
}
