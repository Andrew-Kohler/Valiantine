using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabIcon : MonoBehaviour
{
    [SerializeField] Image tabImage;
    [SerializeField] Sprite tabIcon;
    void Start()
    {
        tabImage.sprite = tabIcon;
    }

}
