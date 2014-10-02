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
	public UITweener gradientTween;

	public void OnEnable()
	{
		MSActionManager.UI.OnDialogueClicked += DoPushOut;
		MSActionManager.Puzzle.OnGemSwapSuccess += DoPushOut;
	}

	public void OnDisable()
	{
		MSActionManager.UI.OnDialogueClicked -= DoPushOut;
		MSActionManager.Puzzle.OnGemSwapSuccess -= DoPushOut;
	}

	/// <summary>
	/// Wrapper around the coroutine, so that we can make sure that the
	/// coroutine is running on this component locally.
	/// </summary>
	/// <returns>The dialogue.</returns>
	/// <param name="mobsterImgName">Mobster image name.</param>
	/// <param name="mobsterName">Mobster name.</param>
	/// <param name="dialogue">Dialogue.</param>
	public Coroutine RunDialogue(string bundleName, string mobsterImgName, string mobsterName, string dialogue, bool hitbox = true)
    {
		return StartCoroutine(BringInMobster(bundleName, mobsterImgName, mobsterName, dialogue, hitbox));
	}

	public void DoDialogue(string bundleName, string mobsterImgName, string mobsterName, string dialogue, bool hitbox = true)
	{
		StartCoroutine(BringInMobster(bundleName, mobsterImgName, mobsterName, dialogue, hitbox));
	}

	IEnumerator BringInMobster(string bundleName, string mobsterImgName, string mobsterName, string dialogue, bool hitbox)
	{
		MSSpriteUtil.instance.SetSprite(bundleName, mobsterImgName, mobster);

		mobsterTween.PlayForward();

		if (hitbox)
		{
			gradientTween.PlayForward();
		}

		while (mobsterTween.tweenFactor < 1)
		{
			yield return null;
		}

		yield return StartCoroutine(BringInDialogue(mobsterName, dialogue));

		clickbox.SetActive(hitbox);
	}

	IEnumerator BringInDialogue(string mobsterName, string dialogue)
	{
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.dialogueBox);
		dialogueLabel.text = dialogue;
		mobsterNameLabel.text = mobsterName;

		dialogueBoxTween.Sample(0, true);
		dialogueBoxTween.PlayForward();

		while (dialogueBoxTween.tweenFactor < 1)
		{
			yield return null;
		}
	}

	public void DoPushOut()
	{
		StartCoroutine(PushOut());
	}

	public Coroutine RunPushOut()
	{
		return StartCoroutine(PushOut());
	}

	IEnumerator PushOut()
	{
		dialogueBoxTween.PlayReverse();
		while (dialogueBoxTween.tweenFactor > 0)
		{
			yield return null;
		}
		gradientTween.PlayReverse();
		mobsterTween.PlayReverse();
		while (mobsterTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	public Coroutine RunDialogueOut()
	{
		return StartCoroutine(DialogueOut());
	}

	IEnumerator DialogueOut()
	{
		dialogueBoxTween.PlayReverse();
		while (dialogueBoxTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	public void ForceOut()
	{
		gradientTween.Sample(0, true);
		dialogueBoxTween.Sample(0, true);
		mobsterTween.Sample(0, true);
	}

}
