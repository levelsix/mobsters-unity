using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSLaboratory : MSBuildingFrame {

	MSProgressBar bar;

	private readonly Vector3 OFFSET = new Vector3(-0.1f, 3.75f, 0f);

	public static MSLaboratory instance;

	void Awake()
	{
		base.Awake();
		bar = MSPoolManager.instance.Get<MSProgressBar>(MSPrefabList.instance.progressBar, transform);
		bar.transform.localPosition = new Vector3(0f,3.2f,0f);
		bar.transform.localEulerAngles = buildingAngle;
		bar.transform.localScale = buildingScale;
		bar.gameObject.SetActive(false);
	}

	void OnEnable(){
		MSActionManager.Goon.OnStartEnhance += InitBar;
		MSActionManager.Loading.OnBuildingsLoaded += InitBar;
		MSActionManager.Goon.OnEnhanceQueueChanged += CheckTag;
		MSActionManager.Scene.OnCity += CheckTag;
		FirstFrameCheck();
	}
	
	void OnDisable(){
		MSActionManager.Goon.OnStartEnhance -= InitBar;
		MSActionManager.Loading.OnBuildingsLoaded -= InitBar;
		MSActionManager.Goon.OnEnhanceQueueChanged -= CheckTag;
		MSActionManager.Scene.OnCity -= CheckTag;
	}

	//Try to init when the building is enabled and when enhancing starts
	public void InitBar()
	{
		UserEnhancementProto currEnhancement = MSEnhancementManager.instance.currEnhancement;
		if(currEnhancement != null && currEnhancement.feeders.Count > 0)
		{
			bar.init(MSEnhancementManager.instance.startTime, MSEnhancementManager.instance.timeLeft, true);
		}
		CheckTag();
	}

	public override void CheckTag(){
		bubbleIcon.gameObject.SetActive(false);
		if(Precheck() && !bar.gameObject.activeSelf)
		{
			if (building.combinedProto.structInfo.level == 0)
			{
				bubbleIcon.gameObject.SetActive(true);
				bubbleIcon.spriteName = "fixbubble";
				bubbleIcon.MakePixelPerfect();
				return;
			}
			else if(building.combinedProto.structInfo.level >= 5)
			{
				foreach(PersistentEventProto pEvent in MSEventManager.instance.GetActiveEvents())
				{
					if (pEvent.type == PersistentEventProto.EventType.ENHANCE)
					{
						if(!MSEventManager.instance.IsOnCooldown(pEvent))
						{
							bubbleIcon.gameObject.SetActive(true);
							bubbleIcon.spriteName = pEvent.monsterElement.ToString().ToLower() + "cakeeventlive";
							bubbleIcon.MakePixelPerfect();
							return;
						}
					}
				}
			}
			if (MSEnhancementManager.instance.enhancementMonster == null ||
			    MSEnhancementManager.instance.enhancementMonster.monster.monsterId == 0) {

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

}
