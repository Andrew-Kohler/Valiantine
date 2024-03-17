using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointInteractable : Interactable
{
    public int pointNumber;
    private string currentScene;
    private void Start()
    {
        if(pointNumber == 0)
            lines.Add("The testing area, full of promise and half-made dreams, evokes an unforgettable sense of hope.");
        else if(pointNumber == 1)
        {
            lines.Add("The fountain, long since dry, evokes an unforgettable sense of melancholy.");
            currentScene = "2_Fountain";
        }  
        else if (pointNumber == 2)
        {
            lines.Add("The garden of blossoming trees evokes an unforgettable sense of calm.");
            currentScene = "6_Garden";
        }
        else if (pointNumber == 3)
        {
            lines.Add("The crumbling tower clings to its perch below the windswept sky, evoking an unforgettable sense of wistfulness.");
            currentScene = "11_TowerTerrace";
        }
        else if (pointNumber == 4)
        {
            lines.Add("The sun warms your skin. You feel an unforgettable sense of hope.");
            currentScene = "18_BeforeBoss";
        }
        else if (pointNumber == 5)
        {
            lines.Add("The crumbled tower has kept its last watch. Its wreckage still evokes an unforgettable sense of wistfulness.");
            currentScene = "11_TowerTerrace";
        }
           
        lines.Add("Your game has been saved.");
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
        GameManager.Instance.WriteSaveData(currentScene, 0);
        yield return null;
    }
}
