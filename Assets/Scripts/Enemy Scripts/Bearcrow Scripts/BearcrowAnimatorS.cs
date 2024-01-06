using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearcrowAnimatorS : EnemyAnimatorS
{
    private BearcrowMovement skullmetMovement;

    private int _SingleIndex = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        skullmetMovement = GetComponentInParent<BearcrowMovement>();
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
                frameLoop = 1;
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
        frameLoop = 1;
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
/*            if (frame > 1)
            {
                deltaT = 0;
            }
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (2f * animationSpeed));*/
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

        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
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

        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
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

        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
        animationSpeed = _NormalAnimationSpeed;
        activeCoroutine = false;
        yield return null;
    }

    // MOVE 4: 
    protected override IEnumerator DoMove4Anim()
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

        rb.velocity = new Vector3(0f, 0f, 0f);

        // Resume idle
        animationSpeed = _NormalAnimationSpeed;
        activeCoroutine = false;
        yield return null;
    }

    // Getting hurt
    protected override IEnumerator DoHurtAnim()
    {
        // Startup stuff
        activeCoroutine = true;
        frameLoop = 4;
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
            frame = 4 + (int)(deltaT * (animationSpeed));
            yield return null;
        }

        yield return null;
    }

    protected override IEnumerator DoDieReverseAnim()
    {
        // Startup stuff
        activeCoroutine = true;
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
        while (frame >= 4)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 9 - (int)(deltaT * (animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }
}

