using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    private WindZone wind;
    private void OnEnable()
    {
        GameManager.onWindStateChange += SwapWind;
    }

    private void OnDisable()
    {
        GameManager.onWindStateChange -= SwapWind;
    }

    private void Start()
    {
        wind = GetComponent<WindZone>();
        wind.windMain = 0;
    }

    private void SwapWind()
    {
        if (GameManager.Instance.IsWindy())
        {
            StartCoroutine(DoWindGradual(true));
        }
        else
        {
            StopCoroutine(DoWindGradual(false));
        }
        
    }

    IEnumerator DoWindGradual(bool increase)
    {
        if (increase)
        {
            while (wind.windMain < 3)
            {
                wind.windMain += 3 * Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(3f);
            wind.windMain = 0;
        }
    }
}
