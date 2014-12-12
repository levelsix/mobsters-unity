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

	[SerializeField]
	GameObject loadingIcon;
	
	const string gemCasePath = "Sprites/Misc/casegems";
	
	MSLoopingElement looper;

	BoosterItemProto boosterItem;

	void Awake()
	{
		looper = GetComponent<MSLoopingElement>();
		if (looper != null) looper.onLoop = OnLoop;
	}

	void OnEnable()
	{
		if (boosterItem != null && boosterItem.boosterItemId > 0)
		{
			Init(boosterItem);
		}
	}
	
	public void Init(BoosterItemProto mobster)
	{

		if (gameObject.activeInHierarchy)
		{
			boosterItem = null;
			StopAllCoroutines();
			loadingIcon.SetActive(true);
			if (mobster.monsterId > 0)
			{
				MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(mobster.monsterId);
				
				StartCoroutine(MSSpriteUtil.instance.SetSpriteCoroutine(monster.imagePrefix, monster.imagePrefix + "Character", mobsterSprite, 1, delegate{loadingIcon.SetActive(false);}));
				
				mobsterName.text = monster.displayName;
				
				rarityBg.spriteName = "battle" + monster.quality.ToString().ToLower() + "tag";
				rarityBg.MakePixelPerfect();

				if(monster.monsterElement == Element.DARK)
				{
					elementSprite.spriteName = "nightorb";
				}
				else
				{
					elementSprite.spriteName = monster.monsterElement.ToString().ToLower() + "orb";
				}

				elementName.text = monster.monsterElement.ToString();
				elementName.color = MSColors.elementColors[monster.monsterElement];
				elementSprite.MakePixelPerfect();
				
				maxHp.text = MSMath.MaxHPAtLevel(monster, monster.maxLevel).ToString("n0");
				maxSpeed.text = MSMath.SpeedAtLevel(monster, monster.maxLevel).ToString("n0");
				maxAttack.text = MSMath.AttackAtLevel(monster, monster.maxLevel).ToString("#,##0");
			}
			else
			{
				mobsterName.text = "GEMS!";
			}
		}
		else
		{
			boosterItem = mobster;
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