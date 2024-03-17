using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFollower : MonoBehaviour
{
    // Start is called before the first frame update
    CanvasGroup cg;
    [SerializeField] CanvasGroup parentCG;
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        cg.alpha = parentCG.alpha;
    }
}
