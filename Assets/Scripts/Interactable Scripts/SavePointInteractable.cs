using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointInteractable : Interactable
{
    private void Start()
    {
        lines.Add("The testing area, full of promise and half-made dreams, evokes an unforgettable sense of hope.");
        lines.Add("Save your game?");
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
