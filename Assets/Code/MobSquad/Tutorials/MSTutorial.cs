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

	public GameObject currUI
	{
		set
		{
			MSTutorialManager.instance.currUi = value;
		}
	}

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
			Debug.Log("Not clicked");
			while (!clicked)
			{
				yield return null;
			}
			Debug.Log("Clicked");
			currUI = null;
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
			if (step.player)
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
		case StepType.CLEAR_UNITS:
			foreach (var item in MSBuildingManager.instance.playerUnits) 
			{
				item.Value.Pool();
			}
			MSBuildingManager.instance.playerUnits.Clear();
			break;
		case StepType.SPAWN_UNIT:
			MSBuildingManager.instance.MakeTutorialUnit(step.id, step.position, step.index);
			break;
		case StepType.MOVE_MOBSTERS:
			foreach (var path in step.paths) 
			{
				MSBuildingManager.instance.MoveTutorialUnit(path.index, path.path);
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
			//Debug.Break();
		}
	}

	[ContextMenu ("Click Dialogue")]
	public void OnClicked()
	{
		clicked = true;
	}
}
