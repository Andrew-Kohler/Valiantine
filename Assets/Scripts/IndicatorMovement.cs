using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorMovement : MonoBehaviour
{
    Vector3 zeroPos;
    Vector3 onePos;
    Vector3 twoPos;
    Vector3 threePos;

    GameObject[] indicators;

    [SerializeField] float xOffset1 = 1.5f;
    [SerializeField] float xOffset2 = 2.5f;
    [SerializeField] float yOffset1 = 4.75f;
    [SerializeField] float yOffset2 = 5.1f;
    [SerializeField] float yOffset3 = 5.5f;
    [SerializeField] float zOffset = 3f;

    [SerializeField] float delay = .2f;
    [SerializeField] float moveSpeed = .04f;
    float moveSpeed2;

    [SerializeField] float rotationSpeed;
    float rotationSpeed2;
    float rotationStep;

    float horizontalInput;
    bool activeCoroutine;
    bool keepGoingCheck;

    // Start is called before the first frame update
    void Start()
    {
       
        indicators = new GameObject[4];
        for(int i = 0; i < 4; i++)  // Get all of the indicator boxes within this parent
        {
            indicators[i] = this.gameObject.transform.GetChild(i).gameObject;
        }

        activeCoroutine = false;
        keepGoingCheck = true;
        moveSpeed2 = moveSpeed * 1.582f;
        rotationSpeed = 360f * moveSpeed / 3.5f;
        rotationSpeed2 = 360f * moveSpeed / 3.5f;
        rotationStep = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        //SetRotations();
        SetColors();
        horizontalInput = Input.GetAxis("Horizontal");

        if (GameManager.Instance.isBattle() && !activeCoroutine) // TODO: We can worry about revealing these at a correct time later
        {
            
            SetPositions();
            //SetRotations(); // TODO: Find a way to keep this enabled for everything except the front one during the coroutine
            
            if(horizontalInput > 0) // Indicates a rightward movement
            {
                StartCoroutine(RightCoroutine());
            }
            else if(horizontalInput < 0) // Indicates a leftward movement
            {
                StartCoroutine(LeftCoroutine());
            }
            if(horizontalInput == 0)
            {
                keepGoingCheck = true;
            }

        }
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

    /*void SetRotations() // Ensures that the indicators are always facing the camera
    {
        // Have the y value of each one face towards the camera
        for (int i = 0; i < 4; i++)
        {
            //indicators[i].transform.LookAt(new Vector3(indicators[i].transform.position.x, indicators[i].transform.position.y, Camera.main.transform.position.z));
        }
    }*/

    void SetColors()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if(i != 0)
            {
                indicators[i].GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f); 
            }
            else
            {
                indicators[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    public string GetLeadBox()
    {
        if (activeCoroutine)
        {
            return "";
        }
        else
        {
            return indicators[0].name;
        }
        
    }

    IEnumerator LeftCoroutine() // The timed sequence which rotates the indicators a quarter-circle left
    {
        activeCoroutine = true;

        while (Vector3.Distance(indicators[0].transform.position, onePos) > .01)    // Move the indicators towards their positions to make it feel natural
        {
            indicators[0].transform.position = Vector3.MoveTowards(indicators[0].transform.position, onePos, moveSpeed2);
            indicators[1].transform.position = Vector3.MoveTowards(indicators[1].transform.position, twoPos, moveSpeed);
            indicators[2].transform.position = Vector3.MoveTowards(indicators[2].transform.position, threePos, moveSpeed2);
            indicators[3].transform.position = Vector3.MoveTowards(indicators[3].transform.position, zeroPos, moveSpeed);

            indicators[3].transform.rotation = Quaternion.Euler(0f, indicators[3].transform.rotation.y + rotationStep, 0f);
            rotationStep += rotationSpeed;

            yield return null;
        }

        GameObject temp = indicators[3];    // Formally assing the indicators their new positions
        for (int i = indicators.Length - 2; i >= 0; i--)
        {
            indicators[i + 1] = indicators[i];
        }
        indicators[0] = temp;

        indicators[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rotationStep = 0f;

        if (keepGoingCheck)
        {
            StartCoroutine(KeepGoingCheckCoroutine());
        }
        else
        {
            activeCoroutine = false;
        }
        
        yield return null;
    }

    IEnumerator RightCoroutine() // The timed sequence which rotates the indicators a quarter-circle right
    {
        activeCoroutine = true;

        while (Vector3.Distance(indicators[0].transform.position, threePos) > .01)    // Move the indicators towards their positions to make it feel natural
        {
            indicators[0].transform.position = Vector3.MoveTowards(indicators[0].transform.position, threePos, moveSpeed);
            indicators[1].transform.position = Vector3.MoveTowards(indicators[1].transform.position, zeroPos, moveSpeed2);
            indicators[2].transform.position = Vector3.MoveTowards(indicators[2].transform.position, onePos, moveSpeed);
            indicators[3].transform.position = Vector3.MoveTowards(indicators[3].transform.position, twoPos, moveSpeed2);

            indicators[1].transform.rotation = Quaternion.Euler(0f, indicators[1].transform.rotation.y + rotationStep, 0f);
            rotationStep += rotationSpeed2;

            yield return null;
        }

        GameObject temp = indicators[0];    // Formally assing the indicators their new positions
        for (int i = 1; i <= indicators.Length - 1; i++)
        {
            indicators[i - 1] = indicators[i];
        }
        indicators[3] = temp;

        indicators[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rotationStep = 0f;

        if (keepGoingCheck)
        {
            StartCoroutine(KeepGoingCheckCoroutine());
        }
        else
        {
            activeCoroutine = false;
        }

        
        yield return null;
    }

    IEnumerator KeepGoingCheckCoroutine()
    {
        yield return new WaitForSeconds(delay);
        if(horizontalInput != 0)
        {
            keepGoingCheck = false;
        }
        activeCoroutine = false;
    }
}
