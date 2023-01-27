/*
Player Inventory
Used on:    Player
For:    Currently, only cares about the material of the player for testing purposes. Later revisions will transform it
        into the player's inventory.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    MeshRenderer mRenderer;
    private bool isBlue;

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material newMaterial;

    private void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mRenderer.material = defaultMaterial;
        isBlue = false;
    }
    public void changePlayerMaterial()
    {
        if (isBlue)
        {
            mRenderer.material = defaultMaterial;
        }
        else
        {
            mRenderer.material = newMaterial;
        }
        isBlue = !isBlue;
    }
}