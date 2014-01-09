using UnityEngine;
using System.Collections;

public class PZComboCounter : MonoBehaviour {

	[SerializeField]
	TweenAlpha alphaTween;

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
		if (combo == 0 && alphaTween.alpha > 0)
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
		alphaTween.PlayForward();
	}

	void FadeOut()
	{
		Debug.Log("Fade out");
		alphaTween.PlayReverse();
	}

}
