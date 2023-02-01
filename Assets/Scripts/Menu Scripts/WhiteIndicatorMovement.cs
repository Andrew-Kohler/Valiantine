using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteIndicatorMovement : MonoBehaviour
{
    GameObject[] indicators;
    SpriteRenderer[] sr;

    float alpha;
    [SerializeField] float alphaStep = .01f;

    Vector3 zeroPos;
    Vector3 onePos;
    Vector3 twoPos;
    Vector3 threePos;

    [SerializeField] float xOffset1 = 1.5f;
    [SerializeField] float xOffset2 = 2.5f;
    [SerializeField] float yOffset1 = 4.75f;
    [SerializeField] float yOffset2 = 5.1f;
    [SerializeField] float yOffset3 = 5.5f;
    [SerializeField] float zOffset = 3f;

    void Start()
    {
        indicators = new GameObject[4];
        sr = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++)  // Get all of the indicator boxes within this parent and by default set them to be transparent
        {
            indicators[i] = this.gameObject.transform.GetChild(i).gameObject;
            sr[i] = indicators[i].GetComponent<SpriteRenderer>();
            sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, 0f);
        }

        SetPositions(); 
        enabled = false; // Ok, enabled deals with scripts
        
    }

    private void Update()
    {
        
    }

    void SetPositions() // Sets the coordinate positions of the squares based on their array positions
    {
        zeroPos = new Vector3(this.gameObject.transform.parent.position.x + xOffset1, this.gameObject.transform.parent.position.y + yOffset1, this.gameObject.transform.parent.position.z - zOffset);
        onePos = new Vector3(this.gameObject.transform.parent.position.x - xOffset2, this.gameObject.transform.parent.position.y + yOffset2, this.gameObject.transform.parent.position.z);
        twoPos = new Vector3(this.gameObject.transform.parent.position.x - xOffset1, this.gameObject.transform.parent.position.y + yOffset3, this.gameObject.transform.parent.position.z + zOffset);
        threePos = new Vector3(this.gameObject.transform.parent.position.x + xOffset2, this.gameObject.transform.parent.position.y + yOffset2, this.gameObject.transform.parent.position.z);

        indicators[0].transform.position = zeroPos;
        indicators[1].transform.position = onePos;
        indicators[2].transform.position = twoPos;
        indicators[3].transform.position = threePos;
    }

    public float GetAlphaStep()
    {
        return alphaStep;
    }

    public IEnumerator DoFlashOut()
    {
        alpha = 1;
        while(alpha > 0)
        {
            for (int i = 0; i < 4; i++)  // Set these to 1f
            {
                sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, alpha);
            }
            alpha -= alphaStep;
            yield return null;
        }

        yield return null;
    }

    

}
