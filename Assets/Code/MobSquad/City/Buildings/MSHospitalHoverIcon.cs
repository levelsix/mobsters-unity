using UnityEngine;
using System.Collections;

public class MSHospitalHoverIcon : MSBuildingFrame {

	void OnEnable(){
		MSActionManager.Goon.OnHealQueueChanged += CheckTag;
		CheckTag ();
	}

	public override void CheckTag(){
		if(bubbleIcon != null)
		{
			bubbleIcon.gameObject.SetActive(false);
			if (MSHospitalManager.instance.healingMonsters.Count == 0) {
				int monstersNeedHealing = 0;
				foreach (PZMonster monster in MSMonsterManager.instance.userMonsters) {
					if(monster.totalHealthToHeal > 0){
						monstersNeedHealing++;
					}
				}
							
				if(monstersNeedHealing >= 1){
					if(monstersNeedHealing > 9){
						bubbleIcon.spriteName = "healredbubble" + "exclamation";
					}
					else
					{
						bubbleIcon.spriteName = "healredbubble" + monstersNeedHealing;
					}
					bubbleIcon.gameObject.SetActive(true);
					bubbleIcon.MakePixelPerfect();
				}
			}
		}

		if( bubbleIcon != null && !Precheck())
		{
			bubbleIcon.gameObject.SetActive(false);
		}

	}

	void OnDisable(){
		MSActionManager.Goon.OnHealQueueChanged -= CheckTag;
	}
}
