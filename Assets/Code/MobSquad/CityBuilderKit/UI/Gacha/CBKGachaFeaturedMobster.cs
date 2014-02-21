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
		MonsterProto monster = CBKDataManager.instance.Get<MonsterProto>(mobster.monsterId);

		mobsterSprite.sprite2D = CBKAtlasUtil.instance.GetMobsterSprite(monster.imagePrefix);
		mobsterName.text = monster.displayName;

		rarityBg.spriteName = monster.quality.ToString().ToLower() + "gtag";


		rarityName.text = monster.quality.ToString();

		elementSprite.spriteName = monster.monsterElement.ToString().ToLower() + "orb";


		maxHp.text = monster.lvlInfo[monster.maxLevel-1].hp.ToString();

		maxAttack.text = GetMaxDamage(monster).ToString();
	}

	int GetMaxDamage(MonsterProto monster)
	{
		return monster.lvlInfo[monster.maxLevel-1].fireDmg + monster.lvlInfo[monster.maxLevel-1].grassDmg + monster.lvlInfo[monster.maxLevel-1].lightningDmg
			+ monster.lvlInfo[monster.maxLevel-1].darknessDmg + monster.lvlInfo[monster.maxLevel-1].waterDmg + monster.lvlInfo[monster.maxLevel-1].rockDmg;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
