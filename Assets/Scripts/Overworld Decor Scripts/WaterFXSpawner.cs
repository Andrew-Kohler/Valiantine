using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFXSpawner : MonoBehaviour
{
    private bool spawn = false;
    private bool activeCoroutine = false;
    [SerializeField] private GameObject sploosh;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spawn && !activeCoroutine)
        {
            StartCoroutine(DoSpawnSploosh());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawn = false;
        }
    }

    private IEnumerator DoSpawnSploosh()
    {
        activeCoroutine = true;
        Instantiate(sploosh, new Vector3(PlayerManager.Instance.PlayerTransform().position.x, this.transform.position.y + .02f, PlayerManager.Instance.PlayerTransform().position.z), Quaternion.identity, this.GetComponentInParent<Transform>());
        yield return new WaitForSeconds(.4f);
        activeCoroutine = false;
    }
}
