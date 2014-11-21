using UnityEngine;
using System.Collections;

public class MSCheckTeamTriggerPopupButton : MSTriggerPopupButton {

	public override void OnClick ()
	{
 		int teamCount = 0;

		int currPower = MSMonsterManager.instance.currTeamPower;

		//Max powers
		//He's the man with the name you want to touch
		//But you mustn't touch
		int maxPowers = MSBuildingManager.currTeamCenter.teamCostLimit;

		if(MSMonsterManager.instance.userMonsters.Count >= MSMonsterManager.instance.totalResidenceSlots )
		{
			ResidenceFullFail();
			return;
		}

		if (MSMonsterManager.instance.currTeamPower > MSBuildingManager.currTeamCenter.teamCostLimit)
		{
			//TooMuchPowerFail();
			//return;
		}

		foreach (var item in MSMonsterManager.instance.userTeam) 
		{
			if (item != null && item.monster != null && item.monster.monsterId > 0 && item.currHP > 0)
			{
				teamCount++;
			}
		}

		if (teamCount < MSMonsterManager.instance.userTeam.Length && currPower < maxPowers)
		{
			int healthyCount = 0;
			foreach(PZMonster monster in MSMonsterManager.instance.userMonsters)
			{
				if(monster.userMonster.teamSlotNum == 0 && monster.currHP > 0 && monster.userMonster.isComplete
				   && monster.teamCost <= (maxPowers-currPower))
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

	void TooMuchPowerFail()
	{
		//Center on team center
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.teamCenter.trans.position, .4f);
		MSBuildingManager.teamCenter.SetArrow(true);
		
		//Popup error message
		MSActionManager.Popup.DisplayRedError("Your team is over the power limit! Manage your team now.");
	}

	void ResidenceFullFail()
	{
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.residences[0].trans.position, .4f);
		MSBuildingManager.residences[0].SetArrow(true);
		MSActionManager.Popup.DisplayRedError("Your residences are full. Sell " + MSValues.monsterName + "s to free up space.");
	}



}
