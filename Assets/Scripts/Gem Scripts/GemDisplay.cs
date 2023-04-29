using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemDisplay : MonoBehaviour
{
    // The display functions of a single gem holder
    private bool gemHeld;
    public bool GemHeld => gemHeld;
    [SerializeField] Image gemImage;
    private void Start()
    {
        if (!gemHeld)
        {
            gemImage.color = Color.clear;
        }
        
    }

    public void showGem()
    {
        gemHeld = true;
        gemImage.color = Color.white;
    }

    
}
