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

	public UserMonsterEvolutionProto tempEvolution = null;

	public UserMonsterEvolutionProto currEvolution = null;

	public PZMonster tempEvoMonster
	{
		get
		{
			if (tempEvolution == null || tempEvolution.userMonsterIds.Count == 0)
			{
				return null;
			}
			return MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == tempEvolution.userMonsterIds[0]);
		}
	}

	public PZMonster tempBuddy
	{
		get
		{
			if (tempEvolution == null || tempEvolution.userMonsterIds.Count < 2)
			{
				return null;
			}
			return MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == tempEvolution.userMonsterIds[1]);
		}
	}

	long finishTime;

	int oilCost
	{
		get
		{
			if (tempEvolution == null)
			{
				return 0;
			}
			return MSMonsterManager.instance.userMonsters.Find(x => x.userMonster.userMonsterId == tempEvolution.userMonsterIds[0]).monster.evolutionCost;
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

	public int gemsToFinish
	{
		get
		{
			return MSMath.GemsForTime (timeLeftMillis);
		}
	}

	public bool ready
	{
		get
		{
			return tempEvolution != null && tempEvolution.catalystUserMonsterId != 0 && tempEvolution.userMonsterIds.Count >= 2;
		}
	}

	public bool hasEvolution
	{
		get
		{
			return tempEvolution != null && tempEvolution.userMonsterIds.Count > 0;
		}
	}

	public bool isEvolving
	{
		get
		{
			return currEvolution != null && currEvolution.startTime > 0;
		}
	}

	void Awake()
	{
		instance = this;
		tempEvolution = null;
		currEvolution = null;
	}

	public void Init(UserMonsterEvolutionProto evo)
	{
		currEvolution = evo;
		if (isEvolving)
		{
			string str = "Evo monsters:";
			foreach (var item in evo.userMonsterIds) 
			{
				str += " " + item;
			}
			Debug.LogWarning(str);
			finishTime = evo.startTime + 
				MSDataManager.instance.Get<MonsterProto>(MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId==evo.userMonsterIds[0]).monster.evolutionMonsterId).minutesToEvolve * 60000;
		}
	}

	public bool IsMonsterEvolving(long userMonsterId)
	{
		return isEvolving && 
			(currEvolution.catalystUserMonsterId == userMonsterId 
			 || currEvolution.userMonsterIds.Contains(userMonsterId));
	}

	public UserMonsterEvolutionProto TryEvolveMonster(PZMonster monster, PZMonster buddy)
	{
		tempEvolution = new UserMonsterEvolutionProto();
		
		tempEvolution.userMonsterIds.Add (monster.userMonster.userMonsterId);

		if (buddy != null)
		{
			tempEvolution.userMonsterIds.Add(buddy.userMonster.userMonsterId);
		}

		List<PZMonster> catalysts = MSMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId);
		if (catalysts.Count >= monster.monster.numCatalystMonstersRequired)
		{
			tempEvolution.catalystUserMonsterId = catalysts[0].userMonster.userMonsterId;
		}

		return tempEvolution;
	}

	public void StartEvolution(bool useGems = false)
	{
		if (tempEvolution == null)
		{
			MSActionManager.Popup.DisplayError("ERROR: No evolution to start");
			return;
		}

		if (!ready)
		{
			MSActionManager.Popup.DisplayError("ERROR: Not enough stuff for evolution");
			return;
		}

		if (isEvolving)
		{
			MSPopupManager.instance.CreatePopup("Lab Busy", 
	            "Your lab is busy evolving another monster. Spend (g) " + gemsToFinish + " to finish it?",
          	  	new string[] {"No", "Yes"},
				new string[] {"greymenuoption", "purplemenuoption"},
				new Action[] {MSActionManager.Popup.CloseTopPopupLayer, delegate{FinishWithGems();StartEvolution();}},
				"purple"
			);
		}

		if (useGems)
		{
			int gemCost = Mathf.CeilToInt((oilCost - MSResourceManager.resources[ResourceType.OIL]) * MSWhiteboard.constants.gemsPerResource);
			if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
			{
				StartEvolution(MSResourceManager.instance.SpendAll(ResourceType.OIL), gemCost);
			}
		}
		else if (MSResourceManager.instance.Spend(ResourceType.OIL, oilCost, delegate{StartEvolution(true);}))
	    {
			StartEvolution(oilCost);
		}
	}

	void StartEvolution(int oil, int gems = 0)
	{
		currEvolution = tempEvolution;
		
		currEvolution.startTime = MSUtil.timeNowMillis;
		
		EvolveMonsterRequestProto request = new EvolveMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.evolution = currEvolution;
		request.oilChange = -oil;
		request.gemsSpent = gems;
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_EVOLVE_MONSTER_EVENT, ReceiveEvolutionStartResponse);
		
		finishTime = currEvolution.startTime + MSDataManager.instance.Get<MonsterProto>(tempEvoMonster.monster.evolutionMonsterId).minutesToEvolve * 60000;
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
		if (!MSSceneManager.instance.loadingState 
		    && currEvolution != null 
		    && currEvolution.userMonsterIds.Count == 2
		    && currEvolution.catalystUserMonsterId > 0
		    && timeLeftMillis <= 0)
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

		foreach (var item in currEvolution.userMonsterIds) 
		{
			MSMonsterManager.instance.RemoveMonster(item);
		}

		MSMonsterManager.instance.RemoveMonster(currEvolution.catalystUserMonsterId);
		
		currEvolution = null;

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		EvolutionFinishedResponseProto response = UMQNetworkManager.responseDict[tagNum] as EvolutionFinishedResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == EvolutionFinishedResponseProto.EvolutionFinishedStatus.SUCCESS)
		{
			PZMonster newMonster = MSMonsterManager.instance.UpdateOrAdd(response.evolvedMonster);
			if (MSActionManager.Goon.OnEvolutionComplete != null)
			{
				MSActionManager.Goon.OnEvolutionComplete(newMonster);
			}
		}
		else
		{
			Debug.LogError("Problem completing evolution: " + response.status.ToString());
		}
	}

}
