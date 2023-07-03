using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalWalk : MonoBehaviour
{
    [SerializeField] private ParticleSystem flowers;

    private void OnTriggerEnter(Collider other)
    {
        flowers.Play();
    }
}
