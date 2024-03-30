using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableFoot : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform golemTransform;
    private Transform playerTransform;

    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected Rigidbody rb;
   
    public enum AnimationAxis { Rows, Columns }
    [SerializeField] protected AnimationAxis axis;

    [SerializeField] protected string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    private Vector3 startAndEnd;
    private Vector3 mid;
    private float atkTime = .065f;

    public bool damage = false;
    public bool end = false;
    private bool activeCoroutine = false;
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
        meshRenderer.material.SetFloat(clipKey, 15);
        meshRenderer.material.SetFloat(frameKey, 8);

        StartCoroutine(DoDuty());   // Do your sole purpose, little rock
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator DoDuty()
    {
        yield return new WaitUntil(()=>playerTransform != null);

        startAndEnd = new Vector3(golemTransform.position.x - .168f, golemTransform.position.y + 1.04f, golemTransform.position.z - .2f);
        mid = new Vector3(playerTransform.position.x + 4.77f, playerTransform.position.y + 3.5f, playerTransform.position.z - .18f);
        this.transform.position = startAndEnd;

        activeCoroutine = true;
        StartCoroutine(DoPosition(mid, atkTime));
        
        yield return new WaitUntil(() => !activeCoroutine);
        damage = true;

        activeCoroutine = true;
        StartCoroutine(DoPosition(startAndEnd, atkTime));
        yield return new WaitUntil(() => !activeCoroutine);
        end = true;
        Destroy(this.gameObject);
    }

    IEnumerator DoPosition(Vector3 targetPos, float step)
    {
        activeCoroutine = true;

        Vector3 velocity = (transform.position - targetPos) / 5f;    // Initial velocity values for the damping functions

        while (Vector3.Distance(transform.position, targetPos) >= .05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, step);
            yield return null;
        }
        activeCoroutine = false;
        yield return null;
    }

    public void SetValues(Transform playerTransform, Transform golemTransform)
    {
        this.playerTransform = playerTransform;
        this.golemTransform = golemTransform;
    }
}
