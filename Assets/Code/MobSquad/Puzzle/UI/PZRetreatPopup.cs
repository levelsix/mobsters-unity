using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class PZRetreatPopup : MonoBehaviour {

	[SerializeField]
	MSActionButton cancelButton;

	[SerializeField]
	MSActionButton retreatButton;

	[SerializeField]
	UILabel chanceLabel;

	void Start()
	{
		cancelButton.onClick = MSActionManager.Popup.CloseAllPopups;
		retreatButton.onClick = delegate { 
			MSActionManager.Puzzle.ForceHideSwap(); 
			MSActionManager.Popup.CloseAllPopups(); 
			PZCombatManager.instance.RunPlayerForfeit();
		};
	}

	void OnEnable()
	{
		chanceLabel.text = (int)(PZCombatManager.instance.forfeitChance * 100f / 1) + "%";
	}
}
