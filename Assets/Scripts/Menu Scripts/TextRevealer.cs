using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextRevealer : MonoBehaviour
{

	[SerializeField] TextMeshProUGUI text;
	bool skipToEnd;
	bool activeCoroutine;

	private void OnEnable()
	{
		skipToEnd = false;
		activeCoroutine = false;
		PlayerMovement.onInteractButton += AdvanceText;
	}

	private void OnDisable()
	{
		PlayerMovement.onInteractButton -= AdvanceText;
	}

	public void ReadOutText(string textToRead)
	{
		StartCoroutine(RevealText(textToRead));
	}

	IEnumerator RevealText(string textToReveal)
	{
		activeCoroutine = true;
		var originalString = textToReveal;
		text.text = "";

		var numCharsRevealed = 0;
		while (numCharsRevealed < originalString.Length)
		{
			while (originalString[numCharsRevealed] == ' ')
				++numCharsRevealed;

			++numCharsRevealed;

            if (skipToEnd)
            {
				skipToEnd = false;
				text.text = originalString;
				break;

			}

			text.text = originalString.Substring(0, numCharsRevealed);

			yield return new WaitForSeconds(0.07f);
		}
		ViewManager.GetView<InGameUIView>().textReadout = true;
		activeCoroutine = false;
	}

	private void AdvanceText()
    {
        if (activeCoroutine)
        {
			skipToEnd = true;
		}
		
    }
}
