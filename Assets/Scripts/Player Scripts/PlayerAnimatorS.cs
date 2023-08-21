using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorS : MonoBehaviour
{
    public enum AnimationAxis {Rows, Columns}

    private MeshRenderer meshRenderer;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 5.4f;
    [SerializeField] private int animationIndex = 0;

    private float deltaT;
    float horizontalInput;
    float verticalInput;
    int direction;

    private int frame;
    private int frameLoop = 0;  // A value to hold the number of the frame that the current animation loops on (e.g. after frame 13, loop it)
    private int frameReset = 0; // A value to hold the number of the frame that the current animation loops back to (e.g. the loop starts on frame 0)
    public bool activeCoroutine = false;    // The classic boolean to use when Update() needs to be quiet during a coroutine

    private float timeToIdle = 7f;
    private float idleTimer;

    private int _AttackIndex = 23;
    private int _DefeatIndex = 22;
    private int _BattleEnterIndex = 21;
    private int _BattleExitIndex = 20;
    private int _HurtIndex = 19;
    private int _BattleIdleIndex = 18;
    private int _BattleRedPotionindex = 17;
    private int _BattleBluePotionIndex = 16;
    private int _SpellcastIndex = 15;

    private int _ActiveIdleBackwardsIndex = 14;
    private int _ActiveIdleForwardsIndex = 13;
    private int _ActiveIdleLIndex = 12;
    private int _ActiveIdleRIndex = 11;
    private int _IdleBackwardsIndex = 10;
    private int _IdleForwardsIndex = 9;
    private int _IdleLIndex = 8;
    private int _IdleRIndex = 7;

    private int _ItemGetIndex = 6;
    private int _RedPotionIndex = 5;
    private int _BluePotionIndex = 4;

    private int _WalkBackwardsIndex = 3;
    private int _WalkLIndex = 2;
    private int _WalkRIndex = 1;
    private int _WalkForwardsIndex = 0;

    // All of the event controls that trigger special animations
    private void OnEnable()
    {
        GameManager.onSaveStatueStateChange += faceAway;
        GameManager.onChestStateChange += faceAway;
        ChestAnimatorS.onChestOpen += faceTowards;

        StaticInventoryDisplay.onHealthPotDrink += PlayRedPotionDrink;
        StaticInventoryDisplay.onManaPotDrink += PlayBluePotionDrink;
    }

    private void OnDisable()
    {
        GameManager.onSaveStatueStateChange -= faceAway;
        GameManager.onChestStateChange -= faceAway;
        ChestAnimatorS.onChestOpen -= faceTowards;

        StaticInventoryDisplay.onHealthPotDrink -= PlayRedPotionDrink;
        StaticInventoryDisplay.onManaPotDrink -= PlayBluePotionDrink;
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        deltaT = 0;
        idleTimer = 7f;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // The logic that determines what animation should be played 
        if (!GameManager.Instance.isSettings() && !activeCoroutine)
        {     
            if (GameManager.Instance.canMove()) // Logic for idle and walk cycles that occur during normal exploration
            {  
                if (isMoving(horizontalInput, verticalInput)) // If we're walking
                {
                    idleTimer = timeToIdle;
                    setDirection(horizontalInput, verticalInput);
                    frameLoop = 8;
                    frameReset = 0;
                    animationSpeed = 8f;
                    if (direction == 0)
                    {
                        animationIndex = _WalkRIndex;
                    }
                    else if (direction == 1)
                    {
                        animationIndex = _WalkLIndex;
                    }
                    else if (direction == 2)
                    {
                        animationIndex = _WalkBackwardsIndex;
                    }
                    else if (direction == 3)
                    {
                        animationIndex = _WalkForwardsIndex;
                    }
                }
                else    // If we're idling
                {
                    animationSpeed = 5.4f;
                    idleTimer -= Time.deltaTime; // Time to switch to another idle
                    if (direction == 0)
                    {
                        if(idleTimer <= 0)
                        {
                            StartCoroutine(DoSecondIdleAnim(0));
                        }
                        else
                        {
                            animationIndex = _ActiveIdleRIndex;
                            frameLoop = 5;
                        }
                        
                    }
                    else if (direction == 1)
                    {
                        if (idleTimer <= 0)
                        {
                            StartCoroutine(DoSecondIdleAnim(1));
                        }
                        else
                        {
                            animationIndex = _ActiveIdleLIndex;
                            frameLoop = 5;
                        }
                    }
                    else if (direction == 2)
                    {
                        if (idleTimer <= 0)
                        {
                            StartCoroutine(DoSecondIdleAnim(2));
                        }
                        else
                        {
                            animationIndex = _ActiveIdleBackwardsIndex;
                            frameLoop = 5;
                        }
                    }
                    else if (direction == 3)
                    {
                        if (idleTimer <= 0)
                        {
                            StartCoroutine(DoSecondIdleAnim(3));
                        }
                        else
                        {
                            animationIndex = _ActiveIdleForwardsIndex;
                            frameLoop = 5;
                        }
                    }
                }
            }

            else if (GameManager.Instance.isBattle())
            {
                animationSpeed = 5.4f;
                animationIndex = _BattleIdleIndex;
                frameLoop = 8;
            }

            else if (GameManager.Instance.isInventory())
            {
                animationSpeed = 5.4f;
                animationIndex = _ActiveIdleRIndex;
                direction = 0;
                frameLoop = 5;

                StopCoroutine(DoSecondIdleAnim(0));
                StopCoroutine(DoSecondIdleAnim(1));
                StopCoroutine(DoSecondIdleAnim(2));
                StopCoroutine(DoSecondIdleAnim(3));

            }

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

            // Animate
            frame = (int)(deltaT * animationSpeed);

            deltaT += Time.deltaTime;
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        } // End of the settings / activeCoroutine lockout
        
    }

    // Public methods -------------------------------------------------------------
    public void PlayRedPotionDrink()
    {
        StopAllCoroutines();
        StartCoroutine(DoPotionAnim(true));
    }

    public void PlayBluePotionDrink()
    {
        StopAllCoroutines();
        StartCoroutine(DoPotionAnim(false));
    }


    // Private methods ----------------------------------------------------------

    private bool isMoving(float hInput, float vInput)
    {
        if(horizontalInput != 0 || verticalInput != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void setDirection(float hInput, float vInput)
    {
        if (horizontalInput > 0 && verticalInput == 0) // Right
        {
            direction = 0;
        }
        else if (horizontalInput < 0 && verticalInput == 0) // Left
        {
            direction = 1; 
        }
        else if (horizontalInput == 0 && verticalInput > 0) // Up
        {
            direction = 2;
        }
        else if (horizontalInput == 0 && verticalInput < 0) // Down
        {
            direction = 3;
        }
    }

    // Coroutines ------------------------------------------------------------

    private IEnumerator DoSecondIdleAnim(int dir) // Plays the passive idle if the player stands still for long enough
    {
        // Startup stuff
        activeCoroutine = true;
        if(dir == 0)
        {
            animationIndex = _IdleRIndex;
        }
        else if(dir == 1)
        {
            animationIndex = _IdleLIndex;
        }
        else if(dir == 2)
        {
            animationIndex = _IdleBackwardsIndex;
        }
        else if(dir == 3)
        {
            animationIndex = _IdleForwardsIndex;
        }
        frameLoop = 5;
        deltaT = 0;
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

        // Animated component (sprite-based motion corresponding to physical motion)
        int frame = 0;// (int)(deltaT * animationSpeed);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * animationSpeed);
            yield return null;
        }
        yield return new WaitUntil(()=>isMoving(horizontalInput, verticalInput));
        deltaT = 0;

        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoPotionAnim(bool red)
    {
        // Startup stuff
        activeCoroutine = true;
        if (red)
        {
            if (GameManager.Instance.isBattle())
            {
                animationIndex = _BattleRedPotionindex;
                frameLoop = 14;
            }
            else
            {
                animationIndex = _RedPotionIndex;
                frameLoop = 16;
            }      
        }
        else
        {
            if (GameManager.Instance.isBattle())
            {
                animationIndex = _BattleBluePotionIndex;
                frameLoop = 14;
            }
            else
            {
                animationIndex = _BluePotionIndex;
                frameLoop = 16;
            }
        }
        
        deltaT = 0;
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

        int frame = 0;// (int)(deltaT * animationSpeed);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * animationSpeed);
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    // Animation control methods (soon to be deprecated)

    private void faceAway() // Faces the player in their idle stance away from the camera
    {
        direction = 2;
        animationIndex = _ActiveIdleBackwardsIndex;
        GameManager.onChestStateChange -= faceAway; // Prevents from turning back around on next state change; will probably change 
    }

    private void faceTowards() // Faces the player in their idle stance towards the camera
    {
        direction = 3;
        animationIndex = _ActiveIdleForwardsIndex;
    }
}


