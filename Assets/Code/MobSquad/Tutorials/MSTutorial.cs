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
			yield return MSTutorialManager.instance.StartCoroutine(DoDialogue(step));
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
			yield return MSTutorialManager.instance.StartCoroutine(DoUIStep(step.ui, step.distance, step.direction));
			break;
		case StepType.CONTINUE:
			if (MSActionManager.Tutorial.OnTutorialContinue != null)
			{
				MSActionManager.Tutorial.OnTutorialContinue();
			}
			break;
		case StepType.MOVE_CAMERA:
			yield return MSTutorialManager.instance.StartCoroutine(DoCameraMove(step.position, step.size, step.time));
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
		case StepType.WAIT_FOR_MOBSTER_MOVE:
			foreach (var item in MSBuildingManager.instance.playerUnits) {
				while (item.Value.cityUnit.moving)
				{
					yield return null;
				}
			}
			break;
		default:
			break;
		}
	}

	public virtual IEnumerator Run()
	{
		MSTutorialManager.instance.currentTutorial = this;

		MSActionManager.UI.OnDialogueClicked += OnClicked;

		foreach (var item in steps) 
		{
			yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));
			//Debug.Break();
		}

		MSActionManager.UI.OnDialogueClicked -= OnClicked;

		MSTutorialManager.instance.currentTutorial = null;
		
		foreach (var item in MSBuildingManager.instance.playerUnits) 
		{
			item.Value.cityUnit.MoveNext();
			item.Value.cityUnit.moving = true;
		}
	}

	protected virtual IEnumerator DoDialogue(MSTutorialStep step)
	{
		MSDialogueUI dialoguer;
		MonsterProto monster;
		switch (step.dialogueType) {
		case DialogueType.RIGHT:
			dialoguer = MSTutorialManager.instance.TutorialUI.rightDialogue;
			break;
		case DialogueType.LEFT:
			dialoguer = MSTutorialManager.instance.TutorialUI.leftDialogue;
			break;
		case DialogueType.PUZZLE:
		default:
			dialoguer = MSTutorialManager.instance.TutorialUI.puzzleDialogue;
			break;
		}
		
		switch(step.dialogueType)
		{
		case DialogueType.RIGHT:
		case DialogueType.LEFT:
			if (step.player)
			{
				monster = MSBuildingManager.instance.playerUnits[step.index].monster;
			}
			else
			{
				monster = MSDataManager.instance.Get<MonsterProto>(step.index);
			}
			break;
		case DialogueType.PUZZLE:
		default:
			if (step.player)
			{
				monster = PZCombatManager.instance.activePlayer.monster.monster;
			}
			else
			{
				monster = PZCombatManager.instance.activeEnemy.monster.monster;
			}
			break;
		}
		
		yield return dialoguer.StartCoroutine(dialoguer.BringInMobster(
			monster.imagePrefix,
			monster.displayName,
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
	}
		
	protected virtual IEnumerator DoCameraMove(Vector3 position, float size, float time)
	{
		yield return MSTutorialManager.instance.StartCoroutine(MSTownCamera.instance.SlideToCameraPosition(position, size, time));
	}

	protected virtual IEnumerator DoUIStep(GameObject ui, float distance, MSValues.Direction direction)
	{
		currUI = ui;
		MSTutorialArrow.instance.Init(ui.transform, distance, direction);
		clicked = false;
		Debug.Log("Not clicked");
		while (!clicked)
		{
			yield return null;
		}
		Debug.Log("Clicked");
		currUI = null;
		MSTutorialArrow.instance.gameObject.SetActive(false);
	}
	
	[ContextMenu ("Click Dialogue")]
	public void OnClicked()
	{
		clicked = true;
	}
}
