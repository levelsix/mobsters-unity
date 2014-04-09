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

	[SerializeField] MSTutorialStep onEndStep;

	[SerializeField] MSTutorialStep onDieStep;

	public override IEnumerator Run()
	{
		abort = false;

		//Attach to the turn start event
		MSActionManager.Puzzle.OnTurnChange += OnTurnStart;

		//Attach to the On City event (in case of retreat)
		MSActionManager.Scene.OnCity += OnCity;

		//Cycle through the steps
		foreach (var item in steps) 
		{
			newTurn = false;

			//Wait for the next turn
			while(!newTurn && !abort)
			{
				yield return null;
			}

			yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));

		}

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
			//Bring in the dialogue
			
			//Cycle through dialogue
			foreach (var item in step.dialogues) 
			{
				clicked = false;
				while (!clicked & !abort)
				{
					yield return null;
				}
			}

			//Slide out the dialogue
		}

		yield return null;

	}

	[ContextMenu ("Next Turn")]
	void OnTurnStart(int turnsLeft)
	{
		newTurn = true;
	}

	[ContextMenu ("Abort")]
	void OnCity()
	{
		abort = true;
	}
}
