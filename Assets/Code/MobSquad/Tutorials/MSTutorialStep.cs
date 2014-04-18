using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// TutorialStep
/// </summary>
[System.Serializable]
public class MSTutorialStep 
{
	public StepType stepType;

	public DialogueType dialogueType;

	public string dialogue;

	public bool needsClick = false;

	public List<Vector2> spaces = new List<Vector2>();

	public bool snap = false;

	public GameObject ui;

	public UnitPath[] paths = new UnitPath[1];
}

public enum StepType {
	DIALOGUE, 
	PUZZLE_BLOCK,
	MOVE_MOBSTERS,
	UI, 
	WAIT_FOR_DIALOGUE, 
	WAIT_FOR_TURN,
	WAIT_FOR_MOBSTER_MOVE,
	CONTINUE,
	SPECIAL_MAKE_SPACE_ON_TEAM,
	MOVE_CAMERA,
	SPAWN_UNIT,
	WAIT_TIME
};

public enum DialogueType {LEFT, RIGHT, PUZZLE};

[System.Serializable]
public class UnitPath
{
	public MSCityUnit unit;

	public Vector2[] path;
}