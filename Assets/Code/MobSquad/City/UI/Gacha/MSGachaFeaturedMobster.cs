using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGachaFeaturedMobster : MonoBehaviour {
	
	[SerializeField]
	MSGachaScreen gachaScreen;
	
	public MSGachaReveal gachaReveal;
	
	[SerializeField]
	UI2DSprite mobsterSprite;
	
	[SerializeField]
	UILabel mobsterName;
	
	[SerializeField]
	UISprite rarityBg;
	
	[SerializeField]
	UISprite elementSprite;
	
	[SerializeField]
	UILabel elementName;
	
	[SerializeField]
	UILabel maxHp;
	
	[SerializeField]
	UILabel maxSpeed;
	
	[SerializeField]
	UILabel maxAttack;
	
	const string gemCasePath = "Sprites/Misc/casegems";
	
	MSLoopingElement looper;
	
	void Awake()
	{
		looper = GetComponent<MSLoopingElement>();
		if (looper != null) looper.onLoop = OnLoop;
	}
	
	public void Init(BoosterItemProto mobster)
	{
		if (mobster.monsterId > 0)
		{
			
			MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(mobster.monsterId);
			
			MSSpriteUtil.instance.SetSprite(monster.imagePrefix, monster.imagePrefix + "Character", mobsterSprite);
			
			mobsterName.text = monster.displayName;
			
			rarityBg.spriteName = "battle" + monster.quality.ToString().ToLower() + "tag";
			rarityBg.MakePixelPerfect();
			
			elementSprite.spriteName = monster.monsterElement.ToString().ToLower() + "orb";
			elementName.text = monster.monsterElement.ToString();
			elementName.color = MSColors.elementColors[monster.monsterElement];
			elementSprite.MakePixelPerfect();
			
			maxHp.text = MSMath.MaxHPAtLevel(monster, monster.maxLevel).ToString("n0");
			
			maxAttack.text = MSMath.AttackAtLevel(monster, monster.maxLevel).ToString("#,##0");
		}
		else
		{
			mobsterName.text = "GEMS!";
		}
	}
	
	void OnLoop(bool left)
	{
		if (left)
		{
			if (gachaScreen != null)
			{
				Init(gachaScreen.PickGoonLeft());
			}
			else
			{
				Init(gachaReveal.PickGoonLeft());
			}
		}
		else
		{
			if (gachaScreen != null)
			{
				Init(gachaScreen.PickGoonRight());
			}
			else
			{
				Init(gachaReveal.PickGoonRight());
			}
		}
	}
}