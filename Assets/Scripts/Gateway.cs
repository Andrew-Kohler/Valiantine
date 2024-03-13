/*
Gateway
Used on:    Scene transition objects ("Gateways")
For:    When the player enters a gateway, start the proper scene transition
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gateway : MonoBehaviour
{
   [SerializeField] public string gatewayName; // This should match for both gateways
   [SerializeField] public bool lostWoods = false;
   [SerializeField] public string destinationName;
   [SerializeField] public string levelToLoad; // The level the gateway leads to
   [SerializeField] public Transform spawnPoint;   // Where the player should spawn

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(lostWoods)
                SceneLoader.Instance.OnEnterGateway(destinationName, levelToLoad, lostWoods);
            else
                SceneLoader.Instance.OnEnterGateway(gatewayName, levelToLoad, lostWoods);
        }
    }
}
