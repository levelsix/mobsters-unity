using UnityEngine;
using System.Collections;

public class CBKTweenOnEnable : MonoBehaviour {

	[SerializeField]
	UITweener[] tweens;

	void OnEnable()
	{
		foreach (var item in tweens) 
		{
			item.PlayForward();
		}
	}

	public void StartDisable()
	{
		foreach (var item in tweens) 
		{
			item.PlayReverse();
		}
	}

	public void FinishDisable()
	{
		gameObject.SetActive(true);
	}
}
