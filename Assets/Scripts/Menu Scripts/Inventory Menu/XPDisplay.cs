using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LVLNum;
    [SerializeField] TextMeshProUGUI XPNum;

    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    /*private void OnEnable()
    {     
        XPNum.text = playerStats.GetXP().ToString() + "/" + playerStats.GetXPThreshold().ToString() + " XP";
        LVLNum.text = playerStats.GetLVL().ToString();
        slider.value = playerStats.GetXP();
        slider.maxValue = playerStats.GetXPThreshold();
    }*/

    public void SetXP(int xp, int maxXP)
    {
        slider.value = xp;
        slider.maxValue = maxXP;
        XPNum.text = xp.ToString() + "/" + maxXP.ToString() + " XP";
    }

    public void SetLVL(int lvl)
    {
        LVLNum.text = lvl.ToString();
    }
}
