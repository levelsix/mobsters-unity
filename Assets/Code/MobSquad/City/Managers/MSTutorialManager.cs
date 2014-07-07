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

	Stack<MSTutorial> currTutorials = new Stack<MSTutorial>();

	public MSTutorial currentTutorial
	{
		get
		{
			if (currTutorials.Count == 0)
			{
				return null;
			}
			return currTutorials.Peek();
		}
		set
		{
			currTutorials.Push(value);
		}
	}

	public bool inTutorial
	{
		get
		{
			return currentTutorial != null;
		}
	}

	public bool UiBlock
	{
		get
		{
			return currUi != null;
		}
	}

	public GameObject currUi = null;

	public TutorialData tutorialData;

	public TutorialUI TutorialUI;

	public bool holdUpEndingCombat = false;

	#region Task Tutorials
	Dictionary<int, Action> tutorialTasks = new Dictionary<int, Action>()
	{
		{5, StartBasicPuzzleTutorial},
		{6, StartPowerupPuzzleTutorial},
		{3, StartRainbowTutorial},
		{4, StartCombineTutorial},
		{1, StartCaptureTutorial}
	};

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnStartup(StartupResponseProto response)
	{
		tutorialTasks.Clear();
		tutorialTasks.Add(response.startupConstants.miniTuts.matchThreeTutorialAssetId,
		                  StartBasicPuzzleTutorial);
		tutorialTasks.Add (response.startupConstants.miniTuts.firstPowerUpAssetId,
		                   StartPowerupPuzzleTutorial);
		tutorialTasks.Add (response.startupConstants.miniTuts.rainbowTutorialAssetId,
		                   StartRainbowTutorial);
		tutorialTasks.Add (response.startupConstants.miniTuts.powerUpComboTutorialAssetId,
		                   StartCombineTutorial);
		tutorialTasks.Add (response.startupConstants.miniTuts.monsterDropTutorialAssetId,
		                   StartCaptureTutorial);
	}

	public void StartTutorial(int taskId)
	{
		tutorialTasks[taskId]();
	}

	public bool IsTaskTutorial(int taskId)
	{
		return tutorialTasks.ContainsKey(taskId);
	}

	static void StartBasicPuzzleTutorial()
	{
		instance.StartCoroutine(instance.tutorialData.basicPuzzleTutorial.Run());
	}

	static void StartPowerupPuzzleTutorial()
	{
		instance.StartCoroutine(instance.tutorialData.powerupPuzzleTutorial.Run());
	}

	static void StartRainbowTutorial()
	{
		instance.StartCoroutine(instance.tutorialData.rainbowPuzzleTutorial.Run());
	}
	
	static void StartCombineTutorial()
	{
		instance.StartCoroutine(instance.tutorialData.combinePuzzleTutorial.Run());
	}

	static void StartCaptureTutorial()
	{
		instance.StartCoroutine(instance.tutorialData.collectPuzzleTutorial.Run());
	}

	[ContextMenu ("Click Dialogue")]
	public void SkipDialogue()
	{
		MSActionManager.UI.OnDialogueClicked();
	}

	[ContextMenu ("Start First Tutorial")]
	public void StartBeginningTutorial()
	{
		StartCoroutine(tutorialData.beginningTutorial.Run());
	}

	public void EndTutorial()
	{
		currTutorials.Pop();
	}

	#endregion
}

[Serializable]
public class TutorialData
{
	public MSPuzzleTutorial basicPuzzleTutorial;
	public MSPuzzleTutorial powerupPuzzleTutorial;
	public MSPuzzleTutorial rainbowPuzzleTutorial;
	public MSPuzzleTutorial combinePuzzleTutorial;
	public MSPuzzleTutorial collectPuzzleTutorial;
	public MSPuzzleTutorial superEffectiveTutorial;
	public MSTutorial beginningTutorial;
}

[Serializable]
public class TutorialUI
{
	public UIWidget dialogueClickbox;

	public UISprite arrow;
	
	public MSDialogueUI leftDialogue;
	public MSDialogueUI rightDialogue;
	public MSDialogueUI puzzleDialogue;

}




