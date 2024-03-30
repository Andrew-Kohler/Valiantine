using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    [SerializeField] protected MeshRenderer meshRenderer;

    public enum AnimationAxis { Rows, Columns }
    [SerializeField] protected AnimationAxis axis;

    [SerializeField] protected string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    // 15 8

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        string clipKey, frameKey;
        if (axis == AnimationAxis.Rows)
        {
            clipKey = rowProperty;
            frameKey = colProperty;
        }
        else
        {
            clipKey = colProperty;
            frameKey = rowProperty;
        }
        meshRenderer.material.SetFloat(clipKey, 10);
        meshRenderer.material.SetFloat(frameKey, 24);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
