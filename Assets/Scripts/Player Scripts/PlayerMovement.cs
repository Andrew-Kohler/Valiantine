/*
Player Movement
Used on:    Player
For:    Allows for keyboard control of player while exploring
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    public float movementSpeed = 7f;
    public float horizontalInput;
    public float verticalInput;
    private Vector3 velocity;

    public bool activeCoroutine = false;
    public bool GettingClose = false; // For animation purposes when moving to attack
    private Vector3 direction;
    private Vector3 idleBattlePosition; // Where the player landed when battle began
    private Vector3 orderedBattlePosition;  // Where the player is headed when ordered to move

    // So we don't fly off of slopes
    public bool grounded;
    private int stepsSinceLastGrounded;

    public delegate void OnInteractButton();
    public static event OnInteractButton onInteractButton;
    private void OnEnable()
    {
        GateInteractable.onCastleEnter += WalkIntoCastle;

    }
    private void OnDisable()
    {
        GateInteractable.onCastleEnter -= WalkIntoCastle;
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.canMove() && !GameManager.Instance.isCutscene()) // If we are allowed to move
        {
            // Uses old input system; I'd like to try the new one on my next project
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

        }
        else // Freezes the player if something else is going on, like entering a menu
        {
            if (!activeCoroutine)
            {
                horizontalInput = 0;
                verticalInput = 0;
                velocity = Vector3.zero;
                //rb.velocity = new Vector3(0f, 0f, 0f);
            }
            
        }

        if (Input.GetButtonDown("Interact"))
        {
            onInteractButton?.Invoke();
        }
        velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);


    }   // End of Update()

    private void FixedUpdate()
    {
        MovePlayer3D();
    }

    private void MovePlayer3D()
    {
        if (!activeCoroutine)
        {
            if (GameManager.Instance.canMove())
            {
                stepsSinceLastGrounded++;
                
                if (grounded || snapToGround())
                {
                    stepsSinceLastGrounded = 0;
                }
                rb.velocity = velocity;
            }
            else
            {
                rb.velocity = Vector3.zero;
                horizontalInput = 0;
                verticalInput = 0;
                velocity = Vector3.zero;
            }
        }
        
        
    }

    public void ForceDeactiveCoroutine()
    {
        activeCoroutine = false;
    }
    public void ZeroOutMovement()
    {
        horizontalInput = 0;
        verticalInput = 0;
        velocity = Vector3.zero;
    }
    public void SetBattleIdlePosition()
    {
        idleBattlePosition = this.transform.position;
        horizontalInput = 0;
        verticalInput = 0;
        velocity = Vector3.zero;

    }

    public Vector3 GetBattleIdlePosition()
    {
        return idleBattlePosition;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3) // Level geometry
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 3) // Level geometry
        {
            grounded = false;
        }
    }

    private bool snapToGround()
    {
        if(stepsSinceLastGrounded > 1) // If we've been up and about for too long
        {
            return false;
        }
        if (!Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit)) // If a downwards raycast can't find a home
        {
            return false;
        }

        // If we pass all these conditions, then we actually do need to be snapping
        float speed = velocity.magnitude;
        float dot = Vector3.Dot(velocity, hit.normal);  
        if(dot > 0) // So that we don't waste time realigning a useful velocity
            velocity = (velocity - hit.normal * dot).normalized * speed;
        return true;
    }

    #region CUTSCENE METHODS

    public void MovePlayerToPoint(Vector3 point, float distanceFromPoint)
    {
        StartCoroutine(MoveToPoint(point, distanceFromPoint));
    }

    public void WalkIntoCastle()
    {
        StartCoroutine(DoWalkIntoCastle());
    }

    private IEnumerator MoveToPoint(Vector3 point, float distanceFromPoint)
    {
        activeCoroutine = true;
        GettingClose = false;
        Vector3 direction = (point - this.transform.position);
        float yStop = this.gameObject.transform.position.y;

        // Method 2
        rb.AddForce(new Vector3(direction.x, 4f, direction.z), ForceMode.VelocityChange); //Mathf.Abs(direction.x * .27f)


        yield return new WaitUntil(()=> Mathf.Abs(this.transform.position.x - point.x) <= distanceFromPoint + .03f);
        GettingClose = true;
        activeCoroutine = false;
        yield return null;
        
    }

    private IEnumerator DoWalkIntoCastle()
    {
        activeCoroutine = true;
        float timer = 100f;
        while (timer > 0)
        {
            verticalInput = 1;
            rb.velocity = new Vector3(0, rb.velocity.y, 1 * movementSpeed);
            timer -= Time.deltaTime;
            yield return null;
        }
        activeCoroutine = false;
        //GameManager.Instance.Cutscene(false);
    }
    #endregion

}
