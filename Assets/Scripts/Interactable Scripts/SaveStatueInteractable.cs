using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStatueInteractable : Interactable
{
    
    private void Start()
    {
        lines.Add("The statue's inscription reads, 'Here stands a guardian of Aleyssia. May her gaze ever meet the eyes of the valiant.'");
        lines.Add("You feel rejuvinated. [All HP and MP restored]");
    }
    public override void Interact()
    {
        StartCoroutine(DoInteraction());
    }

    protected override IEnumerator DoInteraction()
    {
        usePrompt.GetComponent<UsePrompt>().FadeOut(); // Fades out the little indicator

        PlayerManager.Instance.PlayerStats().SetHP(999); // Heal the player
        PlayerManager.Instance.PlayerStats().SetMP(999);

        // Start the text readout
        ViewManager.GetView<InGameUIView>().startInteractionText(lines);
        yield return null;
    }
}

