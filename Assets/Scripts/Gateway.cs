using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gateway : MonoBehaviour
{
   [SerializeField] public string gatewayName; // This should match for both gateways
   [SerializeField] public string levelToLoad; // The level the gateway leads to
   [SerializeField] public Transform spawnPoint;   // Where the player should spawn

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneLoader.Instance.OnEnterGateway(gatewayName, levelToLoad);
        }
    }
}
