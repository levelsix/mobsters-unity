using UnityEngine;
using System.Collections;

public class PZSwapButton : MonoBehaviour {

	[SerializeField]
	UITweener deployTween;

	[SerializeField]
	UITweener swapTween; 

	void OnEnable()
	{
		MSActionManager.Puzzle.OnGemMatch += Hide;
		MSActionManager.Puzzle.OnNewPlayerTurn += Show;
		MSActionManager.Puzzle.ForceHideSwap += Hide;
		MSActionManager.Puzzle.ForceShowSwap += Show;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnGemMatch -= Hide;
		MSActionManager.Puzzle.OnNewPlayerTurn -= Show;
	}

	void OnClick()
	{
		deployTween.PlayForward();
		swapTween.PlayReverse();
	}

	void Show()
	{
		swapTween.PlayForward();
	}

	void Hide()
	{
		if (swapTween.tweenFactor > .9f)
		{
			swapTween.PlayReverse();
		}
	}


}
