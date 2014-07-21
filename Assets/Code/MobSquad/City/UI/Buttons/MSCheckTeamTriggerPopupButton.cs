using UnityEngine;
using System.Collections;

public class MSCheckTeamTriggerPopupButton : MSTriggerPopupButton {

	public override void OnClick ()
	{
		foreach (var item in MSMonsterManager.instance.userTeam) 
		{
			if (item != null && item.monster != null && item.monster.monsterId > 0)
			{
				base.OnClick ();
				return;
			}
		}
		Fail();
	}

	void Fail()
	{
		//Center on team center
		MSTownCamera.instance.CenterOnGroundPos(MSBuildingManager.teamCenter.trans.position, .4f);

		//Popup error message
		MSActionManager.Popup.DisplayError("You have no Mobsters on your team. Manage your team now.");
	}

}
