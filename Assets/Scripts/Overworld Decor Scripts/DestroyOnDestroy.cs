using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject monitor;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     if(monitor == null)  
            Destroy(this.gameObject);
    }
}
