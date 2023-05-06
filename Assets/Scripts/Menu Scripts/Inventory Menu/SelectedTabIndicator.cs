using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the little line under the tabs

public class SelectedTabIndicator : MonoBehaviour
{
    public void moveLine(Vector2 targetPos, float step)
    {
        StopAllCoroutines();
        StartCoroutine(DoPosition(targetPos, step));
    }

    IEnumerator DoPosition(Vector2 targetPos, float step)
    {
        Vector2 velocity = Vector2.zero;    // Initial velocity values for the damping functions

        while (Vector2.Distance(transform.position, targetPos) >= .01f)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref velocity, step); // Move line position

            yield return null;
        }

        yield return null;
    }
}
