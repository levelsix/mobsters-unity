using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSLaboratory : MSBuildingFrame {

	//commented out now that building is defined in MSBuildingFrame
//	MSBuilding lab
//	{
//		get
//		{
//			if (MSBuildingManager.enhanceLabs.Count == 0) return null;
//			return MSBuildingManager.enhanceLabs[0];
//		}
//	}

	void OnEnable(){
		MSActionManager.Goon.OnEnhanceQueueChanged += CheckTag;
		MSActionManager.Scene.OnCity += CheckTag;
		FirstFrameCheck();
	}
	
	void OnDisable(){
		MSActionManager.Goon.OnEnhanceQueueChanged -= CheckTag;
		MSActionManager.Scene.OnCity -= CheckTag;
	}

	public override void CheckTag(){
		bubbleIcon.gameObject.SetActive(false);

		if (building.combinedProto.structInfo.level == 0 && Precheck())
		{
			bubbleIcon.gameObject.SetActive(true);
			bubbleIcon.spriteName = "fixbubble";
			bubbleIcon.MakePixelPerfect();
		}
		else if (MSMonsterManager.instance.currentEnhancementMonster == null ||
		    MSMonsterManager.instance.currentEnhancementMonster.monster.monsterId == 0) {

			int canEnhance = 0;
			foreach (PZMonster monster in MSMonsterManager.instance.userMonsters) {
				if(monster.level < monster.monster.maxLevel &&
				   (monster.monsterStatus == MonsterStatus.HEALTHY || monster.monsterStatus == MonsterStatus.INJURED)){
					canEnhance++;
				}
			}

			if(canEnhance > 1){
				bubbleIcon.gameObject.SetActive(true);
				bubbleIcon.spriteName = "enhancebubble";
				bubbleIcon.MakePixelPerfect();
			}
		}
	}

}
