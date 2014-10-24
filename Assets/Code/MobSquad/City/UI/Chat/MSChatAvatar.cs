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

	[SerializeField]
	bool square = false;

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

	/// <summary>
	/// The square sprites.  These are in menuRef
	/// </summary>
	Dictionary<Element, string> squareSprites = new Dictionary<Element, string>()
	{
		{Element.FIRE, "fireteam"},
		{Element.EARTH, "earthteam"},
		{Element.DARK, "nightteam"},
		{Element.WATER, "waterteam"},
		{Element.LIGHT, "lightteam"}
	};

	public void Init(int monsterId)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(monsterId);
		Init(monster);
	}

	public void Init(MonsterProto monster)
	{
		if(monster != null)
		{
			if(!square)
			{
				bg.spriteName = big ? bigBgSprites[monster.monsterElement] : bgSprites[monster.monsterElement];
			}
			else
			{
				bg.spriteName = squareSprites[monster.monsterElement];
			}
			MSSpriteUtil.instance.SetSprite(monster.imagePrefix, monster.imagePrefix + "Thumbnail", thumbnail);
		}
	}

	public void SetDepth(int depth)
	{
		if(bg != null)
		{
			bg.depth = depth;
		}
		thumbnail.depth = depth + 1;
	}
}
