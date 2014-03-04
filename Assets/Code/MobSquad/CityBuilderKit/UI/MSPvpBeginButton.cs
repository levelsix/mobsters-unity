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
		if (CBKMonsterManager.monstersOnTeam == 0)
		{
			CBKEventManager.Popup.CreateButtonPopup("Uh oh, you have no mobsters on your team. Manage your team?",
			                                        new string[]{"Later", "Manage"},
			new Action[]{delegate{CBKEventManager.Popup.CloseTopPopupLayer();},
				delegate{CBKEventManager.Popup.CloseAllPopups(); CBKEventManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
					CBKGoonScreen.instance.InitHeal();}}
			);
			return;
		}
		else if (CBKMonsterManager.instance.userMonsters.Count > CBKMonsterManager.totalResidenceSlots)
		{
			CBKEventManager.Popup.CreateButtonPopup("Uh oh, you have recruited too many mobsters. Manage your team?",
			                                        new string[]{"Later", "Manage"},
			new Action[]{delegate{CBKEventManager.Popup.CloseTopPopupLayer();},
				delegate{CBKEventManager.Popup.CloseAllPopups(); CBKEventManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
					CBKGoonScreen.instance.InitHeal();}});
			return;
		}
		else
		{
			int i;
			for (i = 0; i < CBKMonsterManager.userTeam.Length; i++) 
			{
				if (CBKMonsterManager.userTeam[i] != null && CBKMonsterManager.userTeam[i].currHP > 0)
				{
					break;
				}
			}
			if (i == CBKMonsterManager.userTeam.Length)
			{
				CBKEventManager.Popup.CreateButtonPopup("No monsters on team have health! Manage your team?",
				                                        new string[]{"Later", "Manage"},
				new Action[]{delegate{CBKEventManager.Popup.CloseTopPopupLayer();},
					delegate{CBKEventManager.Popup.CloseAllPopups(); CBKEventManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
						CBKGoonScreen.instance.InitHeal();}});
				return;
			}
		}

		if (CBKResourceManager.instance.Spend(ResourceType.CASH, PZCombatManager.MATCH_MONEY, OnClick))
		{
			Load();
		}
	}

	void Load()
	{
		PZCombatManager.instance.InitPvp();

		CBKEventManager.Scene.OnPuzzle();
	}

}
