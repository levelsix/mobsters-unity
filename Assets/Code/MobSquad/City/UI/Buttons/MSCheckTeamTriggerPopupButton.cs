using UnityEngine;
using System.Collections;

public class MSCheckTeamTriggerPopupButton : MSTriggerPopupButton {

	public override void OnClick ()
	{

		int teamCount = 0;
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
		}
		else if(teamCount < MSMonsterManager.instance.userTeam.Length)
		{
			int healthyCount = 0;
			foreach(PZMonster monster in MSMonsterManager.instance.userMonsters)
			{
				if(monster.currHP > 0)
				{
					healthyCount++;
				}
			}

			if(healthyCount != 0)
			{
				CouldHaveMoreOnTeamFail();
			}
		}
		else if(teamCount == MSMonsterManager.instance.userTeam.Length)
		{
			base.OnClick ();
			return;
		}
	}

	void NoTeamFail()
	{
		//Center on team center
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.teamCenter.trans.position, .4f);

		//Popup error message
		MSActionManager.Popup.DisplayRedError("You have no Mobsters on your team. Manage your team now.");
	}

	void CouldHaveMoreOnTeamFail()
	{
		//Center on team center
		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.teamCenter.trans.position, .4f);
		
		//Popup error message
		MSActionManager.Popup.DisplayGreenError("You have healthy toons available! Manage your team now.");
	}

}
