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

	[SerializeField]
	TweenAlpha tintAlph;

	[SerializeField]
	UISprite rarityTag;

	TweenColor topColor;

	PZMonster curMonster;

	void Awake()
	{
		topColor = topLabel.GetComponent<TweenColor>();
		topPos.AddOnFinished(delegate{Reset();});
	}

	public void Init(PZMonster monster, int currUnitIndex, int totalUnits){

		curMonster = monster;

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
		topLabel.MakePixelPerfect();
		topLabel.MarkAsChanged();

		if(monster.monster.shorterName.Length > 0 && monster.monster.shorterName != "NULL")
		{
			bottomLabel.text = monster.monster.shorterName;
		}
		else
		{
			bottomLabel.text = monster.monster.displayName;
		}
		bottomLabel.MakePixelPerfect();
		bottomLabel.MarkAsChanged();

		level.text = "L" + monster.level;

		level.MakePixelPerfect();
		level.MarkAsChanged();

		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Thumbnail", thumbNail);

		if (curMonster.taskMonster == null)
		{
			rarityTag.spriteName = "";
		}
		else if(curMonster.taskMonster.monsterType == TaskStageMonsterProto.MonsterType.BOSS)
		{
			topLabel.text = "BOSS";

			topLabel.MakePixelPerfect();
			topLabel.MarkAsChanged();

			topColor.ResetToBeginning();
			topColor.PlayForward();

			rarityTag.spriteName = "battle" + curMonster.monster.quality.ToString().ToLower() + "tag";
		}
		else if(curMonster.taskMonster.monsterType == TaskStageMonsterProto.MonsterType.MINI_BOSS)
		{
			topLabel.text = "Mini Boss";

			topLabel.MakePixelPerfect();
			topLabel.MarkAsChanged();

			topColor.ResetToBeginning();
			topColor.PlayForward();

			rarityTag.spriteName = "battle" + curMonster.monster.quality.ToString().ToLower() + "tag";
		}
		else
		{
			rarityTag.spriteName = "";
		}
	}

	public void PlayAnimation(){
		tintAlph.ResetToBeginning();
		tintAlph.PlayForward();
		topPos.ResetToBeginning ();
		topPos.PlayForward ();
		topAlph.ResetToBeginning();
		topAlph.PlayForward();
		bottomPos.ResetToBeginning ();
		bottomPos.PlayForward();
		bottomAlph.ResetToBeginning();
		bottomAlph.PlayForward();
	}

	/// <summary>
	/// Resets anything that needs to be cleared before the next enemy
	/// </summary>
	void Reset()
	{
		topColor.Sample(0f, false);
		topColor.enabled = false;
	}
}
