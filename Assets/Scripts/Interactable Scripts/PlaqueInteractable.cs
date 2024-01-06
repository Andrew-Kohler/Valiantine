using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaqueInteractable : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        lines.Add("'Here stands Castle Aleyssia, entry to the vast and powerful kingdom named the same. May any traveler who looks upon this plaque know its might.'");
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
