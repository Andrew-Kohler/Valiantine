using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemDisplay : MonoBehaviour
{
    // The display functions of a single gem holder
    private bool gemHeld;
    private bool gemEquipped;
    public bool GemHeld => gemHeld;
    public bool GemEquipped => gemEquipped;

    [SerializeField] Image gemImage;
    [SerializeField] Image equipOutline;
    private void Start()
    {
        if (!gemHeld)
        {
            gemImage.color = Color.clear;
        }
        if (!gemEquipped)
        {
            equipOutline.color = Color.clear;
        }
        
    }

    public void showGem()
    {
        gemHeld = true;
        gemImage.color = Color.white;
    }

    public void equipGem(bool equip)
    {
        if (equip)
        {
            gemEquipped = true;
            equipOutline.color= Color.white;
        }
        else
        {
            gemEquipped = false;
            equipOutline.color = Color.clear;
        }
    }

    
}
