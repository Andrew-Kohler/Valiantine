using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalWalk : MonoBehaviour
{
    [SerializeField] private ParticleSystem flowers;
    [SerializeField] AudioClip clip;

    AudioSource source;
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        flowers.Play();
        source.PlayOneShot(clip, GameManager.Instance.environmentVolume * GameManager.Instance.masterVolume);
    }
}
