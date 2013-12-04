using UnityEngine;
using System.Collections;

public class PZComboCounter : MonoBehaviour {

	[SerializeField]
	TweenAlpha[] alphaTweens;

	[SerializeField]
	UILabel comboLabel;

	int lastCombo = 0;

	void OnEnable()
	{
		CBKEventManager.Puzzle.OnComboChange += OnComboChange;
	}

	void OnDisable()
	{
		CBKEventManager.Puzzle.OnComboChange -= OnComboChange;
	}

	void OnComboChange(int combo)
	{
		Debug.Log("Combo: " + combo + ", Last combo: " + lastCombo);
		if (combo == 0 && comboLabel.alpha > 0)
		{
			FadeOut();
		}
		else if (combo > 1)
		{
			comboLabel.text = "x" + combo;
			if (lastCombo <= 1)
			{
				FadeIn();
			}
		}

		lastCombo = combo;
	}

	void FadeIn()
	{
		Debug.Log("Fade In");
		foreach (var item in alphaTweens) 
		{
			item.Reset();
			item.Play();
		}
	}

	void FadeOut()
	{
		Debug.Log("Fade out");
		foreach (var item in alphaTweens) 
		{
			item.Toggle();
			//item.Play();
		}
	}

}
