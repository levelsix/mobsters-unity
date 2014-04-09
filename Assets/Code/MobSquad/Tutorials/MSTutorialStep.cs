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
	public string[] dialogues;

	public List<Vector2> spaces;

	public UIButton ui;

	public UnitPath[] paths;
}

[System.Serializable]
public class UnitPath
{
	public MSCityUnit unit;

	public Vector2[] path;
}