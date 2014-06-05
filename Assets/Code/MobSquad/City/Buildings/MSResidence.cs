using UnityEngine;
using System.Collections;

public class MSResidence : MSBuildingFrame {

	void onEnable(){
		OnMonsterListChanged ();
		MSActionManager.Goon.OnMonsterListChanged += OnMonsterListChanged;
	}

	void OnMonsterListChanged(){
		if (MSMonsterManager.monstersOwned == MSMonsterManager.instance.totalResidenceSlots) {
			hoverIcon.enabled = true;
			hoverIcon.spriteName = "obfull";
		} else {
			hoverIcon.enabled = false;
		}
	}
}
