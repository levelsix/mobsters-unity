using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZDeployPopup : MonoBehaviour {

	[SerializeField]
	PZDeployCard[] slots;
	
	const int NUM_SLOTS = 3;

	UITweener tween;

	public static bool acting = false;

	void Awake()
	{
		tween = GetComponent<UITweener>();
	}

	void OnEnable()
	{
		MSActionManager.Puzzle.OnDeploy += OnDeploy;
		MSActionManager.Scene.OnPuzzle += OnPuzzle;
	}
	
	void OnDisable()
	{
		MSActionManager.Puzzle.OnDeploy -= OnDeploy;
		MSActionManager.Scene.OnPuzzle -= OnPuzzle;
	}

	void OnPuzzle()
	{
		if (!PZCombatManager.instance.pvpMode)
		{
			//tween.PlayForward();
		}
	}

	void OnDeploy(PZMonster monster)
	{
		acting = false;
		tween.PlayReverse();
	}

	public void Init()
	{
		Init (MSMonsterManager.instance.userTeam);
	}

	public void Init(UserCurrentMonsterTeamProto userMonsters)
	{
		tween.PlayForward();
		acting = true;
		for (int i = 0; i < userMonsters.currentTeam.Count; i++) 
		{
			PZMonster monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == userMonsters.currentTeam[i].userMonsterId);
			if (monster != null)
			{
				slots[i].Init(monster);
			}
			else
			{
				slots[i].InitEmpty();
			}
		}
	}

	public void Init(PZMonster[] userMonsters)
	{
		tween.PlayForward();
		acting = true;
		for (int i = 0; i < userMonsters.Length; i++) 
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
