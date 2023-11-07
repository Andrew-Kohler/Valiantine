using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearcrowMovement : EnemyMovement
{
    private float currentSpeed;
    public bool spawnedToFight; // For disabling the attempts of the spawned enemies to move about

    void Update()
    {
        if (!spawnedToFight) // If I'm an overworld enemy
        {
            direction = new Vector3(0f, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enemyCanMove()) // If we can move
        {
            rb.velocity = direction * currentSpeed * Time.fixedDeltaTime;
        }
    }
}
