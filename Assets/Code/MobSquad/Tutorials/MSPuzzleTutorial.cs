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

	[SerializeField] int rigVictoryOrLoss = 0;

	[SerializeField] int boardSize = 8;

	[SerializeField] string boardLayout;

	[SerializeField] MSTutorialStep onEndStep;

	[SerializeField] MSTutorialStep onDieStep;

	public override IEnumerator Run()
	{
		abort = false;
		
		PZPuzzleManager.instance.InitBoard(boardSize, boardSize, "Tutorial/" + boardLayout);

		MSActionManager.UI.OnDialogueClicked += OnDialogueClicked;

		//Attach to the turn start event
		MSActionManager.Puzzle.OnTurnChange += OnTurnStart;

		//Attach to the On City event (in case of retreat)
		MSActionManager.Scene.OnCity += OnCity;

		//Cycle through the steps
		foreach (var item in steps) 
		{
			newTurn = false;

			Debug.Log("Waiting for new turn...");

			//Wait for the next turn
			while(!newTurn && !abort)
			{
				yield return null;
			}

			yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));

		}
		
		MSActionManager.UI.OnDialogueClicked -= OnDialogueClicked;

		//Detach from the turn start event
		MSActionManager.Puzzle.OnTurnChange -= OnTurnStart;

		//Detach from the on city event
		MSActionManager.Scene.OnCity -= OnCity;
	}

	protected override IEnumerator RunStep (MSTutorialStep step)
	{
		//Block spaces
		if (step.spaces.Count > 0)
		{
			PZPuzzleManager.instance.BlockBoard(step.spaces);
		}

		if (step.dialogues.Length > 0)
		{
			MSTutorialManager.instance.TutorialUI.puzzleMobsterClickbox.SetActive(true);

			MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueBoxTween.Sample(0, false);
			MSTutorialManager.instance.TutorialUI.puzzleMobsterTween.Sample(0, false);

			//Bring in the dialogue
			MSTutorialManager.instance.TutorialUI.puzzleMobsterTween.PlayForward();

			MSTutorialManager.instance.TutorialUI.puzzleMonsterNameLabel.text = PZCombatManager.instance.activePlayer.monster.monster.displayName;

			while (MSTutorialManager.instance.TutorialUI.puzzleMobsterTween.tweenFactor < 1)
			{
				yield return null;
			}
			
			//Cycle through dialogue
			foreach (var item in step.dialogues) 
			{
				//Bring in Dialogue box
				MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueBoxTween.PlayForward();

				MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueLabel.text = item;

				while (MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueBoxTween.tweenFactor < 1)
				{
					yield return null;
				}

				clicked = false;
				Debug.LogWarning("Dialogue: " + item);
				while (!clicked && !abort)
				{
					yield return null;
				}

				//Take out Dialogue box
				MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueBoxTween.PlayReverse();
				
				while (MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueBoxTween.tweenFactor > 0)
				{
					yield return null;
				}
			}

			MSTutorialManager.instance.TutorialUI.puzzleMobsterTween.PlayReverse();

			while (MSTutorialManager.instance.TutorialUI.puzzleMobsterTween.tweenFactor > 0)
			{
				yield return null;
			}

			if (step.ui != null)
			{

			}

			//Slide out the dialogue
			Debug.LogWarning("Dialogue Complete");
			
			MSTutorialManager.instance.TutorialUI.puzzleMobsterClickbox.SetActive(false);

		}

		yield return null;

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
		MSTutorialManager.instance.TutorialUI.puzzleMobsterTween.Sample(0, true);
		MSTutorialManager.instance.TutorialUI.puzzleMobsterDialogueBoxTween.Sample(0, true);
		MSTutorialManager.instance.TutorialUI.puzzleMobsterClickbox.SetActive(false);
	}
}
