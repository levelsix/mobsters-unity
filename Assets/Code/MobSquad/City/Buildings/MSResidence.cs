using UnityEngine;
using System.Collections;

public class MSResidence : MSBuildingFrame {

	void OnEnable(){
		CheckFullResidence ();
		MSActionManager.Goon.OnMonsterListChanged += CheckFullResidence;
	}

	public void CheckFullResidence(){
		if (MSMonsterManager.monstersOwned >= MSMonsterManager.instance.totalResidenceSlots) {
			hoverIcon.gameObject.SetActive(true);
			hoverIcon.spriteName = "obfull";
			hoverIcon.MakePixelPerfect();
		} else {
			hoverIcon.gameObject.SetActive(false);
		}
	}

	void OnDisable(){
		MSActionManager.Goon.OnMonsterListChanged -= CheckFullResidence;
	}
}
