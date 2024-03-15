using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsongInteractable : Interactable
{
    // Start is called before the first frame update
    [SerializeField] private CameraControl camController;   
    void Start()
    {
        lines.Add("The Heartsong Amulet glitters in the sunlight.");
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
        yield return new WaitUntil(() => !GameManager.Instance.isInteraction());

        //Immobilize the player again
        GameManager.Instance.Cutscene(true);

        // Push volume down to 0


        // Pull the camera up and fade to black
        camController.SetCamEndingCutscene();
        yield return new WaitForSeconds(1f);
        GameObject transitionPanel = GameObject.Find("Black Panel");
        FadeScene fade = transitionPanel.GetComponent<FadeScene>();
        fade.NoLoadSceneFadeIn();
        yield return new WaitForSeconds(3f);
        // Tell IG UI to do the closing convo
        ViewManager.GetView<InGameUIView>().playClosingConvo();
        yield return new WaitForSeconds(27f);

        // Fade to black and ROLL CREDITS!
        //GameObject transitionPanel2 = GameObject.Find("Black Panel 2");
        //FadeScene fad2e = transitionPanel.GetComponent<FadeScene>();
        fade.SceneFadeIn("24_Credits");

        yield return null;
    }
}
