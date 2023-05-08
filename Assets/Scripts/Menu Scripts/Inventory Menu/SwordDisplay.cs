using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordDisplay : MonoBehaviour
{
    [SerializeField] Image shine;
    Animator shineAnimator;
    void Start()
    {
        shine.color = Color.clear;
        shineAnimator = shine.GetComponent<Animator>(); 
    }

    public void shineSword()
    {
        StartCoroutine(DoSwordShine());
    }

    IEnumerator DoSwordShine()
    {
        shine.color = Color.white;
        shineAnimator.Play("SwordShine", 0, 0);
        yield return new WaitForSeconds(.95f);
        shine.color = Color.clear;
        yield return null;
    }
}
