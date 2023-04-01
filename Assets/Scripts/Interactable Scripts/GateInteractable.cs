using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateInteractable : Interactable
{
    [SerializeField] GameObject leftGate;
    [SerializeField] GameObject leftGatePivot;
    [SerializeField] GameObject rightGate;
    [SerializeField] GameObject rightGatePivot;
    [SerializeField] GameObject trigger;

    [SerializeField] ItemData key;

    [SerializeField] float gateOpenSpeed;

    bool validKey;
    bool validInteraction;

    private void OnEnable()
    {
        InGameUIView.onInteractionEnd += GateCheck;
    }

    private void OnDisable()
    {
        InGameUIView.onInteractionEnd -= GateCheck;
    }

    /*private void Start()
    {
        lines.Add("The gates seem as though they won't budge.");
        lines.Add("Perhaps if someone programmed in a rotation sequence cutscene, they would open.");
    }*/

    public override void Interact()
    {
        StartCoroutine(DoInteraction());
    }

    private void GateCheck()
    {
        if(validInteraction && validKey)
        StartCoroutine(DoGateOpen()); // Add redundancy so that this only happens when the gate is actually checked
    }

    protected override IEnumerator DoInteraction()
    {
        validInteraction = true;
        usePrompt.GetComponent<UsePrompt>().FadeOut(); // Fades out the little indicator

        // Check for if the player has the item necessary to open the gate
        lines.Clear();
        if (PlayerManager.Instance.PlayerInventory().InventorySystem.ContainsItem(key))
        {
            lines.Add("The " + key.DisplayName + " fits right in the lock. You hear a click as you turn it.");
            validKey = true;
        }
        else
        {
            lines.Add("The gate is locked tight, even after all this time.");
            lines.Add("The lock looks like a " + key.DisplayName + " would fit right in.");
        }

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);

        yield return null;
    }

    private IEnumerator DoGateOpen()
    {
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
        
        yield return null;
        
    }
}
