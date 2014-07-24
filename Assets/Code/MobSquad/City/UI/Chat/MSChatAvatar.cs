using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSChatAvatar : MonoBehaviour {

	[SerializeField]
	UISprite bg;

	[SerializeField]
	UI2DSprite thumbnail;

	[SerializeField]
	bool big = false;

	public float alpha
	{
		set
		{
			bg.alpha = value;
		}
	}

	Dictionary<Element, string> bgSprites = new Dictionary<Element, string>()
	{
		{Element.FIRE, "fireavatar"},
		{Element.EARTH, "earthavatar"},
		{Element.DARK, "nightavatar"},
		{Element.WATER, "wateravatar"},
		{Element.LIGHT, "lightavatar"}
	};

	Dictionary<Element, string> bigBgSprites = new Dictionary<Element, string>()
	{
		{Element.FIRE, "firebigavatar"},
		{Element.EARTH, "earthbigavatar"},
		{Element.DARK, "nightbigavatar"},
		{Element.WATER, "waterbigavatar"},
		{Element.LIGHT, "lightbigavatar"}
	};

	public void Init(int monsterId)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(monsterId);
		bg.spriteName = big ? bigBgSprites[monster.monsterElement] : bgSprites[monster.monsterElement];
		MSSpriteUtil.instance.SetSprite(monster.imagePrefix, monster.imagePrefix + "Thumbnail", thumbnail);
	}
}
