using UnityEngine;
using com.lvl6.proto;
using System.Collections;

public class PZMonsterIntro : MonoBehaviour {

	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	UILabel topLabel;

	[SerializeField]
	UI2DSprite thumbNail;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	UILabel level;

	[SerializeField]
	TweenPosition topPos;

	[SerializeField]
	TweenPosition bottomPos;

	[SerializeField]
	TweenAlpha topAlph;

	[SerializeField]
	TweenAlpha bottomAlph;

	public void Init(PZMonster monster, int currUnitIndex, int totalUnits){

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
		elementSprite.MakePixelPerfect();

		topLabel.text = "Enemy " + currUnitIndex + "/" + totalUnits;
		bottomLabel.text = monster.monster.shorterName;
		level.text = "L" + monster.level;

		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Thumbnail", thumbNail);

		//these MAY have to be marked as changed
		topLabel.MarkAsChanged();
		bottomLabel.MarkAsChanged();
	}

	public void PlayAnimation(){
		topPos.ResetToBeginning ();
		topPos.PlayForward ();
		topAlph.ResetToBeginning();
		topAlph.PlayForward();
		bottomPos.ResetToBeginning ();
		bottomPos.PlayForward();
		bottomAlph.ResetToBeginning();
		bottomAlph.PlayForward();
	}
}
