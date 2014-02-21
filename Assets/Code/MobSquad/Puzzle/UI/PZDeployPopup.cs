using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZDeployPopup : MonoBehaviour {

	[SerializeField]
	PZDeployCard[] slots;
	
	const int NUM_SLOTS = 3;

	UITweener tween;

	void Awake()
	{
		tween = GetComponent<UITweener>();
	}

	void OnEnable()
	{
		CBKEventManager.Puzzle.OnDeploy += OnDeploy;
		CBKEventManager.Scene.OnPuzzle += OnPuzzle;
	}
	
	void OnDisable()
	{
		CBKEventManager.Puzzle.OnDeploy -= OnDeploy;
		CBKEventManager.Scene.OnPuzzle -= OnPuzzle;
	}

	void OnPuzzle()
	{
		tween.PlayForward();
	}

	void OnDeploy(PZMonster monster)
	{
		tween.PlayReverse();
	}
	
	public void Init(PZMonster[] userMonsters)
	{
		tween.PlayForward();

		int i;
		for (i = 0; i < userMonsters.Length; i++) 
		{
			if (userMonsters[i] != null && userMonsters[i].monster != null && userMonsters[i].monster.monsterId > 0)
			{
				slots[i].Init(userMonsters[i]);
			}
			else
			{
				slots[i].InitEmpty();
			}
		}
	}
}
