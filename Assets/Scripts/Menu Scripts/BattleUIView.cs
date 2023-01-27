using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUIView : View
{
    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] GameObject actionIndicators;
    IndicatorMovement indicatorInfo;
    
    public override void Initialize()
    {
        //throw new System.NotImplementedException();
        battleText.text = "And so it begins!";
        indicatorInfo = actionIndicators.GetComponent<IndicatorMovement>();
    }

    private void Update()
    {
        // Seeing as there may be other uses for this loop in the future, I will refrain from flooding it with the code for 
        // what text should be displayed, and just make it a method instead.
        if (GameManager.Instance.isBattle())
        {
            updateText();
        }
        
    }

    void updateText()
    {
        string currentBoxName = indicatorInfo.GetLeadBox();
        if(currentBoxName == "ATK")
        {
            battleText.text = "Attack an enemy for 100% damage.";
        }
        else if(currentBoxName == "SPL")
        {
            battleText.text = "Cast your gem spell. Equipped Spell: None (I haven't coded that yet, silly)";
        }
        else if(currentBoxName == "ITM")
        {
            battleText.text = "Use a health or mana potion to restore your strength.";
        }
        else if(currentBoxName == "RUN")
        {
            battleText.text = "Attempt to flee the battle.";
        }
        else
        {
            battleText.text = "";
        }
    }

    
}
