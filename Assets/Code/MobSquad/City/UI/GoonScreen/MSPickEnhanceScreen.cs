using UnityEngine;
using System.Collections;

public class MSPickEnhanceScreen : MSFunctionalScreen 
{
	public static MSPickEnhanceScreen instance;

	[SerializeField]
	MSMobsterGrid grid;

	[SerializeField]
	MSGoonScreen goonScreen;

	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Determines whether this instance is available.
	/// If we're already enhancing, make sure to skip this window
	/// and go on
	/// </summary>
	/// <returns><c>true</c> if this instance is available; otherwise, <c>false</c>.</returns>
	public override bool IsAvailable ()
	{
		return MSBuildingManager.enhanceLabs.Count > 0
			&& MSBuildingManager.enhanceLabs.Find(x=>x.combinedProto.structInfo.level > 0)
				&& !MSEnhancementManager.instance.hasEnhancement;
	}

	public override void Init ()
	{
		if (MSEnhancementManager.instance.hasEnhancement)
		{
			goonScreen.Init(GoonScreenMode.DO_ENHANCE);
		}
		else
		{
			grid.Init(GoonScreenMode.PICK_ENHANCE);
		}
	}

	public void PickMonster(PZMonster monster)
	{
		MSEnhancementManager.instance.SetEnhancementMonster(monster);
		goonScreen.DoShiftRight(false);
	}

}
