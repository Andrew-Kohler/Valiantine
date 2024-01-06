using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnabledInTutorial : MonoBehaviour
{
    // This is just for UI elements that won't be there in the rest of the game
    void Start()
    {
        if(SceneManager.GetActiveScene().name != "0_Exterior")
        {
            this.gameObject.SetActive(false);
        }
    }
}
