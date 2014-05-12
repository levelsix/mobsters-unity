using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSGachaReveal : MonoBehaviour {

	[SerializeField]
	UI2DSprite goonSprite;

	[SerializeField]
	UILabel goonName;

	[SerializeField]
	UILabel description;

	[SerializeField]
	UISprite rarityTag;

	[SerializeField]
	UILabel raritylabel;

	[SerializeField]
	UILabel piecesLabel;

	const string gemCasePath = "Sprites/Misc/casegems";

	public void Init(BoosterItemProto prize)
	{
		if (prize.monsterId > 0)
		{
			MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(prize.monsterId);
			goonSprite.sprite2D = MSSpriteUtil.instance.GetMobsterSprite(monster.imagePrefix);
			description.text = monster.description;
			rarityTag.spriteName = monster.quality.ToString().ToLower() + "gtag";
			raritylabel.text = monster.quality.ToString();
			piecesLabel.text = "Pieces: " + prize.numPieces + "/" + monster.numPuzzlePieces;
		}
		else
		{
			goonSprite.sprite2D = MSSpriteUtil.instance.GetSprite(gemCasePath);
			description.text = prize.gemReward + " gems";
			rarityTag.spriteName = "";
			raritylabel.text = "";
			piecesLabel.text = "";
		}
	}
}
