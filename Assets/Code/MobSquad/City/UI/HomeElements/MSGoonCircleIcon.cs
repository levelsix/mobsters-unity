using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGoonCircleIcon : MonoBehaviour {

	[SerializeField]
	UILabel goonName;

	[SerializeField]
	UISprite barBg;

	[SerializeField]
	MSFillBar hpbar;
	
	[SerializeField]
	UI2DSprite icon;
	
	[SerializeField]
	UISprite background;

	[SerializeField]
	int index;

	PZMonster monster;

	static readonly Dictionary<Element, string> backgroundElementDict = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightteam"},
		{Element.FIRE, "fireteam"},
		{Element.EARTH, "earthteam"},
		{Element.LIGHT, "lightteam"},
		{Element.WATER, "waterteam"}
	};

	const string emptyBackground = "teamempty";
	const string fullBackground = "memberbg";

	void Awake()
	{
		MSActionManager.Scene.OnCity += OnCity;
	}

	void OnDestroy()
	{
		MSActionManager.Scene.OnCity -= OnCity;
	}

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterFinishHeal += OnMobsterHealed;
		MSActionManager.Goon.OnMonsterAddTeam += OnMobsterAddTeam;
		MSActionManager.Goon.OnMonsterRemoveTeam += OnMobsterRemoveTeam;
	}

	void OnDisable()
	{	
		MSActionManager.Goon.OnMonsterFinishHeal -= OnMobsterHealed;
		MSActionManager.Goon.OnMonsterAddTeam -= OnMobsterAddTeam;
		MSActionManager.Goon.OnMonsterRemoveTeam -= OnMobsterRemoveTeam;
	}
	
	public void Init(PZMonster monster)
	{
		this.monster = monster;

		if (monster == null || monster.monster == null || monster.monster.monsterId == 0)
		{
			goonName.text = "Slot Empty";
			background.spriteName = emptyBackground;
			barBg.alpha = 0;
			icon.alpha = 0;
		}
		else
		{
			goonName.text = monster.monster.displayName;
			background.spriteName = backgroundElementDict[monster.monster.monsterElement];
			icon.alpha = 1;
			barBg.alpha = 1;

			string mobsterPrefix = MSUtil.StripExtensions (monster.monster.imagePrefix);
			MSSpriteUtil.instance.SetSprite(mobsterPrefix, mobsterPrefix + "Card", icon);
			
			hpbar.fill = ((float)monster.currHP) / monster.maxHP;
		}
	}

	void OnCity()
	{
		Init(monster);
	}

	void OnMobsterHealed(PZMonster monster)
	{
		if (this.monster == monster)
		{
			hpbar.fill = ((float)monster.currHP) / monster.maxHP;
		}
	}

	void OnMobsterAddTeam(PZMonster monster)
	{
		if (monster.userMonster.teamSlotNum == index)
		{
			Init (monster);
		}
	}

	void OnMobsterRemoveTeam(PZMonster monster)
	{
		if (this.monster == monster)
		{
			Init (null);
		}
	}
}
