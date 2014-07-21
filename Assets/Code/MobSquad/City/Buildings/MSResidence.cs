using UnityEngine;
using System.Collections;

public class MSResidence : MSBuildingFrame {

	void OnEnable(){
		if(bubbleIcon != null)
		{
			FirstFrameCheck ();
		}
		MSActionManager.Goon.OnMonsterListChanged += CheckTag;
	}

	public override void CheckTag(){
		if (MSMonsterManager.monstersOwned > MSMonsterManager.instance.totalResidenceSlots) {
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
		} else {
			bubbleIcon.gameObject.SetActive(false);
		}
	}

	void OnDisable(){
		MSActionManager.Goon.OnMonsterListChanged -= CheckTag;
	}
}
