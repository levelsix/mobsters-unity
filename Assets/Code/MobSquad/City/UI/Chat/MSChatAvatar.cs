using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSChatAvatar : MonoBehaviour {

	[SerializeField]
	UISprite bg;

	[SerializeField]
	UI2DSprite thumbnail;

	Dictionary<Element, string> bgSprites = new Dictionary<Element, string>()
	{
		{Element.FIRE, "fireavatar"},
		{Element.EARTH, "earthavatar"},
		{Element.DARK, "nightavatar"},
		{Element.WATER, "wateravatar"},
		{Element.LIGHT, "lightavatar"}
	};

	public void Init(int monsterId)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(monsterId);
		bg.spriteName = bgSprites[monster.monsterElement];
		MSSpriteUtil.instance.SetSprite(monster.imagePrefix, monster.imagePrefix + "Thumbnail", thumbnail);
	}
}
