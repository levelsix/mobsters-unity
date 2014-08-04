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

	[SerializeField]
	MSUIHelper scientistsHeader;

	[SerializeField]
	MSUIHelper currEvoHeader;

	void Awake()
	{
		instance = this;
	}

	public override void Init ()
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

		StartCoroutine(RepositionNextFrame());


		if (MSEvolutionManager.instance.isEvolving)
		{
			scientistsHeader.ResetAlpha(false);
			currEvoHeader.ResetAlpha(true);
		}
		else
		{
			scientistsHeader.ResetAlpha(false);
			currEvoHeader.ResetAlpha(true);
		}
	}

	public void ClickBottom()
	{
		MSEvolutionManager.instance.tempEvolution = MSEvolutionManager.instance.currEvolution;
		MSPopupManager.instance.popups.goonScreen.DoShiftRight(false);
	}

	IEnumerator RepositionNextFrame()
	{
		yield return null;
		table.Reposition();
	}

	public override bool IsAvailable ()
	{
		return MSBuildingManager.evoLabs.Count > 0
			&& !MSEvolutionManager.instance.hasEvolution;
	}

	public void AddMobster(PZMonster monster)
	{
		notReadyGrid.AddCard(monster, GoonScreenMode.PICK_EVOLVE, true);
	}

	void OnEvoFinish(PZMonster monster)
	{
		AddMobster(monster);
		currEvoHeader.Fade(false);
		scientistsHeader.Fade(true);
	}

}
