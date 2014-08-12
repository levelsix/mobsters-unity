using UnityEngine;
using System.Collections;

public class MSLaboratory : MSBuildingFrame {

	void onEnable(){
		CheckTag ();
		MSActionManager.Goon.OnEnhanceQueueChanged += CheckTag;
	}

	public override void CheckTag(){
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

			if(canEnhance > 1 && Precheck()){
				bubbleIcon.gameObject.SetActive(true);
				bubbleIcon.spriteName = "enhancebubble";
				bubbleIcon.MakePixelPerfect();
			}else{
				bubbleIcon.gameObject.SetActive(false);
			}
		}
	}

	void onDisable(){
		MSActionManager.Goon.OnEnhanceQueueChanged -= CheckTag;
	}

}
