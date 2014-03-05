using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKEvolutionManager
/// </summary>
public class MSEvolutionManager : MonoBehaviour {

	public static MSEvolutionManager instance;

	public UserMonsterEvolutionProto currEvolution = null;

	long finishTime;

	int oilCost
	{
		get
		{
			if (currEvolution == null)
			{
				return 0;
			}
			return MSMonsterManager.instance.userMonsters.Find(x => x.userMonster.userMonsterId == currEvolution.userMonsterIds[0]).monster.evolutionCost;
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
				return finishTime - MSUtil.timeNowMillis ;
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

	public void Init(UserMonsterEvolutionProto evo)
	{
		currEvolution = evo;
		if (evo != null)
		{
			string str = "Evo monsters:";
			foreach (var item in evo.userMonsterIds) {
				str += " " + item;
			}
			Debug.LogWarning(str);
			finishTime = evo.startTime + MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId==evo.userMonsterIds[0]).monster.minutesToEvolve * 60000;
		}
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

		List<PZMonster> catalysts = MSMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId);
		if (catalysts.Count >= monster.monster.numCatalystMonstersRequired)
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

		currEvolution.startTime = MSUtil.timeNowMillis;

		EvolveMonsterRequestProto request = new EvolveMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.evolution = currEvolution;
		request.oilChange = oilCost;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_EVOLVE_MONSTER_EVENT, ReceiveEvolutionStartResponse);

		finishTime = currEvolution.startTime + 
			MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId==currEvolution.userMonsterIds[0]).monster.minutesToEvolve * 60000;
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
		if (!MSSceneManager.instance.loadingState && currEvolution != null && timeLeftMillis <= 0)
		{
			StartCoroutine(CompleteEvolution());
		}
	}

	public void FinishWithGems()
	{
		int gems = Mathf.CeilToInt((timeLeftMillis / 60000f) / MSWhiteboard.constants.minutesPerGem);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gems))
		{
			StartCoroutine(CompleteEvolution(gems));
		}
	}

	IEnumerator CompleteEvolution(int gems = 0)
	{
		EvolutionFinishedRequestProto request = new EvolutionFinishedRequestProto();
		request.sender = MSWhiteboard.localMup;
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
				MSMonsterManager.instance.RemoveMonster(item);
			}
			MSMonsterManager.instance.RemoveMonster(currEvolution.catalystUserMonsterId);

			MSMonsterManager.instance.UpdateOrAdd(response.evolvedMonster);

			currEvolution = null;
		}
		else
		{
			Debug.LogError("Problem completing evolution: " + response.status.ToString());
		}
	}

}
