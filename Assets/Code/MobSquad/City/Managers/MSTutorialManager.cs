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

	public TutorialUI TutorialUI;

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
		instance.StartCoroutine(instance.tutorialData.basicPuzzleTutorial.Run());
	}

	static void SetupTutorial2()
	{
		Debug.Log("Starting Tutorial 2");
		PZPuzzleManager.instance.InitBoard(6,6);
	}

	[ContextMenu ("Click Dialogue")]
	public void SkipDialogue()
	{
		MSActionManager.UI.OnDialogueClicked();
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
	public UIWidget dialogueClickbox;

	public UISprite arrow;

	public UI2DSprite leftMobster;
	public UITweener leftMobsterTween;
	public UISprite leftMobsterDialogueBox;
	public UITweener leftMobsterDialogueBoxTween;
	public UILabel leftMobsterDialogueLabel;
	public UILabel leftMobsterNameLabel;
	
	public UI2DSprite rightMobster;
	public UITweener rightMobsterTween;
	public UISprite rightMobsterDialogueBox;
	public UITweener rightMobsterDialogueBoxTween;
	public UILabel rightMobsterDialogueLabel;
	public UILabel rightMobsterNameLabel;

	public GameObject puzzleMobsterClickbox;
	public UI2DSprite puzzleMobster;
	public UITweener puzzleMobsterTween;
	public UISprite puzzleMobsterDialogueBox;
	public UITweener puzzleMobsterDialogueBoxTween;
	public UILabel puzzleMobsterDialogueLabel;
	public UILabel puzzleMonsterNameLabel;

}




