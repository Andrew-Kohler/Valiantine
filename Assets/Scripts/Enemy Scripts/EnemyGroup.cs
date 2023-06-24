using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] private bool spawn = false;    // Whether or not we're spawning new friends
    [SerializeField] public int numberToSpawn = 0; // How many new friends we're spawning (max of 2)
    [SerializeField] private GameObject[] additionalEnemies;    // Our new friends
    [SerializeField] public int lowerLVLBound;
    [SerializeField] public int upperLVLBound;
    public int[] additionalEnemyLevels;    // Levels generated for everyone in the encounter, including the original enemy
    [SerializeField] private Transform spawnpoint;
    private void Awake()
    {
        additionalEnemyLevels = new int[numberToSpawn + 1];
        for(int i = 0; i < additionalEnemyLevels.Length; i++)
        {
            additionalEnemyLevels[i] = Random.Range(lowerLVLBound, upperLVLBound + 1);
        }
    }

    public void SpawnEncounter()    // Called at the start of a battle
    {
        if (spawn)  // If we are indeed spawning enemies at all
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                Instantiate(additionalEnemies[i], spawnpoint.position, additionalEnemies[i].transform.rotation);
            }
        }
    }
}
