using UnityEngine;
using System.Collections;

public class MSLevelUpAnimation : MonoBehaviour {

	[SerializeField] UITweener[] topInTweens;
	[SerializeField] UITweener[] botInTweens;
	[SerializeField] UITweener[] outTweens;
	[SerializeField] UILabel bottomLabel;
	[SerializeField] float waitSeconds;

	IEnumerator currCorout;

	[ContextMenu ("Test Play")]
	public void Play()
	{
		gameObject.SetActive(true);
		if (currCorout != null) StopCoroutine(currCorout);
		currCorout = RunTweens();
		StartCoroutine(currCorout);
		bottomLabel.gameObject.SetActive(false);
	}

	IEnumerator RunTweens()
	{
		foreach (var item in botInTweens) 
		{
			item.Sample(0, true);
		}
		foreach (var item in topInTweens) 
		{
			item.Sample(0, true);
			item.PlayForward();
		}
		foreach (var item in topInTweens) 
		{
			while (item.tweenFactor < 1) yield return null;
		}
		foreach (var item in botInTweens) 
		{
			item.PlayForward();
		}
		foreach (var item in botInTweens) 
		{
			while (item.tweenFactor < 1) yield return null;
		}
		currCorout = WaitThenOut();
		StartCoroutine(currCorout);
	}

	IEnumerator WaitThenOut()
	{	
		yield return new WaitForSeconds(waitSeconds);
		foreach (var item in outTweens) 
		{
			item.ResetToBeginning();
			item.PlayForward();
		}
		currCorout = null;
		gameObject.SetActive(false);
	}

	public void Skip(int levels)
	{
		if (currCorout != null) StopCoroutine(currCorout);
		if (levels > 1)
		{
			bottomLabel.gameObject.SetActive (true);
			bottomLabel.text = "x" + levels;
		}
		foreach (var item in topInTweens) 
		{
			item.Sample(1, true);
		}
		foreach (var item in botInTweens) 
		{
			item.Sample(1, true);
		}
		currCorout = WaitThenOut();
		StartCoroutine(currCorout);
	}

}
