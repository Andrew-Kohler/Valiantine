using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoButtons : MonoBehaviour
{
    PlayerInventory buttonRef;
    [SerializeField] GameObject Player;
    private void Start()
    {
        buttonRef = Player.GetComponent<PlayerInventory>();
    }
    public void changePlayerColor()
    {
        buttonRef.changePlayerMaterial();
    }
}
