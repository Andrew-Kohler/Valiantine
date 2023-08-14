using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindyGroundSwap : MonoBehaviour
{
    [SerializeField] ParticleSystem petals;
    bool blowing = false;

    private void OnEnable()
    {
        GameManager.onWindStateChange += SwapConstants;
    }

    private void OnDisable()
    {
        GameManager.onWindStateChange -= SwapConstants;
    }

    private void SwapConstants()
    {
        var externalForces = petals.externalForces;
        if (!blowing)
        {
            externalForces.enabled = true;
            blowing = true;
        }
        else
        {
            externalForces.enabled = false;
            blowing = false;
        }
    }
}
