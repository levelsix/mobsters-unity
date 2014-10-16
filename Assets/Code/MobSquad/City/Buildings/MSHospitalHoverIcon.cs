using UnityEngine;
using System.Collections;

public class MSHospitalHoverIcon : MSBuildingFrame {

	void OnEnable()
	{
		MSActionManager.Goon.OnHealQueueChanged += CheckTag;
		FirstFrameCheck();
	}

	public override void CheckTag()
	{
		bubbleIcon.gameObject.SetActive(false);
		if (building.hospital.goon == null && Precheck())
		{
			int monstersNeedHealing = 0;
			foreach (PZMonster monster in MSMonsterManager.instance.userMonsters)
			{
				if(monster.totalHealthToHeal > 0)
				{
					monstersNeedHealing++;
				}
			}
						
			if(monstersNeedHealing >= 1)
			{
				if(monstersNeedHealing > 9)
				{
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

	void OnDisable()
	{
		MSActionManager.Goon.OnHealQueueChanged -= CheckTag;
	}
}
