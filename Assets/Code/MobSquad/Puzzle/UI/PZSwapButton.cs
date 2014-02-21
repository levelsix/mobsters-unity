using UnityEngine;
using System.Collections;

public class PZSwapButton : MonoBehaviour {

	[SerializeField]
	UITweener deployTween;

	[SerializeField]
	UITweener swapTween; 

	void OnEnable()
	{
		CBKEventManager.Puzzle.OnGemMatch += Hide;
		CBKEventManager.Puzzle.OnNewPlayerTurn += Show;
		CBKEventManager.Puzzle.ForceHideSwap += Hide;
		CBKEventManager.Puzzle.ForceShowSwap += Show;
	}

	void OnDisable()
	{
		CBKEventManager.Puzzle.OnGemMatch -= Hide;
		CBKEventManager.Puzzle.OnNewPlayerTurn -= Show;
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
