using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// TutorialStepEditor
/// </summary>
[CustomEditor(typeof(MSTutorialManager))]
public class TutorialEditor : Editor 
{
	bool uiEnabled = false;

	Dictionary<MSTutorial, bool> folds = new Dictionary<MSTutorial, bool>();

	public override void OnInspectorGUI ()
	{
		MSTutorialManager tutMan = target as MSTutorialManager;

		if (tutMan.currentTutorial != null)
		{
			EditorGUILayout.LabelField("In tutorial!");
			EditorGUILayout.LabelField(tutMan.currentTutorial.name);
		}

		uiEnabled = EditorGUILayout.Foldout(uiEnabled, "UI");

		if (uiEnabled)
		{
			tutMan.TutorialUI.rightDialogue = EditorGUILayout.ObjectField("Right Dialogue", tutMan.TutorialUI.rightDialogue, typeof(MSDialogueUI), true) as MSDialogueUI;
			tutMan.TutorialUI.leftDialogue = EditorGUILayout.ObjectField("Left Dialogue", tutMan.TutorialUI.leftDialogue, typeof(MSDialogueUI), true) as MSDialogueUI;
			tutMan.TutorialUI.puzzleDialogue = EditorGUILayout.ObjectField("Puzzle Mobster Clickbox", tutMan.TutorialUI.puzzleDialogue, typeof(MSDialogueUI),true) as MSDialogueUI;
		}

		EditTutorial(tutMan.tutorialData.basicPuzzleTutorial);
		EditTutorial(tutMan.tutorialData.powerupPuzzleTutorial);
		EditTutorial(tutMan.tutorialData.rainbowPuzzleTutorial);
		EditTutorial(tutMan.tutorialData.combinePuzzleTutorial);
		EditTutorial(tutMan.tutorialData.collectPuzzleTutorial);
	}

	void EditTutorial(MSTutorial tutorial)
	{
		EditorGUILayout.Space();

		if (!folds.ContainsKey(tutorial))
		{
			folds[tutorial] = false;
		}

		folds[tutorial] = EditorGUILayout.Foldout(folds[tutorial], tutorial.name);

		if (folds[tutorial])
		{
			tutorial.name = EditorGUILayout.TextField("Name", tutorial.name);

			if (tutorial is MSPuzzleTutorial)
			{
				MSPuzzleTutorial puzTut = tutorial as MSPuzzleTutorial;

				puzTut.boardSize = EditorGUILayout.IntField("Board Size", puzTut.boardSize);

				puzTut.boardLayout = EditorGUILayout.TextField("Board File", puzTut.boardLayout);

				puzTut.rigVictoryOrLoss = EditorGUILayout.IntField("Rig Victory or Loss", puzTut.rigVictoryOrLoss);

				bool hasEndStep = EditorGUILayout.Toggle("End step", puzTut.endSteps != null);

				if (hasEndStep)
				{
					if (puzTut.endSteps == null)
					{
						puzTut.endSteps = new MSTutorialStep[0];
					}

					int numSteps = EditorGUILayout.IntField("Number of Steps", puzTut.endSteps.Length);
					
					if (numSteps > puzTut.endSteps.Length)
					{
						MSTutorialStep[] newSteps = new MSTutorialStep[numSteps];
						int i;
						for (i = 0; i < puzTut.endSteps.Length; i++) 
						{
							newSteps[i] = puzTut.endSteps[i];
						}
						for (; i < newSteps.Length; i++) 
						{
							newSteps[i] = new MSTutorialStep();
						}
						
						puzTut.endSteps = newSteps;
					}
					else if (numSteps < puzTut.endSteps.Length)
					{
						MSTutorialStep[] newSteps = new MSTutorialStep[numSteps];
						for (int i = 0; i < numSteps; i++) 
						{
							newSteps[i] = puzTut.endSteps[i];
						}
						
						puzTut.endSteps = newSteps;
					}
					
					for (int i = 0; i < puzTut.endSteps.Length; i++) 
					{
						EditStep(puzTut.endSteps[i], i);
					}
				}
				else
				{
					puzTut.endSteps = null;
				}
			}

			if (tutorial.steps == null)
			{
				tutorial.steps = new MSTutorialStep[0];
			}
			int stepCount = EditorGUILayout.IntField("Number of Steps", tutorial.steps.Length);
			
			if (stepCount > tutorial.steps.Length)
			{
				MSTutorialStep[] newSteps = new MSTutorialStep[stepCount];
				int i;
				for (i = 0; i < tutorial.steps.Length; i++) 
				{
					newSteps[i] = tutorial.steps[i];
				}
				for (; i < newSteps.Length; i++) 
				{
					newSteps[i] = new MSTutorialStep();
				}
				
				tutorial.steps = newSteps;
			}
			else if (stepCount < tutorial.steps.Length)
			{
				MSTutorialStep[] newSteps = new MSTutorialStep[stepCount];
				for (int i = 0; i < stepCount; i++) 
				{
					newSteps[i] = tutorial.steps[i];
				}
				
				tutorial.steps = newSteps;
			}
			
			for (int i = 0; i < tutorial.steps.Length; i++) 
			{
				EditStep(tutorial.steps[i], i);
			}
		}
	}

	void EditStep(MSTutorialStep step, string label)
	{
		EditorGUILayout.LabelField(label);
		
		step.stepType = (StepType)EditorGUILayout.EnumPopup("Type", step.stepType);
		
		int num;
		
		switch (step.stepType) 
		{
		case StepType.DIALOGUE:
			step.needsClick = EditorGUILayout.Toggle ("Needs Click", step.needsClick);
			step.dialogueType = (DialogueType)EditorGUILayout.EnumPopup("Dialogue Type", step.dialogueType);
			
			step.dialogue = EditorGUILayout.TextField(step.dialogue);
			
			break;
		case StepType.PUZZLE_BLOCK:
			num = EditorGUILayout.IntField("Spaces", step.spaces.Count);
			while (num > step.spaces.Count)
			{
				step.spaces.Add(Vector2.zero);
			}
			while (num < step.spaces.Count)
			{
				step.spaces.RemoveAt(step.spaces.Count-1);
			}
			
			for (int i = 0; i < step.spaces.Count; i++) 
			{
				step.spaces[i] = EditorGUILayout.Vector2Field("Space:", step.spaces[i]);
			}
			
			break;
		case StepType.UI:
			step.ui = EditorGUILayout.ObjectField("Object", step.ui, typeof(GameObject), true) as GameObject;
			break;
		case StepType.MOVE_CAMERA:
			step.position = EditorGUILayout.Vector3Field("Cam Pos", step.position);
			break;
		case StepType.MOVE_MOBSTERS:
			int numUnits = EditorGUILayout.IntField("Units", step.paths.Count);
			while (step.paths.Count < numUnits) 
			{
				step.paths.Add(new UnitPath());
			}
			while (step.paths.Count > numUnits)
			{
				step.paths.RemoveAt(step.paths.Count-1);
			}

			foreach (UnitPath path in step.paths) 
			{
				path.unit = EditorGUILayout.ObjectField("Unit", path.unit, typeof(MSCityUnit), true) as MSCityUnit;
				int spaces = EditorGUILayout.IntField("Spaces", path.path.Count);
				while (path.path.Count < spaces)
				{
					path.path.Add(new Vector2());
				}
				while (path.path.Count > spaces)
				{
					path.path.RemoveAt(path.path.Count-1);
				}
				for (int i = 0; i < path.path.Count; i++) 
				{
					path.path[i] = EditorGUILayout.Vector2Field("Space " + i, path.path[i]);
				}
			}

			break;
		default:
			break;
		}
	}

	void EditSteps(ref MSTutorialStep[] steps)
	{

	}

	void EditStep(MSTutorialStep step, int index)
	{
		EditStep(step, "Step " + (index + 1));
	}

}
