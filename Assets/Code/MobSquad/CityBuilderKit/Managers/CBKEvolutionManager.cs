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

	public static CBKEvolutionManager instance;

	public UserMonsterEvolutionProto currEvolution = null;

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

	public long timeLeftMillis
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

	public bool ready
	{
		get
		{
			return currEvolution != null && currEvolution.catalystUserMonsterId != 0 && currEvolution.userMonsterIds.Count >= 2;
		}
	}

	public bool active
	{
		get
		{
			return currEvolution != null && currEvolution.startTime > 0;
		}
	}

	void Awake()
	{
		instance = this;
		currEvolution = null;
	}

	public bool IsMonsterEvolving(long userMonsterId)
	{
		return currEvolution != null && 
			(currEvolution.catalystUserMonsterId == userMonsterId 
			 || currEvolution.userMonsterIds.Contains(userMonsterId));
	}

	public UserMonsterEvolutionProto TryEvolveMonster(PZMonster monster, PZMonster buddy)
	{
		currEvolution = new UserMonsterEvolutionProto();
		
		currEvolution.userMonsterIds.Add (monster.userMonster.userMonsterId);

		if (buddy != null)
		{
			currEvolution.userMonsterIds.Add(buddy.userMonster.userMonsterId);
		}

		List<PZMonster> catalysts = CBKMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId);
		if (catalysts.Count > monster.monster.numCatalystMonstersRequired)
		{
			currEvolution.catalystUserMonsterId = catalysts[0].userMonster.userMonsterId;
		}

		return currEvolution;
	}

	public void StartEvolution()
	{
		if (currEvolution == null)
		{
			Debug.LogError("ERROR: No evolution to start");
			return;
		}

		if (!ready)
		{
			Debug.LogError("ERROR: Not enough stuff for evolution");
			return;
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
		if (!CBKSceneManager.instance.loadingState && currEvolution != null && timeLeftMillis <= 0)
		{
			StartCoroutine(CompleteEvolution());
		}
	}

	public void FinishWithGems()
	{
		int gems = Mathf.CeilToInt((timeLeftMillis / 60000f) / CBKWhiteboard.constants.minutesPerGem);
		if (CBKResourceManager.instance.Spend(ResourceType.GEMS, gems))
		{
			StartCoroutine(CompleteEvolution(gems));
		}
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
