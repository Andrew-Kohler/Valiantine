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
		StartCoroutine(Fade());
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

	private IEnumerator Duration()
	{
		rb.AddForce(xSign * 5f, 8f, 0f, ForceMode.Impulse);
		yield return new WaitForSeconds(duration);
		Destroy(gameObject);
	}

	private IEnumerator Fade()
    {
		while(nums.color.a > 0)
        {
			nums.color = new Color(nums.color.r, nums.color.g, nums.color.b, nums.color.a - .5f * Time.deltaTime);
			backer.color = new Color(backer.color.r, backer.color.g, backer.color.b, backer.color.a - .5f * Time.deltaTime);
			yield return null;
		}
    }

}
