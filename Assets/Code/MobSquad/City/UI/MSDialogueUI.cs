using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSDialogueUI
/// </summary>
public class MSDialogueUI : MonoBehaviour {

	public GameObject clickbox;
	public UI2DSprite mobster;
	public UITweener mobsterTween;
	public UISprite dialogueBox;
	public UITweener dialogueBoxTween;
	public UILabel dialogueLabel;
	public UILabel mobsterNameLabel;

	public IEnumerator BringInMobster(string mobsterImgName, string mobsterName, string dialogue)
	{
		MSSpriteUtil.instance.SetSprite(mobsterImgName, mobsterImgName + "Character", mobster);

		mobsterTween.PlayForward();

		MSActionManager.Puzzle.OnGemPressed += PushOut;

		while (mobsterTween.tweenFactor < 1)
		{
			yield return null;
		}

		yield return StartCoroutine(BringInDialogue(mobsterName, dialogue));
	}

	IEnumerator BringInDialogue(string mobsterName, string dialogue)
	{
		dialogueLabel.text = dialogue;
		mobsterNameLabel.text = mobsterName;

		dialogueBoxTween.Sample(0, false);
		dialogueBoxTween.PlayForward();

		while (dialogueBoxTween.tweenFactor < 1)
		{
			yield return null;
		}
	}

	public void PushOut()
	{
		MSActionManager.Puzzle.OnGemPressed -= PushOut;

		dialogueBoxTween.PlayReverse();
		mobsterTween.PlayReverse();
	}

	public void ForceOut()
	{
		dialogueBoxTween.Sample(0, true);
		mobsterTween.Sample(0, true);
	}

}
