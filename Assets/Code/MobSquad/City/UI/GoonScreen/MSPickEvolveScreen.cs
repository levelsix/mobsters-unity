using UnityEngine;
using System.Collections;

public class MSPickEvolveScreen : MSFunctionalScreen {

	public static MSPickEvolveScreen instance;

	[SerializeField]
	UITable table;

	[SerializeField]
	MSMobsterGrid readyToEvolveGrid;

	[SerializeField]
	MSMobsterGrid notReadyGrid;

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
			readyToEvolveGrid.Init(GoonScreenMode.PICK_EVOLVE);
			notReadyGrid.Init(GoonScreenMode.PICK_EVOLVE);
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
