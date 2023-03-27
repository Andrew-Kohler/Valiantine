using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextRevealer : MonoBehaviour
{

	[SerializeField] TextMeshProUGUI text;

	public void ReadOutText(string textToRead)
	{
		StartCoroutine(RevealText(textToRead));
	}

	IEnumerator RevealText(string textToReveal)
	{
		var originalString = textToReveal;
		text.text = "";

		var numCharsRevealed = 0;
		while (numCharsRevealed < originalString.Length)
		{
			while (originalString[numCharsRevealed] == ' ')
				++numCharsRevealed;

			++numCharsRevealed;

			text.text = originalString.Substring(0, numCharsRevealed);

			yield return new WaitForSeconds(0.07f);
		}
		ViewManager.GetView<InGameUIView>().textReadout = true;
	}
}
