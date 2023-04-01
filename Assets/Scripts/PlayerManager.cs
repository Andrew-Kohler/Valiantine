using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    //[SerializeField] GameObject _player;

    // So, this is wholly temporary architecture right here. In the future, once the player has all the stuff on them
    // I think they'll need, this can be revised to create the prefab of the player. But, for now, its primary
    // purpose is to give everything else a much easier go of accessing critical player components.

    // Have fun, future me.
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // This shouldn't happen, there's always an instance of this on the player!
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    public PlayerStats PlayerStats()
    {
        return GetComponent<PlayerStats>();
    }

    public Rigidbody PlayerRigidbody()
    {
        return GetComponent<Rigidbody>();
    }

    public Transform PlayerTransform()
    {
        return this.transform;
    }

    public InventoryHolder PlayerInventory()
    {
        return GetComponent<InventoryHolder>();
    }

    public string PlayerName()
    {
        return this.name;
    }

    public PlayerMovement PlayerMovement()
    {
        return GetComponent<PlayerMovement>();
    }



    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
