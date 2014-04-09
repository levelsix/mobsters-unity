using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSTutorialManager
/// </summary>
public class MSTutorialManager : MonoBehaviour 
{
	public static MSTutorialManager instance;

	[SerializeField] TutorialData tutorialData;

	[SerializeField] TutorialUI TutorialUI;

	#region Task Tutorials
	Dictionary<int, Action> tutorialTasks = new Dictionary<int, Action>()
	{
		{2, SetupTutorial1},
		{3, SetupTutorial2}
	};

	void Awake()
	{
		instance = this;
	}

	public void StartTutorial(int taskId)
	{
		Debug.Log("Doing a tutorial...");
		tutorialTasks[taskId]();
	}

	public bool IsTaskTutorial(int taskId)
	{
		return tutorialTasks.ContainsKey(taskId);
	}

	static void SetupTutorial1()
	{
		Debug.Log("Starting Tutorial 1");
		PZPuzzleManager.instance.InitBoard(6,6, "Tutorial/TutorialBattle1Layout.txt");
		instance.StartCoroutine(instance.tutorialData.basicPuzzleTutorial.Run());
	}

	static void SetupTutorial2()
	{
		Debug.Log("Starting Tutorial 2");
		PZPuzzleManager.instance.InitBoard(6,6);
	}
	#endregion
}

[Serializable]
public class TutorialData
{
	public MSPuzzleTutorial basicPuzzleTutorial;
}

[Serializable]
public class TutorialUI
{
	public float mobsterDistance = 200;

	public UIWidget dialogueClickbox;

	public UISprite arrow;

	public UI2DSprite leftMobster;
	public UISprite leftMobsterDialogueBox;
	public UILabel leftMobsterDialogueLabel;
	
	public UI2DSprite rightMobster;
	public UISprite rightMobsterDialogueBox;
	public UILabel rightMobsterDialogueLabel;
	
	public UI2DSprite puzzleMobster;
	public UISprite puzzleMobsterDialogueBox;
	public UILabel puzzleMobsterDialogueLabel;

}




