using UnityEngine;
using System.Collections;

public class PZSwapButton : MonoBehaviour {

	[SerializeField]
	PZDeployPopup deployPopup;

	[SerializeField]
	UITweener deployTween;

	[SerializeField]
	UITweener swapTween; 

	void OnEnable()
	{
		MSActionManager.Puzzle.OnGemMatch += Hide;
		MSActionManager.Puzzle.OnNewPlayerRound += Show;
		MSActionManager.Puzzle.ForceHideSwap += Hide;
		MSActionManager.Puzzle.ForceShowSwap += Show;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnGemMatch -= Hide;
		MSActionManager.Puzzle.OnNewPlayerRound -= Show;
	}

	void OnClick()
	{
		deployPopup.Init();
		deployTween.PlayForward();
		swapTween.PlayReverse();
	}

	public void Show()
	{
		if (PZCombatManager.instance.activePlayer.alive)
		{
			swapTween.PlayForward();
		}
	}

	void Hide()
	{
		if (swapTween.tweenFactor > .9f)
		{
			swapTween.PlayReverse();
		}
	}


}
