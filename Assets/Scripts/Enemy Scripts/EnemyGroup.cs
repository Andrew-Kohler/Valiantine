using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] private bool spawn = false;    // Whether or not we're spawning new friends
    [SerializeField] private int numberToSpawn = 0; // How many new friends we're spawning
    [SerializeField] private GameObject[] additionalEnemies;    // Our new friends
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEncounter()    // Called at the start of a battle
    {
        if (spawn)  // If we are indeed spawning enemies at all
        {

        }
    }
}
