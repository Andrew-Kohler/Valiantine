using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerfallCutscene : MonoBehaviour
{
    [SerializeField] Animator animator; // Tower animator
    [SerializeField] ChestInteractable chest;   // Do this when we open the chest
    AudioSource audioS;
    [SerializeField] AudioClip towerfall1;
    [SerializeField] AudioClip towerfall2;
    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chest.Opened && !GameManager.Instance.towerfall)
        {
            StartCoroutine(DoTowerfall());
        }
    }

    IEnumerator DoTowerfall()
    {
        GameManager.Instance.towerfall = true;
        yield return new WaitForSeconds(4f);
        animator.Play("Fall", 0, 0);
        audioS.PlayOneShot(towerfall1, GameManager.Instance.masterVolume);
        yield return new WaitForSeconds(8f);
        audioS.PlayOneShot(towerfall2, GameManager.Instance.masterVolume);
        yield return null;
    }
}
