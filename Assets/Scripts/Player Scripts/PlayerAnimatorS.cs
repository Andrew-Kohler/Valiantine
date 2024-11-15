using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorS : MonoBehaviour
{
    public enum AnimationAxis {Rows, Columns}

    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private AudioSource audioSource;
    [SerializeField] private string rowProperty = "_CurrRow", colProperty = "_CurrCol";

    [SerializeField] private AnimationAxis axis;
    [SerializeField] private float animationSpeed = 5.4f;
    [SerializeField] private int animationIndex = 0;
    [SerializeField] private List<AudioClip> playerSounds;

    private bool faceAwayBool;
    private bool faceDirBool;

    public int walkType = 0;    // 0 = stone, 1 = earth, 2 = wood, 3 = water
    private bool footBool1 = false;
    private bool footBool2 = false;

    private float deltaT;
    float horizontalInput;
    float verticalInput;
    int direction;
    

    private int frame;
    private int frameLoop = 0;  // A value to hold the number of the frame that the current animation loops on (e.g. after frame 13, loop it)
    private int frameReset = 0; // A value to hold the number of the frame that the current animation loops back to (e.g. the loop starts on frame 0)
    public bool activeCoroutine = false;    // The classic boolean to use when Update() needs to be quiet during a coroutine
    public bool defeated = false;

    private float timeToIdle = 7f;
    private float idleTimer;

    public bool dealDamage = false;         // A boolean flipped on and off to indicate when in the animation damage should be dealt to the player to keep things lined up
    public bool standStill = false;         // A bool to make the player quit their moving when something important is happening

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

    [SerializeField] private GameObject spellshinePrefab;

    // All of the event controls that trigger special animations
    private void OnEnable()
    {
        GameManager.onSaveStatueStateChange += faceAwayToggle;
        GameManager.onGateStateChange += faceAwayToggle;
        GameManager.onPlaqueStateChange += faceAwayToggle;
        GameManager.onRubbleStateChange += faceDirToggle;
        ChestInteractable.onChestInteract += faceAwayToggle;
        ChestAnimatorS.onChestOpen += PlayItemGet1;
        GateInteractable.onCastleInteract += faceAwayToggle;

        InGameUIView.onInteractionEnd += PlayItemGet2;

        StaticInventoryDisplay.onHealthPotDrink += PlayRedPotionDrink;
        StaticInventoryDisplay.onManaPotDrink += PlayBluePotionDrink;
    }

    private void OnDisable()
    {
        GameManager.onSaveStatueStateChange -= faceAwayToggle;
        GameManager.onGateStateChange -= faceAwayToggle;
        GameManager.onPlaqueStateChange -= faceAwayToggle;
        GameManager.onRubbleStateChange -= faceDirToggle;
        ChestInteractable.onChestInteract -= faceAwayToggle;
        ChestAnimatorS.onChestOpen -= PlayItemGet1;
        GateInteractable.onCastleInteract -= faceAwayToggle;

        InGameUIView.onInteractionEnd -= PlayItemGet2;

        StaticInventoryDisplay.onHealthPotDrink -= PlayRedPotionDrink;
        StaticInventoryDisplay.onManaPotDrink -= PlayBluePotionDrink;
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        deltaT = 0;
        idleTimer = 7f;
        rb = GetComponentInParent<Rigidbody>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!GameManager.Instance.isInteraction())
        {
            horizontalInput = playerMovement.horizontalInput;
            verticalInput = playerMovement.verticalInput;

        }
        else
        {
            horizontalInput = 0;
            verticalInput = 0;
        }

        // The logic that determines what animation should be played 
        if (!GameManager.Instance.isSettings() && !activeCoroutine)
        {     
            if (GameManager.Instance.canMove()) // Logic for idle and walk cycles that occur during normal exploration
            {  
                if (isMoving(horizontalInput, verticalInput) && !GameManager.Instance.isInteraction()) // If we're walking
                {
                    idleTimer = timeToIdle;
                    setDirection(horizontalInput, verticalInput);
                    frameLoop = 8;
                    frameReset = 0;

                    if (walkType == 3)  // Water go slower
                        animationSpeed = 4f;
                    else
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

            else if (GameManager.Instance.isTransition())
            {
                animationSpeed = 5.4f;
                idleTimer -= Time.deltaTime; // Time to switch to another idle
                
                if (direction == 0)
                {
                    if (idleTimer <= 0)
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

            else if (GameManager.Instance.isInteraction())
            {
                if (faceAwayBool)
                {
                    faceAway();
                }
                if (faceDirBool)
                {
                    faceDir();
                }
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
            if(walkType != 3)
            {
                if ((frame == 3) && !footBool1 && isMoving(horizontalInput, verticalInput))
                {
                    footBool1 = true;
                    playFootstep(frame);
                }
                if ((frame == 7) && !footBool2 && isMoving(horizontalInput, verticalInput))
                {
                    footBool2 = true;
                    playFootstep(frame);
                }
            }
            else
            {
                if ((frame == 2) && !footBool1 && isMoving(horizontalInput, verticalInput))
                {
                    footBool1 = true;
                    playFootstep(frame);
                }
                if ((frame == 6) && !footBool2 && isMoving(horizontalInput, verticalInput))
                {
                    footBool2 = true;
                    playFootstep(frame);
                }
            }
            

            
            /*if (GameManager.Instance.isInteraction())
            {
                animationIndex = _ActiveIdleForwardsIndex;
            }*/

            deltaT += Time.deltaTime;
            if (frame >= frameLoop)
            {
                deltaT = 0;
                frame = frameReset;
                footBool1 = false;
                footBool2 = false;
            }
            //Debug.Log(animationIndex + " " + _ActiveIdleForwardsIndex);
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
        } // End of the settings / activeCoroutine lockout
        
    }

    // Public methods -------------------------------------------------------------

    public int GetDirection()
    {
        return direction;
    }

    public void SetDirection(int newDir)
    {
        direction = newDir;
    }
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

    public void PlayItemGet1()
    {
        StopAllCoroutines();
        StartCoroutine(DoItemGet1());
    }

    public void PlayItemGet2()
    {
        if(animationIndex == _ItemGetIndex)
        {
            StopAllCoroutines();
            StartCoroutine(DoItemGet2());
        }
    }

    public void PlayBattleEnter(bool leftOfEnemy)
    {
        StopAllCoroutines();
        StartCoroutine(DoBattleEnterAnim(leftOfEnemy));
        audioSource.PlayOneShot(playerSounds[4], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
    }

    public void PlayAttack(Transform enemyTransform)
    {
        StopAllCoroutines();
        Vector3 attackPosition = new Vector3(enemyTransform.position.x - 1f, GetComponentInParent<Transform>().position.y, enemyTransform.position.z);
        StartCoroutine(DoAttackAnim(attackPosition));
    }

    public void PlayHurt()
    {
        StopAllCoroutines();
        StartCoroutine(DoHurtAnim());
        audioSource.PlayOneShot(playerSounds[8]);
    }

    public void PlaySpell(int index)
    {
        StopAllCoroutines();
        StartCoroutine(DoSpellAnim(index));
    }

    public void PlayBattleWin()
    {
        StopAllCoroutines();
        StartCoroutine(DoWinAnim());
    }

    public void PlayBattleExit()
    {
        StopAllCoroutines();
        StartCoroutine(DoBattleExitAnim());
    }

    public void PlayBattleLost()
    {
        StopAllCoroutines();
        StartCoroutine(DoDefeatAnim());
        audioSource.PlayOneShot(playerSounds[8], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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

    private void faceAwayToggle()
    {
        faceAwayBool = !faceAwayBool;
    }

    private void faceDirToggle()
    {
        faceDirBool = !faceDirBool;
    }

    private void faceAway() // Faces the player in their idle stance away from the camera
    {
        if (!activeCoroutine)
        {
            direction = 2;
            animationIndex = _ActiveIdleBackwardsIndex;
            animationSpeed = 5.4f;
            frameLoop = 5;
        }
        
        //GameManager.onChestStateChange -= faceAway; // Prevents from turning back around on next state change; will probably change 
    }

    private void faceDir() // Faces the player in their idle stance wherever they were looking
    {
        if (!activeCoroutine)
        {
            if(direction == 0)
                animationIndex = _ActiveIdleRIndex;
            if(direction == 1)
                animationIndex = _ActiveIdleLIndex;
            if (direction == 2)
                animationIndex = _ActiveIdleBackwardsIndex;
            if (direction == 3)
                animationIndex = _ActiveIdleForwardsIndex;
            animationSpeed = 5.4f;
            frameLoop = 5;
        }

        //GameManager.onChestStateChange -= faceAway; // Prevents from turning back around on next state change; will probably change 
    }

    private void playFootstep(int frame)
    {
        
        if (walkType == 0) // Stone
        {
            audioSource.Stop();
            if (frame == 3)
                audioSource.PlayOneShot(playerSounds[15], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            if(frame == 7)
                audioSource.PlayOneShot(playerSounds[16], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        }
        else if (walkType == 1) // Earth
        {
            audioSource.Stop();
            if (frame == 3)
                audioSource.PlayOneShot(playerSounds[19], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            if (frame == 7)
                audioSource.PlayOneShot(playerSounds[20], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        }
        else if (walkType == 2) // Wood
        {
            audioSource.Stop();
            if (frame == 3)
                audioSource.PlayOneShot(playerSounds[21], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            if (frame == 7)
                audioSource.PlayOneShot(playerSounds[22], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        }
        else // Water
        {
            if (frame == 2)
                audioSource.PlayOneShot(playerSounds[17], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            if (frame == 6)
                audioSource.PlayOneShot(playerSounds[18], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
        audioSource.PlayOneShot(playerSounds[14], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
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
        while (frame < frameLoop && !isMoving(horizontalInput, verticalInput))
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * animationSpeed);
            yield return null;
        }
        float humTimer = 5f;
        while(humTimer > 0 && !isMoving(horizontalInput, verticalInput))
        {
            humTimer -= Time.deltaTime;
            yield return null; 

        }
        audioSource.PlayOneShot(playerSounds[7], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        yield return new WaitUntil(()=>isMoving(horizontalInput, verticalInput));
        audioSource.Stop();
        deltaT = 0;

        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoPotionAnim(bool red)
    {
        // Startup stuff
        activeCoroutine = true;
        bool potBool = false;
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
            if (frame == 5 && !potBool)
            {
                potBool = true;
                audioSource.PlayOneShot(playerSounds[9], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }

            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoItemGet1()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
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
        animationIndex = _ItemGetIndex;
        animationSpeed = 6.4f;

        // Content ----------------------------------------------

        // Jump around and raise your arms
        int frame = 0;
        frameLoop = 8;
        while (frame < frameLoop)
        {
            
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * animationSpeed);
            deltaT += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator DoItemGet2()
    {
        // Lower your arms and set the direction, index, and frame appropriately
        activeCoroutine = true;
        
        
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
        frameLoop = 10;
        deltaT = 0;
        while (frame < frameLoop)
        {
            
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 7 + (int)(deltaT * animationSpeed);
            deltaT += Time.deltaTime;
            yield return null;
        }


        direction = 3;
        animationIndex = _IdleForwardsIndex;
        
        //deltaT = 0;
        activeCoroutine = false;
        GameManager.Instance.Animating(false) ;
        yield return null;
    }

    private IEnumerator DoBattleEnterAnim(bool leftOfEnemy)
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
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
        animationIndex = _BattleEnterIndex;
        animationSpeed = 5.4f;

        // Content ----------------------------------------------

        rb.velocity = Vector3.zero;
        float yStop = gameObject.GetComponentInParent<Transform>().position.y;
        frame = 0;// (int)(deltaT * animationSpeed);
        frameLoop = 14;

        // Play the startup animation
        while (frame < 3)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (4f * animationSpeed));
            yield return null;
        }

        if(leftOfEnemy)
            rb.AddForce(new Vector3(-10f, 3f, 0f), ForceMode.VelocityChange);
        else
            rb.AddForce(new Vector3(-13f, 3f, 0f), ForceMode.VelocityChange);

        // Play the in-air animation
        yield return new WaitForSeconds(.1f);
        deltaT = 0;
        while (gameObject.GetComponentInParent<Transform>().position.y > yStop)
        {
            if (frame > 4)
            {
                deltaT = 0;
            }
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 3 + (int)(deltaT * (animationSpeed));
            yield return null;
        }

        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Play the landing animation
        audioSource.PlayOneShot(playerSounds[13], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 5 + (int)(deltaT * (1.4f * animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoAttackAnim(Vector3 attackPosition) // Coroutine for the attack animation
    {
        // Startup stuff
        activeCoroutine = true;
        bool atkBool = false;
        int frame = 0;
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
        animationIndex = _AttackIndex;
        playerMovement.SetBattleIdlePosition();

        // Animation content ---------------------------------------------------
        while (frame < 5) // Play the windup animation
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 3 && !atkBool)
            {
                atkBool = true;
                audioSource.PlayOneShot(playerSounds[1], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }

        float yStop = gameObject.GetComponentInParent<Transform>().position.y;
        

        playerMovement.MovePlayerToPoint(attackPosition, .1f); // Start the player moving towards the enemy
        yield return new WaitForSeconds(.1f);
        deltaT = 0;
        while (gameObject.GetComponentInParent<Transform>().position.y > yStop) //  
        {
            deltaT += Time.deltaTime;
            frame = 5 + (int)(deltaT * (2f * animationSpeed));
            if (frame > 6)
            {
                deltaT = 0;
                frame = 5;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);

            yield return null;
        }

        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        // Play the attack animation
        deltaT = 0;
        audioSource.PlayOneShot(playerSounds[2], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < 14)
        {
            deltaT += Time.deltaTime;
            frame = 7 + (int)(deltaT * (1.5f * animationSpeed));
            if (frame >= 9)
            {
                dealDamage = true;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);

            yield return null;
        }

        playerMovement.MovePlayerToPoint(playerMovement.GetBattleIdlePosition(), .1f); // Start the player moving to their original spot
        yield return new WaitForSeconds(.3f);
        deltaT = 0;
        while (gameObject.GetComponentInParent<Transform>().position.y > yStop) // Play the return flying animation
        {
            deltaT += Time.deltaTime;
            frame = 14 + (int)(deltaT * (2f * animationSpeed));
            if (frame > 15)
            {
                deltaT = 0;
                frame = 14;
            }
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);

            yield return null;
        }

        // Play the landing animation
        deltaT = 0;
        frame = 16;
        audioSource.PlayOneShot(playerSounds[0], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < 18 && !playerMovement.GettingClose)
        {
            deltaT += Time.deltaTime;

            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 16 + (int)(deltaT * (animationSpeed));
            yield return null;
        }

        //yield return new WaitUntil(() => );

        deltaT = 0;
        rb.velocity = new Vector3(0f, 0f, 0f);

        deltaT = 0;
        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoHurtAnim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
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
        animationIndex = _HurtIndex;
        animationSpeed = 5.4f;
        frameLoop = 3;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoSpellAnim(int index)
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool shinespawn = false;
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
        animationIndex = _SpellcastIndex;
        animationSpeed = 5.4f;
        frameLoop = 13;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        audioSource.PlayOneShot(playerSounds[11], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if(frame == 6 && !shinespawn)
            {
                shinespawn = true;
                // Spawn the little prefab glimmer and make sure it's the correct color
                GameObject shine = Instantiate(spellshinePrefab, 
                    new Vector3(PlayerManager.Instance.PlayerTransform().position.x + 2.29f, 
                    PlayerManager.Instance.PlayerTransform().position.y + 5.18f,
                    PlayerManager.Instance.PlayerTransform().position.z - .1f), spellshinePrefab.transform.rotation);
                shine.GetComponent<SpellshineAnimatorS>().animationIndex = index;
                if(index != 5)
                    dealDamage = true;
            }
            yield return null;
        }

        activeCoroutine = false;
        yield return null;
    }

    private IEnumerator DoWinAnim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool winBool = false;
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
        animationIndex = _BattleExitIndex;
        animationSpeed = 5.4f;
        frameLoop = 10;
        int frameLoop2 = 12;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));
            if (frame == 6 && !winBool)
            {
                winBool = true;
                audioSource.PlayOneShot(playerSounds[12], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
            }
            yield return null;
        }
        deltaT = 0;
        audioSource.volume = GameManager.Instance.entityVolume * GameManager.Instance.masterVolume;
        audioSource.clip = playerSounds[6];
        audioSource.Play();
        animationSpeed = 1.4f;
        while (frame < frameLoop2)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 10 + (int)(deltaT * (animationSpeed));
            if(frame >= frameLoop2)
            {
                deltaT = 0;
                frame = 10;
            }
            yield return null;
        }
        

        yield return null;
    }

    private IEnumerator DoBattleExitAnim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
        bool shinespawn = false;
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
        animationIndex = _BattleExitIndex;
        animationSpeed = 5.4f;
        frameLoop = 17;
        audioSource.Stop();
        audioSource.clip = null;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        audioSource.PlayOneShot(playerSounds[14], GameManager.Instance.entityVolume * GameManager.Instance.masterVolume);
        while (frame < frameLoop)
        {
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 12 + (int)(deltaT * (animationSpeed));

            yield return null;
        }

        activeCoroutine = false;
        animationIndex = _ActiveIdleRIndex;
        frameLoop = 5;

        yield return null;
    }

    private IEnumerator DoDefeatAnim()
    {
        // Setup ----------------------------------------------
        activeCoroutine = true;
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
        animationIndex = _DefeatIndex;
        animationSpeed = 5.4f;
        frameLoop = 17;

        // Content ----------------------------------------------
        // Play the animation
        int frame = 0;
        while (frame < 4)
        {
            
            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = (int)(deltaT * (animationSpeed));

            yield return null;
        }

        deltaT = 0;
        animationSpeed = 1.6f;
        while (frame < 9)
        {

            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 4 + (int)(deltaT * (animationSpeed));

            yield return null;
        }

        deltaT = 0;
        animationSpeed = 5.4f;
        audioSource.PlayOneShot(playerSounds[5]);
        while (frame < frameLoop)
        {

            deltaT += Time.deltaTime;
            meshRenderer.material.SetFloat(clipKey, animationIndex);
            meshRenderer.material.SetFloat(frameKey, frame);
            frame = 9 + (int)(deltaT * (animationSpeed));

            yield return null;
        }

        defeated = true;

        yield return null;
    }

}


