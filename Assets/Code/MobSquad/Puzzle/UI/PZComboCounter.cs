using UnityEngine;
using System.Collections;

public class PZComboCounter : MonoBehaviour {

	[SerializeField]
	TweenAlpha alphaTween;

	[SerializeField]
	UILabel comboLabel;

	[SerializeField]
	ParticleSystem flames;

	int lastCombo = 0;

	const int COMBO_FIRE = 5;

	void OnEnable()
	{
		MSActionManager.Puzzle.OnComboChange += OnComboChange;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnComboChange -= OnComboChange;
	}

	void OnComboChange(int combo)
	{
		if (combo == 0 && alphaTween.value > 0)
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

			if(combo == COMBO_FIRE)
			{
				MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.comboFire);
				flames.Play();
			}
		}

		lastCombo = combo;
	}

	void FadeIn()
	{
		//Debug.Log("Fade In");
		alphaTween.PlayForward();
	}

	void FadeOut()
	{
		//Debug.Log("Fade out");
		alphaTween.PlayReverse();
		flames.Stop();
	}

}
