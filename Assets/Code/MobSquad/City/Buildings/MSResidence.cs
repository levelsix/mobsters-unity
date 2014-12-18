using UnityEngine;
using System.Collections;

public class MSResidence : MSBuildingFrame {

	void OnEnable(){
		MSActionManager.Goon.OnMonsterListChanged += CheckTag;
		MSActionManager.Gacha.OnPurchaseBoosterSucces += CheckTag;
		FirstFrameCheck();
	}

	public override void CheckTag(){
		bubbleIcon.gameObject.SetActive(false);

		if (MSMonsterManager.instance.userMonsters.Count > MSMonsterManager.instance.totalResidenceSlots && Precheck()) {
			bubbleIcon.gameObject.SetActive(true);
			if(MSMonsterManager.instance.userMonsters.Count - MSMonsterManager.instance.totalResidenceSlots <= 9)
			{
				bubbleIcon.spriteName = "sellbubble" + (MSMonsterManager.instance.userMonsters.Count - MSMonsterManager.instance.totalResidenceSlots);
			}
			else
			{
				bubbleIcon.spriteName = "sellbubbleexclamation";
			}
			bubbleIcon.MakePixelPerfect();
		}
	}

	void OnDisable(){
		MSActionManager.Goon.OnMonsterListChanged -= CheckTag;
	}
}
