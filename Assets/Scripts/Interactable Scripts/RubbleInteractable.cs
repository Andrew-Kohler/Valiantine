using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleInteractable : Interactable
{
    

    // Start is called before the first frame update
    void Start()
    {
        lines.Add("Rubble from collapsed ruins blocks the way. It looks like something stronger than you could shake it loose.");
    }

    // Update is called once per frame
    void Update()
    {
        
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
