using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKEvolutionManager
/// </summary>
public class CBKEvolutionManager : MonoBehaviour {

	static UserMonsterEvolutionProto currEvolution = null;

	static bool active = false;

	long timeLeftMillis
	{
		get
		{
			if (currEvolution == null)
			{
				return 0;
			}
			else
			{
				return CBKUtil.timeNowMillis - currEvolution.startTime;
			}
		}
	}

	public bool IsMonsterEvolving(long userMonsterId)
	{
		return currEvolution != null && 
			(currEvolution.catalystUserMonsterId == userMonsterId 
			 || currEvolution.userMonsterIds.Contains(userMonsterId));
	}

	public bool TryEvolveMonster(PZMonster monster, out string failString)
	{
		if (monster.userMonster.currentLvl < monster.monster.maxLevel)
		{
			failString = monster.monster.displayName + " needs to be max level " + monster.monster.maxLevel + " to begin the evolution";
			return false;
		}

		List<PZMonster> catalysts = CBKMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId);
		if (catalysts.Count < monster.monster.numCatalystMonstersRequired)
		{
			MonsterProto catalyst = CBKDataManager.instance.Get<MonsterProto>(monster.monster.evolutionCatalystMonsterId);
			failString = "You must find a " + catalyst.displayName + " to begin the evolution.";
			return false;
		}

		List<PZMonster> copiesOfThisMonster = CBKMonsterManager.instance.GetMonstersByMonsterId(monster.monster.monsterId, monster.userMonster.userMonsterId);
		if (copiesOfThisMonster.Count == 0)
		{
			failString = "You need another " + monster.monster.displayName + " to begin the evolution.";
			return false;
		}

		currEvolution = new UserMonsterEvolutionProto();
		currEvolution.userMonsterIds.Add (monster.userMonster.userMonsterId);
		currEvolution.userMonsterIds.Add (copiesOfThisMonster[0].userMonster.userMonsterId);
		currEvolution.catalystUserMonsterId = catalysts[0].userMonster.userMonsterId;

		failString = "";
		return true;
	}

	public void StartEvolution()
	{
		if (currEvolution == null)
		{
			Debug.LogError("ERROR: No evolution to start");
		}
	}

	void Update()
	{
		if (currEvolution != null && timeLeftMillis <= 0)
		{
			StartCoroutine(CompleteEvolution());
		}
	}

	void FinishWithGems()
	{
		StartCoroutine(CompleteEvolution(Mathf.CeilToInt((timeLeftMillis / 60000f) / CBKWhiteboard.constants.minutesPerGem)));
	}

	IEnumerator CompleteEvolution(int gems = 0)
	{
		yield return null;
	}


}
