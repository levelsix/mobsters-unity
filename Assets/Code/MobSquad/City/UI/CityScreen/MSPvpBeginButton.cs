using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSPvpBeginButton
/// </summary>
public class MSPvpBeginButton : MonoBehaviour {

	bool loading = false;

	void OnClick()
	{
		if (MSMonsterManager.monstersOwned == 0)
		{
			MSPopupManager.instance.CreatePopup("No Toons!",
				"Uh oh, you have no toons on your team. Manage your team?",
        		new string[]{"Later", "Manage"},
				new string[]{"greymenuoption", "greenmenuoption"},
				new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups();
						MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.gameObject.GetComponent<MSPopup>());
						MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);}}
			);
			return;
		}
		else if (MSMonsterManager.instance.userMonsters.Count > MSMonsterManager.instance.totalResidenceSlots)
		{
			MSPopupManager.instance.CreatePopup("Residences Full!",
				"Uh oh, you have recruited too many toons. Manage your team?",
                new string[]{"Later", "Manage"},
				new string[]{"greymenuoption", "greenmenuoption"},
				new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups();
						MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.gameObject.GetComponent<MSPopup>());
						MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);}}
			);
			return;
		}
		else
		{
			int i;
			for (i = 0; i < MSMonsterManager.instance.userTeam.Length; i++) 
			{
				if (MSMonsterManager.instance.userTeam[i] != null && MSMonsterManager.instance.userTeam[i].currHP > 0)
				{
					break;
				}
			}
			if (i == MSMonsterManager.instance.userTeam.Length)
			{
				MSPopupManager.instance.CreatePopup("No Toons!",
					"No toons on team have health! Manage your team?",
	        		new string[]{"Later", "Manage"},
					new string[] {"greymenuoption", "greenmenuoption"},
					new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
						delegate{MSActionManager.Popup.CloseAllPopups();
							MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.gameObject.GetComponent<MSPopup>());
							MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);}}
				);
				return;
			}
		}

		if (MSResourceManager.instance.Spend(ResourceType.CASH, PZCombatManager.instance.pvpMatchCost, LoadWithGems))
		{
			Load(PZCombatManager.instance.pvpMatchCost);
		}
	}

	void LoadWithGems()
	{
		int gemCost = Mathf.CeilToInt((PZCombatManager.instance.pvpMatchCost - MSResourceManager.resources[ResourceType.CASH]) * MSWhiteboard.constants.gemsPerResource);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
		{
			Load(MSResourceManager.instance.SpendAll(ResourceType.CASH), gemCost);
		}
	}

	void Load(int cash, int gems = 0)
	{
		MSActionManager.Popup.CloseAllPopups();
		
		MSActionManager.Scene.OnPuzzle();

		PZCombatManager.instance.InitPvp(cash, gems);
	}

}
