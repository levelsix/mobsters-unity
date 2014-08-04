using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// PuzzleTutorial
/// </summary>
[Serializable]
public class MSPuzzleTutorial : MSTutorial 
{
	bool newTurn = false;

	bool abort = false;

	public int rigVictoryOrLoss = 0;

	public int boardSize = 8;

	public string boardLayout;

	public MSTutorialStep[] endSteps;

	public MSTutorialStep dieStep;

	public override IEnumerator Run()
	{
		//MSTutorialManager.instance.currentTutorial = this;

		abort = false;
		
		PZPuzzleManager.instance.InitBoard(boardSize, boardSize, "Tutorial/" + boardLayout);

		MSActionManager.UI.OnDialogueClicked += OnClicked;

		//Attach to the turn start event
		MSActionManager.Puzzle.OnTurnChange += OnTurnStart;

		//Attach to the On City event (in case of retreat)
		MSActionManager.Scene.OnCity += OnCity;

		//Cycle through the steps
		if (steps == null)
		{
			steps = new MSTutorialStep[0];
		}
		foreach (var item in steps) 
		{
			yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));
		}

		if (endSteps != null)
		{
			MSTutorialManager.instance.holdUpEndingCombat = true;
			while(MSTutorialManager.instance.holdUpEndingCombat)
			{
				yield return null;
			}

			foreach (var item in endSteps) 
			{
				yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));
			}
		}
		
		MSActionManager.UI.OnDialogueClicked -= OnClicked;

		//Detach from the turn start event
		MSActionManager.Puzzle.OnTurnChange -= OnTurnStart;

		//Detach from the on city event
		MSActionManager.Scene.OnCity -= OnCity;

		//if (MSTutorialManager.instance.currentTutorial == this)
		//{
		//	MSTutorialManager.instance.currentTutorial = null;
		//
	}

	protected override IEnumerator RunStep (MSTutorialStep step)
	{
		switch (step.stepType) 
		{
		case StepType.DIALOGUE:
			yield return MSTutorialManager.instance.StartCoroutine(DoDialogue(step));
			break;
		case StepType.PUZZLE_BLOCK:
			PZPuzzleManager.instance.BlockBoard(step.spaces);
			break;
		case StepType.WAIT_FOR_DIALOGUE:
			MSTutorialManager.instance.TutorialUI.puzzleDialogue.clickbox.SetActive(true);
			clicked = false;
			while (!clicked && !abort)
			{
				yield return null;
			}
			MSTutorialManager.instance.TutorialUI.puzzleDialogue.clickbox.SetActive(false);
			break;
		case StepType.WAIT_FOR_TURN:
			newTurn = false;
			while (!newTurn && !abort)
			{
				yield return null;
			}
			break;
		case StepType.UI:
			currUI = step.ui;
			clicked = false;
			while (!clicked && !abort)
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
		default:
			break;
		}
	}

	void OnTurnStart(int turnsLeft)
	{
		Debug.Log("Turn start");
		newTurn = true;
	}

	void OnCity()
	{
		Debug.Log("Abort");
		abort = true;
		MSTutorialManager.instance.TutorialUI.puzzleDialogue.ForceOut();
	}
}
