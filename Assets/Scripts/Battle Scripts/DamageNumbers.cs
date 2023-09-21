using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumbers : MonoBehaviour
{
	[SerializeField] float duration;
	[SerializeField] TextMeshPro nums;
	[SerializeField] TextMeshPro backer;

	int xSign;

	Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		StartCoroutine(Duration());
	}

	public DamageNumbers(float duration, int numberValue, int xSign, bool crit)
	{
		this.duration = duration;
		nums.text = numberValue.ToString();
		backer.text = numberValue.ToString();
		this.xSign = xSign;
        if (crit)
        {
			nums.color = Color.red;
        }
		else
		{
			nums.color = Color.white;
		}
	}

	public void SetValues(float duration, int numberValue, int xSign, bool crit)
	{
		this.duration = duration;
		nums.text = numberValue.ToString();
		backer.text = numberValue.ToString();
		this.xSign = xSign;
		if (crit)
		{
			nums.color = Color.red;
		}
        else
        {
			nums.color = Color.white;
        }
	}

	IEnumerator Duration()
	{
		rb.AddForce(xSign * 3f, 5f, 0f, ForceMode.Impulse);
		yield return new WaitForSeconds(duration);
		Destroy(gameObject);
	}

}
