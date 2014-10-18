using UnityEngine;
using System.Collections;

public class MSCheckTeamTriggerPopupButton : MSTriggerPopupButton {

	public override void OnClick ()
	{
		//total number of usable teammates
		int teamCount = 0;
		if(MSMonsterManager.instance.userMonsters.Count >= MSMonsterManager.instance.totalResidenceSlots )
		{
			ResidenceFullFail();
			return;
		}
		foreach (var item in MSMonsterManager.instance.userTeam) 
		{
			if (item != null && item.monster != null && item.monster.monsterId > 0 && item.currHP > 0)
			{
				teamCount++;
			}
		}

		if(teamCount == 0)
		{
			NoTeamFail();
			return;
		}
		else if(teamCount < MSMonsterManager.instance.userTeam.Length)
		{
			int healthyCount = 0;
			foreach(PZMonster monster in MSMonsterManager.instance.userMonsters)
			{
				if(monster.userMonster.teamSlotNum == 0 && monster.currHP > 0)
				{
					CouldHaveMoreOnTeamFail();
					return;
				}
			}
		}
		base.OnClick ();

	}

	void NoTeamFail()
	{
		//Center on team center
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.teamCenter.trans.position, .4f);
		MSBuildingManager.teamCenter.SetArrow(true);

		//Popup error message
		MSActionManager.Popup.DisplayRedError("You have no toons on your team. Manage your team now.");
		MSTaskBar.instance.manageTeamNeedsArrow = true;
	}

	void CouldHaveMoreOnTeamFail()
	{
		//Center on team center
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.teamCenter.trans.position, .4f);
		MSBuildingManager.teamCenter.SetArrow(true);
		
		//Popup error message
		MSActionManager.Popup.DisplayGreenError("You have healthy toons available! Manage your team now.");
		MSTaskBar.instance.manageTeamNeedsArrow = true;
	}

	void ResidenceFullFail()
	{
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.residences[0].trans.position, .4f);
		MSBuildingManager.residences[0].SetArrow(true);
		MSActionManager.Popup.DisplayRedError("Your residences are full. Sell " + MSValues.monsterName + "s to free up space.");
	}



}
