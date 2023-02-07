using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBar : MonoBehaviour
{
    Slider slider;
    [SerializeField] TextMeshProUGUI healthText;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        healthText.text = "HP: " + slider.value + "/" + slider.maxValue;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
