using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSwapper : MonoBehaviour
{
    [SerializeField] private float newMoveSpeed = 7f;
    [SerializeField] private int groundType = 0;

    private PlayerAnimatorS anim;
    void Start()
    {
        anim = GameObject.Find("Player Sprite").GetComponent<PlayerAnimatorS>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.PlayerMovement().movementSpeed = newMoveSpeed;
            //other.gameObject.GetComponent<PlayerMovement>().movementSpeed = newMoveSpeed;
           anim.walkType = groundType;
        }
        
    }
}
