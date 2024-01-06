using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCam : MonoBehaviour
{
    bool activeCoroutine;

    // X, Y, and Z camera position modifers for different interactions in the menu
    [SerializeField] Vector3 mainPos = new Vector3(-14.5f, 5.15f, -6.599f);
    [SerializeField] Vector3 creditsPos = new Vector3(-13.59f, 8.98f, 18.76f);
    [SerializeField] Vector3 optionsPos = new Vector3(-15.53f, 10.89f, -33.55f);

    [SerializeField] Vector3 newGamePos = new Vector3(-14.5f, 12.99f, -6.599f);
    [SerializeField] Vector3 continueGamePos = new Vector3(13.51f, 5.15f, -1.63f);

    // X, Y, and Z camera angles for different interactions in the game
    [SerializeField] Vector3 mainAngle = new Vector3(4.82f, 90, 0);
    [SerializeField] Vector3 creditsAngle = new Vector3(4.82f, 90, 0);
    [SerializeField] Vector3 optionsAngle = new Vector3(4.82f, 90, 0);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = mainPos;
        transform.eulerAngles = mainAngle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CamToNew()
    {
        StopAllCoroutines();
        StartCoroutine(DoCamPosition(newGamePos, 2f, mainAngle));
    }

    public void CamToCont()
    {
        StopAllCoroutines();
        StartCoroutine(DoCamPosition(continueGamePos, 2f, mainAngle));
    }

    public void CamToMain()
    {
        StopAllCoroutines();
        StartCoroutine(DoCamPosition(mainPos, 1f, mainAngle));
    }

    public void CamToOptions()
    {
        StopAllCoroutines();
        StartCoroutine(DoCamPosition(optionsPos, 1f, optionsAngle));
    }

    public void CamToCredits()
    {
        StopAllCoroutines();
        StartCoroutine(DoCamPosition(creditsPos, 1f, creditsAngle));
    }

    IEnumerator DoCamPosition(Vector3 targetPos, float step, Vector3 targetRotation)
    {
        activeCoroutine = true;

        float xAngle;
        float yAngle;
        float zAngle;

        Vector3 velocity = Vector3.zero;    // Initial velocity values for the damping functions
        float xVelocity = 0f;
        float yVelocity = 0f;
        float zVelocity = 0f;

        while (Vector3.Distance(transform.position, targetPos) >= .05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, step); // Move camera position

            xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRotation.x, ref xVelocity, step);
            yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.y, ref yVelocity, step);
            zAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation.z, ref zVelocity, step);

            transform.eulerAngles = new Vector3(xAngle, yAngle, zAngle);    // Change camera rotation
            yield return null;
        }
        activeCoroutine = false;
        yield return null;
    }
}
