using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBar : MonoBehaviour
{
    Slider slider;
    [SerializeField] TextMeshProUGUI manaText;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        manaText.text = "MP: " + slider.value + "/" + slider.maxValue;
    }

    public void SetMaxMana(int mana)
    {
        slider.maxValue = mana;
    }

    public void SetMana(int mana)
    {
        slider.value = mana;
    }
}
