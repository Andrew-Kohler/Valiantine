using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledTowerfall : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject preTowerfall;
    [SerializeField] GameObject postTowerfall;
    void Start()
    {
        if (GameManager.Instance.towerfall) // If the tower has fallen
        {
            preTowerfall.SetActive(false);
            postTowerfall.SetActive(true);
        }
        else
        {
            preTowerfall.SetActive(true);
            postTowerfall.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
