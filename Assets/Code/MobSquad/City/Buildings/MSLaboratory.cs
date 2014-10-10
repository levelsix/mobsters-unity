using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSLaboratory : MSBuildingFrame {

	MSBuilding lab
	{
		get
		{
			if (MSBuildingManager.enhanceLabs.Count == 0) return null;
			return MSBuildingManager.enhanceLabs[0];
		}
	}

	void OnEnable(){
		CheckTag ();
		MSActionManager.Goon.OnEnhanceQueueChanged += CheckTag;
		MSActionManager.Scene.OnCity += CheckTag;
	}
	
	void OnDisable(){
		MSActionManager.Goon.OnEnhanceQueueChanged -= CheckTag;
		MSActionManager.Scene.OnCity -= CheckTag;
	}

	public override void CheckTag(){
		if (bubbleIcon != null)
		{
			if (lab != null && lab.combinedProto.structInfo.level == 0)
			{
				bubbleIcon.gameObject.SetActive(true);
				bubbleIcon.spriteName = "fixbubble";
				bubbleIcon.MakePixelPerfect();
				return;
			}

			int canEnhance = 0;

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
	}

}
