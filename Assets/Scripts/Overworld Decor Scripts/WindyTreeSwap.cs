using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindyTreeSwap : MonoBehaviour
{
    [SerializeField] ParticleSystem petals;
    [SerializeField] ParticleSystem modifiedPetals;
    // These are the constants for when the particle system is behaving normally
    private int normalEmissionRate = 1;
    private bool normalVOverLifetime = true;
    private int normalXLimit = 2;
    private int normalZLimit = 2;
    private bool normalExternalForces = false;

    // These are the constants for when it is windy
    private int modifiedEmissionRate = 2;
    private bool modifiedVOverLifetime = false;
    private int modifiedXLimit = 10;
    private int modifiedZLimit = 10;
    private bool modifiedExternalForces = true;

    bool blowing = false;

    private void OnEnable()
    {
        GameManager.onWindStateChange += SwapConstants;
    }

    private void OnDisable()
    {
        GameManager.onWindStateChange -= SwapConstants;
    }

    private void Start()
    {
    }

    private void SwapConstants()
    {
        if (!blowing)
        {
            StopAllCoroutines();
            StartCoroutine(DoWindStart());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(DoWindStop());
            blowing = false;
        }
    }

    IEnumerator DoWindStart()
    {
        // Get everything we'll be modifying
        var emissionRate = petals.emission;
        var velocityOverLifetime = petals.velocityOverLifetime;
        var externalForces = petals.externalForces;
        var vLimits = petals.limitVelocityOverLifetime;

        // Modify it
        emissionRate.rateOverTime = modifiedEmissionRate;
        velocityOverLifetime.enabled = modifiedVOverLifetime;
        externalForces.enabled = modifiedExternalForces;
        StartCoroutine(DoLimitsGradual(true));

        yield return new WaitForSeconds(4f);

        // Stop 1, Start 2
        petals.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        modifiedPetals.Play();

        yield return new WaitForSeconds(4f);

        blowing = true;

        // Reset 1 to its normal state
        emissionRate.rateOverTime = normalEmissionRate;
        velocityOverLifetime.enabled = normalVOverLifetime;
        externalForces.enabled = normalExternalForces;
        vLimits.limitX = normalXLimit;
        vLimits.limitZ = normalZLimit;
    }

    IEnumerator DoWindStop()
    {
        petals.Clear();
        modifiedPetals.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(3f);
        petals.Play();

    }

    IEnumerator DoLimitsGradual(bool increase)
    {
        var vLimits = petals.limitVelocityOverLifetime;
        float currentXLimit;
        float currentZLimit;

        if (increase)
        {
            currentXLimit = normalXLimit;
            currentZLimit = normalZLimit;
            while (vLimits.limitX.constant < modifiedXLimit)
            {
                currentXLimit += 5 * Time.deltaTime;
                currentZLimit += 5 * Time.deltaTime;
                vLimits.limitX = currentXLimit;
                vLimits.limitZ = currentZLimit;
                yield return null;
            }
            vLimits.limitX = modifiedXLimit;
            vLimits.limitZ = modifiedZLimit;
        }
        else
        {
            currentXLimit = modifiedXLimit;
            currentZLimit = modifiedZLimit;
            while (vLimits.limitX.constant > normalXLimit)
            {
                currentXLimit -= .1f * Time.deltaTime;
                currentZLimit -= .1f * Time.deltaTime;
                vLimits.limitX = currentXLimit;
                vLimits.limitZ = currentZLimit;
                yield return null;
            }
            vLimits.limitX = normalXLimit;
            vLimits.limitZ = normalZLimit;
        }

    }
}
