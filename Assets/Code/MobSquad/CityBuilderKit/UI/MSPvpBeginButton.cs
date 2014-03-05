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
		if (MSMonsterManager.monstersOnTeam == 0)
		{
			MSActionManager.Popup.CreateButtonPopup("Uh oh, you have no mobsters on your team. Manage your team?",
			                                        new string[]{"Later", "Manage"},
			new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
				delegate{MSActionManager.Popup.CloseAllPopups(); MSActionManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
					CBKGoonScreen.instance.InitHeal();}}
			);
			return;
		}
		else if (MSMonsterManager.instance.userMonsters.Count > MSMonsterManager.totalResidenceSlots)
		{
			MSActionManager.Popup.CreateButtonPopup("Uh oh, you have recruited too many mobsters. Manage your team?",
			                                        new string[]{"Later", "Manage"},
			new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
				delegate{MSActionManager.Popup.CloseAllPopups(); MSActionManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
					CBKGoonScreen.instance.InitHeal();}});
			return;
		}
		else
		{
			int i;
			for (i = 0; i < MSMonsterManager.userTeam.Length; i++) 
			{
				if (MSMonsterManager.userTeam[i] != null && MSMonsterManager.userTeam[i].currHP > 0)
				{
					break;
				}
			}
			if (i == MSMonsterManager.userTeam.Length)
			{
				MSActionManager.Popup.CreateButtonPopup("No monsters on team have health! Manage your team?",
				                                        new string[]{"Later", "Manage"},
				new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups(); MSActionManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
						CBKGoonScreen.instance.InitHeal();}});
				return;
			}
		}

		if (MSResourceManager.instance.Spend(ResourceType.CASH, PZCombatManager.MATCH_MONEY, OnClick))
		{
			Load();
		}
	}

	void Load()
	{
		PZCombatManager.instance.InitPvp();

		MSActionManager.Scene.OnPuzzle();
	}

}
