using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextRevealer : MonoBehaviour
{

	[SerializeField] TextMeshProUGUI text;

	[SerializeField] Image advanceArrow;
	[SerializeField] TextMeshProUGUI advanceE;
	bool skipToEnd;
	bool activeCoroutine;

	[SerializeField] private List<AudioClip> sounds;
	private AudioSource audioS;

    private void Start()
    {
        audioS = GetComponent<AudioSource>();
		
	}

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
		advanceArrow.color = new Color(.6f, .6f, .6f);
		advanceE.color = new Color(.6f, .6f, .6f);
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
				advanceArrow.color = new Color(1f, 1f, 1f);
				advanceE.color = new Color(1f, 1f, 1f);
				skipToEnd = false;
				text.text = originalString;
				break;

			}

			text.text = originalString.Substring(0, numCharsRevealed);
			audioS.PlayOneShot(sounds[0], GameManager.Instance.uiVolume * GameManager.Instance.masterVolume);

			if(text.text == originalString)
            {
				advanceArrow.color = new Color(1f, 1f, 1f);
				advanceE.color = new Color(1f, 1f, 1f);
			}
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
