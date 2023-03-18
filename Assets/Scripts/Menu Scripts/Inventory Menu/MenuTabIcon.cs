using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabIcon : MonoBehaviour
{
    [SerializeField] Image tabImage;
    [SerializeField] Sprite tabIcon;
    [SerializeField] Image selectedBox;
    void Start()
    {
        tabImage.sprite = tabIcon;
        selectedBox.color = Color.clear;
    }

    // Update is called once per frame
   /* void Update()
    {
        
    }*/

    public void Select()
    {
        selectedBox.color = Color.white;
    }

    public void Deselect()
    {
        selectedBox.color = Color.clear;
    }
}
