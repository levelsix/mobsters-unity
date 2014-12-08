using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSMobsterIcon
/// @author Rob Giusti
/// </summary>
public class MSMobsterIcon : MonoBehaviour 
{
	[SerializeField] UISprite bgSprite;
	[SerializeField] UI2DSprite thumb;

	const string emptySprite = "teamempty";

	public void Init(PZMonster monster)
	{
		Init (monster.monster);
	}

	public void Init(MinimumUserMonsterProto monster)
	{
		Init (MSDataManager.instance.Get<MonsterProto>(monster.monsterId));
	}

	public void Init(MonsterProto monster)
	{
		if (monster == null)
		{
			bgSprite.spriteName = emptySprite;
			thumb.alpha = 0;
		}
		else
		{
			bgSprite.spriteName = MSGoonCard.smallBackgrounds[monster.monsterElement];
			MSSpriteUtil.instance.SetSprite(monster.imagePrefix, monster.imagePrefix + "Thumbnail", thumb);
		}
	}

	public void Init(PvpMonsterProto monster)
	{
		Init (monster.defenderMonster);
	}
}
