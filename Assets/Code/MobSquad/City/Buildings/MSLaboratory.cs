using UnityEngine;
using System.Collections;

public class MSLaboratory : MSBuildingFrame {

	void onEnable(){
		canEnhance ();
		MSActionManager.Goon.OnEnhanceQueueChanged += canEnhance;
	}

	public void canEnhance(){
		int canEnhance = 0;
		//(currentEnhancementMonster == null || currentEnhancementMonster.monsterId == 0)
		if (MSMonsterManager.instance.currentEnhancementMonster == null ||
		    MSMonsterManager.instance.currentEnhancementMonster.monster.monsterId == 0) {
			foreach (PZMonster monster in MSMonsterManager.instance.userMonsters) {
				if(monster.level < monster.monster.maxLevel &&
				   (monster.monsterStatus == MonsterStatus.HEALTHY || monster.monsterStatus == MonsterStatus.INJURED)){
					canEnhance++;
				}
			}

			if(canEnhance > 1){
				hoverIcon.gameObject.SetActive(true);
				hoverIcon.spriteName = "obenhance";
				hoverIcon.MakePixelPerfect();
			}else{
				hoverIcon.gameObject.SetActive(false);
			}
		}
	}

	void onDisable(){
		MSActionManager.Goon.OnEnhanceQueueChanged -= canEnhance;
	}

}
