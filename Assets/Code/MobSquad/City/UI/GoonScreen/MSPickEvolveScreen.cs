using UnityEngine;
using System.Collections;

public class MSPickEvolveScreen : MSFunctionalScreen {

	public static MSPickEvolveScreen instance;

	[SerializeField]
	UITable table;

	[SerializeField]
	GameObject noMobstersReadyElements;

	[SerializeField]
	UIGrid readyToEvolveGrid;

	[SerializeField]
	MSMobsterGrid notReadyGrid;

	[SerializeField]
	GameObject readyToEvolveHeader;

	[SerializeField]
	GameObject notReadyHeader;

	void Awake()
	{
		instance = this;
	}

	public override void Init ()
	{
		if (MSEvolutionManager.instance.hasEvolution)
		{
			MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.DO_EVOLVE);
		}
		else
		{
			notReadyGrid.Init(GoonScreenMode.PICK_EVOLVE);

			foreach (var item in notReadyGrid.cards) 
			{
				if (item.FindBuddy(notReadyGrid.cards) 
				    && MSMonsterManager.instance.GetMonstersByMonsterId(item.monster.monster.evolutionCatalystMonsterId).Count > 0)
				{
					item.transform.parent = readyToEvolveGrid.transform;
				}
			}

			if (readyToEvolveGrid.transform.childCount == 0)
			{
				noMobstersReadyElements.SetActive(true);
				readyToEvolveHeader.SetActive(false);
				readyToEvolveGrid.gameObject.SetActive(false);
			}
			else
			{
				noMobstersReadyElements.SetActive(false);
				readyToEvolveHeader.SetActive(true);
				readyToEvolveGrid.gameObject.SetActive(true);
				readyToEvolveGrid.Reposition();
			}

			if (notReadyGrid.Count == 0)
			{
				notReadyHeader.SetActive(false);
				notReadyGrid.gameObject.SetActive(false);
			}
			else
			{
				notReadyHeader.SetActive(true);
				notReadyGrid.gameObject.SetActive(true);
				notReadyGrid.Reposition();
			}

			table.Reposition();
		}
	}

	public override bool IsAvailable ()
	{
		return MSBuildingManager.evoLabs.Count > 0
			&& !MSEvolutionManager.instance.hasEvolution;
	}

	public void AddMobster(MSGoonCard card)
	{

	}

}
