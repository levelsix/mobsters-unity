using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Tutorial
/// </summary>
[Serializable]
public class MSTutorial {

	public string name;

	public MSTutorialStep[] steps;

	protected bool clicked = false;

	public GameObject currUI;

	protected virtual IEnumerator RunStep(MSTutorialStep step)
	{
		switch (step.stepType) 
		{
		case StepType.DIALOGUE:
			MSDialogueUI dialoguer;
			switch (step.dialogueType) {
			case DialogueType.PUZZLE:
			default:
				dialoguer = MSTutorialManager.instance.TutorialUI.puzzleDialogue;
				break;
			}
			yield return dialoguer.StartCoroutine(dialoguer.BringInMobster(
				PZCombatManager.instance.activePlayer.monster.monster.imagePrefix,
				PZCombatManager.instance.activePlayer.monster.monster.displayName,
				step.dialogue));
			if (step.needsClick)
			{
				MSTutorialManager.instance.TutorialUI.puzzleDialogue.clickbox.SetActive(true);
				clicked = false;
				while (!clicked)
				{
					yield return null;
				}
				MSTutorialManager.instance.TutorialUI.puzzleDialogue.clickbox.SetActive(false);
			}
			break;
		case StepType.WAIT_FOR_DIALOGUE:
			MSTutorialManager.instance.TutorialUI.puzzleDialogue.clickbox.SetActive(true);
			clicked = false;
			while (!clicked)
			{
				yield return null;
			}
			MSTutorialManager.instance.TutorialUI.puzzleDialogue.clickbox.SetActive(false);
			break;
		case StepType.UI:
			currUI = step.ui;
			clicked = false;
			while (!clicked)
			{
				yield return null;
			}
			break;
		case StepType.CONTINUE:
			if (MSActionManager.Tutorial.OnTutorialContinue != null)
			{
				MSActionManager.Tutorial.OnTutorialContinue();
			}
			break;
		case StepType.MOVE_CAMERA:
			yield return MSTutorialManager.instance.StartCoroutine(MSTownCamera.instance.SlideToCameraPosition(step.position, step.time));
			break;
		case StepType.GO_TO_CITY:
			if (step.home)
			{
				MSWhiteboard.currCityType = MSWhiteboard.CityType.PLAYER;
				MSWhiteboard.cityID = MSWhiteboard.localMup.userId;
				yield return MSTutorialManager.instance.StartCoroutine(MSBuildingManager.instance.LoadPlayerCity());
			}
			else
			{
				MSWhiteboard.currCityType = MSWhiteboard.CityType.NEUTRAL;
				MSWhiteboard.cityID = step.id;
				yield return MSTutorialManager.instance.StartCoroutine(MSBuildingManager.instance.LoadNeutralCity(step.id));
			}
			break;
		default:
			break;
		}
	}

	public virtual IEnumerator Run()
	{
		foreach (var item in steps) 
		{
			yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));
		}
	}

	[ContextMenu ("Click Dialogue")]
	public void OnClicked()
	{
		clicked = true;
	}
}
