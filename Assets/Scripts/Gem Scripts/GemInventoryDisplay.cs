using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemInventoryDisplay : MonoBehaviour
{
    [SerializeField] private List<GameObject> gemDisplays;   // A list of all gem displays for easy access
    [SerializeField] private List<Sprite> ittyBitty;   // All the itty bitty gem assets
    [SerializeField] private Image currentIttyBitty;    // The currently enshrined itty bitty
    [SerializeField] private Image sword;
    [SerializeField] private GemSystem gemSystem;
    [SerializeField] private StatDisplay statDisplay;

    /*[SerializeField] private GameObject indicator;          // The little arrow that shows what gem you have selected
    [SerializeField] private GameObject equipTag;          // The little arrow that shows what gem you have selected*/
    [SerializeField] private GameObject selectArrow;
    [SerializeField] private GameObject textTabIndicator;   // The little arrow that shows which way to scroll in the text box
    [SerializeField] private TextMeshProUGUI tabText;
    InventoryArrow selectorArrow;

    [SerializeField] float arrowSpeed = .5f;
    [SerializeField] float yOffset = 10f;
    private float waitReset = 7.0f;
    private float waitTime;

    [SerializeField] private List<AudioClip> sounds;
    private AudioSource audioS;

    private bool activeCoroutine;
    private bool showSpell;
    private bool gemChosen;
    private bool pointerUpdated;
    private string currentText;
    public string CurrentText => currentText;
    private int selectedSlot;
    private int equippedSlot;

    // Current order in these lists: 
    // Will (starter)
    // Courage
    // Patience
    // Constitution
    // Cunning
    // Great Patience
    // Heart

    public delegate void OnSelectedGemChange();
    public static event OnSelectedGemChange onSelectedGemChange;
    public delegate void OnGemSwap();
    public static event OnGemSwap onGemSwap;

    private void Start()
    {
        selectorArrow = selectArrow.GetComponent<InventoryArrow>();
        //Init();
        
        selectedSlot = 0;
        
        UpdateText();
        UpdateStatDisplay();
        currentIttyBitty.sprite = ittyBitty[0];
        audioS = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        selectedSlot = 0;
        equippedSlot = -1;
        waitTime = waitReset;

        gemChosen = false;
        if (showSpell)
        {
            textTabIndicator.transform.Rotate(new Vector3(0, 0, 180));
        }
        showSpell = false;
        pointerUpdated = false;
        activeCoroutine = false;

        UpdateText();
        UpdateStatDisplay();

        Init();
        onSelectedGemChange?.Invoke();
        selectArrow.SetActive(true);
    }

    private void Update()
    {
        if (!activeCoroutine)
        {   
            if (!gemChosen)
            {
                if (!pointerUpdated)
                {
                    UpdatePointerFast();
                    pointerUpdated = true;
                }

                if (Input.GetButtonDown("Inventory Left"))  // Move the selection arrow left
                {
                    audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
                    StartCoroutine(DoMoveLeft());
                    onSelectedGemChange?.Invoke();
                }
                else if (Input.GetButtonDown("Inventory Right")) // Move the selection arrow right
                {
                    audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
                    StartCoroutine(DoMoveRight());
                    onSelectedGemChange?.Invoke();
                }
                else if (Input.GetButtonDown("Inventory Up") && showSpell && gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)  // Change descriptive text
                {
                    showSpell = false;
                    tabText.text = "S";
                    UpdateText();
                    textTabIndicator.transform.Rotate(new Vector3(0, 0, 180));
                    waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time
                }
                else if (Input.GetButtonDown("Inventory Down") && !showSpell && gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
                {
                    showSpell = true;
                    tabText.text = "W";
                    UpdateText();
                    textTabIndicator.transform.Rotate(new Vector3(0, 0, 180));
                    waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time
                }
                else if (Input.GetButtonDown("Interact"))   // Start the thing where we ask if they player wants to equip the gem
                {
                    audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
                    if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld && !gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemEquipped)
                    {
                        gemChosen = true;
                        // equipTag.SetActive(true);
                        // Change the indicator to a "Equip?" prompt similar to the one in the other inventory
                        selectorArrow.selectorSwap();
                    }
                    else
                    {
                        // In the far, far future, this will make a sound play.
                    }


                }

            }
            else    // If we have selected a gem
            {
                waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time
                if (Input.GetButtonDown("Interact")) // If we choose to equip that gem
                {
                    audioS.PlayOneShot(sounds[1], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
                    if (equippedSlot != -1) // TODO: Find a way to give the player the Gem of Will and have it equipped already when the game begins
                    {
                        gemDisplays[equippedSlot].GetComponent<GemDisplay>().equipGem(false); // Turn off the outline on the old selected one
                    }
                    gemSystem.equipGem(selectedSlot);                                       // Alert the backend gem system that a change has been made
                    gemSystem.currentGemIndex = selectedSlot;
                    gemDisplays[selectedSlot].GetComponent<GemDisplay>().equipGem(true);    // Turn on the outline on the new selected one
                    equippedSlot = selectedSlot;
                    UpdateStatDisplay();    // Alert the stat display that a change has been made
                    currentIttyBitty.sprite = ittyBitty[equippedSlot];      // Alert the itty bitty gem that a change has been made

                    // Revert the changes made to the indicator
                    selectorArrow.selectorSwap();
                    gemChosen = false;

                    if (GameManager.Instance.isBattle())
                    {
                        currentText = "Gem equipped!";
                        onGemSwap?.Invoke(); 
                        StartCoroutine(DoWait());
                    }

                }
                else if (Input.GetButtonDown("Return"))
                {
                    // Revert the changes made to the indicator
                    audioS.PlayOneShot(sounds[2], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);
                    selectorArrow.selectorSwap();
                    gemChosen = false;
                }
            }

            // For activating the shine
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                StartCoroutine(DoShine());
            }
        }
    }

    // Methods ----------------------------------

    private void UpdateText()
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
            textTabIndicator.SetActive(true);
            if (showSpell)
            {
                currentText = gemSystem.HeldGemList[selectedSlot].UseDescription;
                tabText.text = "W";
            }
            else
            {
                currentText = gemSystem.HeldGemList[selectedSlot].InventoryDescription;
                tabText.text = "S";
            }
            
        }
        else
        {
            textTabIndicator.SetActive(false);
            currentText = "Perhaps this blade holds secrets yet...";
            tabText.text = "";
        }
    }

    private void UpdatePointer()
    {
        Vector3 selectedPosition = gemDisplays[selectedSlot].transform.position;
        selectorArrow.moveArrow(new Vector2(selectedPosition.x, selectedPosition.y + yOffset), arrowSpeed, true);
    }

    private void UpdatePointerFast()
    {
        Vector3 selectedPosition = gemDisplays[selectedSlot].transform.position;
        selectorArrow.moveArrow(new Vector2(selectedPosition.x, selectedPosition.y + yOffset), .001f, false);
    }
    private void UpdateStatDisplay()    // Feeds the stat display the relevant data from making stat predictions
    {
        if (gemDisplays[selectedSlot].GetComponent<GemDisplay>().GemHeld)
        {
            statDisplay.SetRelevantGemStats(gemSystem.CurrentGem, gemSystem.GemStats[selectedSlot]);
        }
        else
        {
            statDisplay.SetRelevantGemStats(gemSystem.CurrentGem, gemSystem.CurrentGem);
        }
    }

    private void obtainGem(ItemData data)
    {
        gemDisplays[data.MPRestore].GetComponent<GemDisplay>().showGem();
    }

    private void Init()
    {
        for (int i = 0; i < gemSystem.HeldGemList.Length; i++)
        {
            if (gemSystem.HeldGemList[i] != null)
            {
                gemDisplays[i].GetComponent<GemDisplay>().showGem();
                if (i == gemSystem.currentGemIndex)
                {
                    if (equippedSlot != -1) // TODO: Find a way to give the player the Gem of Will and have it equipped already when the game begins
                    {
                        gemDisplays[equippedSlot].GetComponent<GemDisplay>().equipGem(false); // Turn off the outline on the old selected one
                    }
                    equippedSlot = i;
                    UpdateStatDisplay();
                    UpdateText();
                    gemDisplays[i].GetComponent<GemDisplay>().equipGem(true);
                    currentIttyBitty.sprite = ittyBitty[equippedSlot];
                }
            }
        }


    }

    // Coroutines ----------------------------------
    #region COROUTINES
    IEnumerator DoMoveLeft()
    {
        activeCoroutine = true;
        if (selectedSlot == 0)
        {
            selectedSlot = 6;
        }
        else
        {
            selectedSlot = selectedSlot - 1;
        }

        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();
        waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time

        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoMoveRight()
    {
        activeCoroutine = true;
        if (selectedSlot == 6)
        {
            selectedSlot = 0;
        }
        else
        {
            selectedSlot = selectedSlot + 1;
        }

        UpdateText();
        UpdatePointer();
        UpdateStatDisplay();
        waitTime = waitReset; // Player is interacting, that's a no-no for the gem shine and we reset the time

        activeCoroutine = false;
        yield return null;
    }

    IEnumerator DoShine()
    {
        waitTime = waitReset; // Reset the timer
        for (int i = 0; i < 7; i++)
        {
            if (gemDisplays[i].GetComponent<GemDisplay>().GemHeld)
            {
                gemDisplays[i].GetComponent<GemDisplay>().shineGem();
            }
            if(i == selectedSlot)   // Spin the pointer in time with the shine!
            {
                UpdatePointer();
            }
            yield return new WaitForSeconds(.16f);
        }
        sword.GetComponent<SwordDisplay>().shineSword();
        waitTime = waitReset; // Reset the timer
        yield return null;
    }

    IEnumerator DoWait()    // Exists to freeze this class until the inventory closes
    {
        activeCoroutine = true;
        selectorArrow.selectorSwap();
        gemChosen = false;
        selectArrow.SetActive(false);
        yield return new WaitForSeconds(5f);
        activeCoroutine = false;
    }
    #endregion

}
