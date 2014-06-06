using UnityEngine;
using System.Collections;

public class MSHospitalHoverIcon : MSBuildingFrame {

	void OnEnable(){
		MSActionManager.Goon.OnHealQueueChanged += CheckTag;
		CheckTag ();
	}

	new public void CheckTag(){
		if (MSHospitalManager.instance.healingMonsters.Count == 0) {
			int monstersNeedHealing = 0;
			foreach (PZMonster monster in MSMonsterManager.instance.userMonsters) {
				if(monster.totalHealthToHeal > 0){
					monstersNeedHealing++;
				}
			}
			
			if(monstersNeedHealing > 1){
				hoverIcon.gameObject.SetActive(true);
				hoverIcon.spriteName = "obheal";
				hoverIcon.MakePixelPerfect();
			}
		} else {
			hoverIcon.gameObject.SetActive(false);
		}
	}

	void OnDisable(){
		MSActionManager.Goon.OnHealQueueChanged -= CheckTag;
	}
}
