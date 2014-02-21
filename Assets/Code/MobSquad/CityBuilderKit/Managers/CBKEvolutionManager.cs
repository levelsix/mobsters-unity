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

	int oilCost
	{
		get
		{
			if (currEvolution == null)
			{
				return 0;
			}
			return CBKMonsterManager.instance.userMonsters.Find(x => x.userMonster.userMonsterId == currEvolution.userMonsterIds[0]).monster.evolutionCost;
		}
	}

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
		long partnerId = 0;
		foreach (var item in copiesOfThisMonster) 
		{
			if (item.userMonster.currentLvl >= item.monster.maxLevel)
			{
				partnerId = item.userMonster.userMonsterId;
				break;
			}
		}
		if (partnerId == 0)
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

		currEvolution.startTime = CBKUtil.timeNowMillis;

		EvolveMonsterRequestProto request = new EvolveMonsterRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.evolution = currEvolution;
		request.oilChange = oilCost;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_EVOLVE_MONSTER_EVENT, ReceiveEvolutionStartResponse);

	}

	void ReceiveEvolutionStartResponse(int tagNum)
	{
		EvolveMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as EvolveMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != EvolveMonsterResponseProto.EvolveMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem evolving monster: " + response.status.ToString());
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
		EvolutionFinishedRequestProto request = new EvolutionFinishedRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.gemsSpent = gems;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_EVOLUTION_FINISHED_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		EvolutionFinishedResponseProto response = UMQNetworkManager.responseDict[tagNum] as EvolutionFinishedResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == EvolutionFinishedResponseProto.EvolutionFinishedStatus.SUCCESS)
		{
			//TODO: Update monster

			foreach (var item in currEvolution.userMonsterIds) {
				CBKMonsterManager.instance.RemoveMonster(item);
			}
			CBKMonsterManager.instance.RemoveMonster(currEvolution.catalystUserMonsterId);

			CBKMonsterManager.instance.UpdateOrAdd(response.evolvedMonster);

			currEvolution = null;
		}
		else
		{
			Debug.LogError("Problem completing evolution: " + response.status.ToString());
		}
	}

}
