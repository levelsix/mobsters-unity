using UnityEngine;
using System.Collections;

public class MSResidence : MSBuildingFrame {

	void OnEnable(){
		MSActionManager.Goon.OnMonsterListChanged += CheckTag;
		FirstFrameCheck();
	}

	public override void CheckTag(){
		bubbleIcon.gameObject.SetActive(false);

		if (MSMonsterManager.monstersOwned > MSMonsterManager.instance.totalResidenceSlots && Precheck()) {
			bubbleIcon.gameObject.SetActive(true);

			if(MSMonsterManager.monstersOwned - MSMonsterManager.instance.totalResidenceSlots <= 9)
			{
				bubbleIcon.spriteName = "sellbubble" + (MSMonsterManager.monstersOwned - MSMonsterManager.instance.totalResidenceSlots);
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
