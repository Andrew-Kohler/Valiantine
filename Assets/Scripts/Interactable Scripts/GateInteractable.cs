using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateInteractable : Interactable
{
    private void Start()
    {
        lines.Add("The gates seem as though they won't budge.");
        lines.Add("Perhaps if someone programmed in a rotation sequence cutscene, they would open.");
    }
    public override void Interact()
    {
        StartCoroutine(DoInteraction());
    }

    protected override IEnumerator DoInteraction()
    {
        usePrompt.GetComponent<UsePrompt>().FadeOut(); // Fades out the little indicator

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);
        yield return null;
    }
}
