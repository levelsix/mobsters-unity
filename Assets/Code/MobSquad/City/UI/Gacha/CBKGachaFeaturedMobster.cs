using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKGachaFeaturedMobster : MonoBehaviour {

	[SerializeField]
	UI2DSprite mobsterSprite;

	[SerializeField]
	UILabel mobsterName;

	[SerializeField]
	UISprite rarityBg;

	[SerializeField]
	UILabel rarityName;

	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	UILabel maxHp;

	[SerializeField]
	UILabel maxAttack;

	public CBKLoopingElement looper;

	void Awake()
	{
		looper = GetComponent<CBKLoopingElement>();
	}

	public void Init(BoosterItemProto mobster)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(mobster.monsterId);

		mobsterSprite.sprite2D = MSAtlasUtil.instance.GetMobsterSprite(monster.imagePrefix);
		mobsterName.text = monster.displayName;

		rarityBg.spriteName = monster.quality.ToString().ToLower() + "gtag";


		rarityName.text = monster.quality.ToString();

		elementSprite.spriteName = monster.monsterElement.ToString().ToLower() + "orb";


		maxHp.text = monster.lvlInfo.Find(x=>x.lvl == monster.maxLevel).hp.ToString();

		maxAttack.text = GetMaxDamage(monster).ToString();
	}

	int GetMaxDamage(MonsterProto monster)
	{
		MonsterLevelInfoProto levelInfo = monster.lvlInfo.Find(x=>x.lvl == monster.maxLevel);
		return levelInfo.fireDmg + levelInfo.grassDmg + levelInfo.lightningDmg
			+ levelInfo.darknessDmg + levelInfo.waterDmg + levelInfo.rockDmg;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
