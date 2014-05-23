using UnityEngine;
using com.lvl6.proto;
using System.Collections;

public class PZMonsterIntro : MonoBehaviour {

	[SerializeField]
	UILabel currNumber;

	[SerializeField]
	UILabel totalNumber;

	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	UILabel name;

	[SerializeField]
	UILabel level;

	[SerializeField]
	TweenPosition top;

	[SerializeField]
	TweenPosition bottom;

	public void SetText(PZMonster monster, int currUnitIndex, int totalUnits){
		currNumber.text = currUnitIndex.ToString ();
		totalNumber.text = totalUnits.ToString ();

		switch (monster.monster.monsterElement) {
		case Element.DARK:
			elementSprite.spriteName = "nightorb";
			break;
		case Element.EARTH:
			elementSprite.spriteName = "earthorb";
			break;
		case Element.FIRE:
			elementSprite.spriteName = "fireorb";
			break;
		case Element.LIGHT:
			elementSprite.spriteName = "lightorb";
			break;
		case Element.ROCK:
			elementSprite.spriteName = "rockorb";
			break;
		case Element.WATER:
			elementSprite.spriteName = "waterorb";
			break;
		}
		
		name.text = monster.monster.displayName;

		int levelNum = monster.taskMonster.level;
		level.text = levelNum.ToString();

		//these MAY have to be marked as changed
		currNumber.MarkAsChanged ();
		totalNumber.MarkAsChanged ();
		elementSprite.MarkAsChanged ();
		name.MarkAsChanged ();
		level.MarkAsChanged ();
	}

	public void PlayAnimation(){
		top.ResetToBeginning ();
		top.PlayForward ();
		bottom.ResetToBeginning ();
		bottom.PlayForward ();
	}
}
