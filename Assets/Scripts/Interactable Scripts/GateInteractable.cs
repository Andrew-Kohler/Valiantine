using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateInteractable : Interactable
{
    [Header("Save Index")]
    public int saveIndex = 0;

    [Header("Gate Components")]
    [SerializeField] GameObject leftGate;       // The gates, their pivot points, and the interaction trigger
    [SerializeField] GameObject leftGatePivot;
    [SerializeField] GameObject rightGate;
    [SerializeField] GameObject rightGatePivot;
    [SerializeField] GameObject trigger;

    [Header("Gate Key")]
    [SerializeField] ItemData key;  // The item required to open the gates

    [Header("Additional Considerations")]
    [SerializeField] public bool mainDoors;
    [SerializeField] float gateOpenSpeed;   // Speed at which the gates open

    bool validKey;
    bool validInteraction;

    public delegate void OnCastleInteract();
    public static event OnCastleInteract onCastleInteract;
    public delegate void OnCastleEnter();
    public static event OnCastleEnter onCastleEnter;

    private void OnEnable()
    {
        InGameUIView.onInteractionEnd += GateCheck;
    }

    private void OnDisable()
    {
        InGameUIView.onInteractionEnd -= GateCheck;
    }

    private void Start()
    {
        if (!mainDoors)
        {
            if (GameManager.Instance.openedGates[saveIndex])
            {
                StartCoroutine(DoSceneLoadGateOpen());
            }
        }
        
    }

    public override void Interact()
    {
        StartCoroutine(DoInteraction());
    }

    private void GateCheck()
    {
        if (!mainDoors)
        {
            if (validInteraction && validKey)
                StartCoroutine(DoGateOpen()); // Add redundancy so that this only happens when the gate is actually checked
        }
        else
        {
            if (validInteraction)
            {
                onCastleInteract?.Invoke();
                StartCoroutine(DoGateOpen()); // Add redundancy so that this only happens when the gate is actually checked
            }
                
        }
        
    }

    protected override IEnumerator DoInteraction()
    {
        validInteraction = true;
        usePrompt.GetComponent<UsePrompt>().FadeOut(); // Fades out the little indicator

        // Check for if the player has the item necessary to open the gate
        lines.Clear();
        if (!mainDoors)
        {
            if (PlayerManager.Instance.PlayerInventory().InventorySystem.ContainsItem(key))
            {
                if(SceneManager.GetActiveScene().name == "14_DarkLever")
                    lines.Add("You ram the gate with your shoulder, knocking the rust of ages loose from the lock. The gate swings open.");
                else
                    lines.Add("The " + key.DisplayName + " fits right in the lock. You hear a click as you turn it.");
                validKey = true;
            }
            else
            {
                lines.Add("The gate is locked tight, even after all this time.");
                lines.Add("The lock looks like a " + key.DisplayName + " would fit right in.");
            }
        }
        else
        {
            lines.Add("The doors hang loose on their hinges, as though they believe there is nothing left to guard.");
            lines.Add("But you know better.");
        }
        

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);

        yield return null;
    }

    private IEnumerator DoGateOpen() // Opens the gates by rotating them
    {
        if (mainDoors)
        {
            GameManager.Instance.Cutscene(true);
        }
        trigger.SetActive(false);   // Disable the ability to interact with the gate; it's open, we're done
        float count = 0;
        while (count <= 90)
        {
            leftGate.transform.RotateAround(leftGatePivot.transform.position, Vector3.up, -gateOpenSpeed * Time.deltaTime);
            rightGate.transform.RotateAround(rightGatePivot.transform.position, Vector3.up, gateOpenSpeed * Time.deltaTime);
            count += gateOpenSpeed * Time.deltaTime;
            yield return null;
        }
        count = 0;
        validInteraction = false;
        

        if (mainDoors)
        {
            onCastleEnter?.Invoke();
        }
        else
        {
            GameManager.Instance.openedGates[saveIndex] = true;
        }
        
        yield return null;
        
    }

    private IEnumerator DoSceneLoadGateOpen()
    {
        trigger.SetActive(false);   // Disable the ability to interact with the gate; it's open, we're done

        leftGate.transform.RotateAround(leftGatePivot.transform.position, Vector3.up, -85);
        rightGate.transform.RotateAround(rightGatePivot.transform.position, Vector3.up, 85);
        yield return null;

        validInteraction = false;
    }
}
