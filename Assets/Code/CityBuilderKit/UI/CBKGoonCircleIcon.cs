using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKGoonCircleIcon : MonoBehaviour {

	[SerializeField]
	UISprite ring;
	
	[SerializeField]
	UISprite icon;
	
	[SerializeField]
	UISprite background;
	
	static readonly Dictionary<MonsterProto.MonsterElement, string> ringElementDict = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightring"},
		{MonsterProto.MonsterElement.FIRE, "firering"},
		{MonsterProto.MonsterElement.GRASS, "earthring"},
		{MonsterProto.MonsterElement.LIGHTNING, "lightring"},
		{MonsterProto.MonsterElement.WATER, "waterring"}
	};
	
	const string emptyBackground = "emptyring";
	const string fullBackground = "memberbg";
	
	public void Init(PZMonster monster)
	{
		if (monster == null)
		{
			background.spriteName = emptyBackground;
			ring.fillAmount = 0;
			icon.alpha = 0;
		}
		else
		{
			background.spriteName = fullBackground;
			icon.alpha = 1;
			
			icon.spriteName = CBKAtlasUtil.instance.StripExtensions(monster.monster.imagePrefix) + "Icon";
			
			ring.fillAmount = ((float)monster.currHP) / monster.maxHP;
			ring.spriteName = ringElementDict[monster.monster.element];
		}
	}
}
