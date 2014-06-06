using UnityEngine;
using System.Collections;

public class MSResidence : MSBuildingFrame {

	void OnEnable(){
		CheckTag ();
		MSActionManager.Goon.OnMonsterListChanged += CheckTag;
	}

	void Update(){
		CheckTag ();
	}

	new public void CheckTag(){
		if (MSMonsterManager.monstersOwned >= MSMonsterManager.instance.totalResidenceSlots) {
			hoverIcon.gameObject.SetActive(true);
			hoverIcon.spriteName = "obfull";
			hoverIcon.MakePixelPerfect();
		} else {
			hoverIcon.gameObject.SetActive(false);
		}
	}

	void OnDisable(){
		MSActionManager.Goon.OnMonsterListChanged -= CheckTag;
	}
}
