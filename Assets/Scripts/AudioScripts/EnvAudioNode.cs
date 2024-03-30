using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvAudioNode : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource audioS;
    public float multiplier = 1f;
    void Start()
    {
     audioS = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
       audioS.volume = GameManager.Instance.environmentVolume * GameManager.Instance.masterVolume * multiplier;
    }
}
