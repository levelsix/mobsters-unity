using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSMiniGoonBox : MonoBehaviour {

	[SerializeField]
	public UI2DSprite goonPortrait;
	
	[SerializeField]
	MSFillBar bar;
	
	[SerializeField]
	UISprite barBG;
	
	[SerializeField]
	public UILabel label;

	[SerializeField]
	public UISprite background;
	
	public PZMonster monster;
	
	[SerializeField]
	public MSActionButton removeButton;

	[HideInInspector]
	public MSUIHelper helper;

	public bool on = false;

	static readonly Vector3 BOTTOM_Y_OFFSET = new Vector3(0, -100, 0);

	Dictionary<Element, string> elementBackgrounds = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightteam"},
		{Element.FIRE, "fireteam"},
		{Element.EARTH, "earthteam"},
		{Element.LIGHT, "lightteam"},
		{Element.WATER, "waterteam"},
		{Element.ROCK, "earthteam"},
		{Element.NO_ELEMENT, "darkteam"}
	};

	const string EMPTY = "teamempty";

	void Awake()
	{
		helper = GetComponent<MSUIHelper>();
	}

	public void Init(MonsterProto monster)
	{
		if (removeButton != null) removeButton.gameObject.SetActive(false);
		if (monster == null)
		{
			goonPortrait.alpha = 0;
			background.spriteName = EMPTY;
		}
		else
		{
			string monsterPrefix = MSUtil.StripExtensions (monster.imagePrefix);
			MSSpriteUtil.instance.SetSprite(monsterPrefix, monsterPrefix + "Thumbnail", goonPortrait);
			background.spriteName = elementBackgrounds[monster.monsterElement];
		}
		gameObject.SetActive(true);
	}

	public void Init(PZMonster monster, bool forTeam = false)
	{
		gameObject.SetActive(true);

		if (monster == null)
		{
			goonPortrait.alpha = 0;
			background.spriteName = EMPTY;
			if (removeButton != null) removeButton.gameObject.SetActive(false);
		}
		else
		{
			string monsterPrefix = MSUtil.StripExtensions (monster.monster.imagePrefix);
			MSSpriteUtil.instance.SetSprite(monsterPrefix, monsterPrefix + "Thumbnail", goonPortrait);
			if (monster.monster.monsterElement == Element.NO_ELEMENT)
			{
				//Debug.Log("What the fuck, " + monster.monster.displayName + " has no element");
				monster.monster.monsterElement = Element.DARK;
			}
			background.spriteName = elementBackgrounds[monster.monster.monsterElement];
			if (removeButton != null) removeButton.gameObject.SetActive(true);
		}

		this.monster = monster;

		if (forTeam)
		{
			if (removeButton != null) removeButton.onClick = RemoveTeam;
		}
		else if (monster != null)
		{
			if (removeButton != null) removeButton.onClick = RemoveQueue;
		}
	}

	public void RemoveTeam()
	{
		MSMonsterManager.instance.RemoveFromTeam(monster);
		on = false;
	}

	public void RemoveQueue()
	{
		//Debug.LogWarning("Removing " + monster.userMonster.userMonsterId);
		if (monster.isHealing)
		{
			MSHospitalManager.instance.RemoveFromHealQueue(monster);
		}
		else if (monster.isEnhancing)
		{
			if (monster == MSMonsterManager.instance.currentEnhancementMonster)
			{
				Debug.Log("Clear enhance queue");
				MSMonsterManager.instance.ClearEnhanceQueue();
			}
			else
			{
				MSMonsterManager.instance.RemoveFromEnhanceQueue(monster);
			}
		}
		else
		{
			if (MSActionManager.Goon.OnMonsterRemoveQueue != null)
			{
				MSActionManager.Goon.OnMonsterRemoveQueue(monster);
			}
		}
		on = false;
	}
	
	void Update()
	{
		if (monster == null)
		{
			if (barBG != null)
			{
				barBG.alpha = 0;
			}
		}
		else if (monster.isHealing)
		{
			if (MSUtil.timeNowMillis < monster.healStartTime)
			{
				barBG.alpha = 0;
				return;
			}
			barBG.alpha = 1;
			if (bar != null) bar.fill = monster.healProgressPercentage;
			label.text = MSUtil.TimeStringShort(monster.healTimeLeftMillis);
		}
		//there is no longer a timer for enhancing
//		else if (monster.isEnhancing)
//		{
//			if (MSUtil.timeNowMillis < monster.enhancement.expectedStartTimeMillis)
//			{
//				barBG.alpha = 0;
//				return;
//			}
//			if (bar != null) bar.fill = monster.enhanceProgress;
//			label.text = MSUtil.TimeStringShort(monster.enhanceTimeLeft);
//		}
		else if (barBG != null)
		{
			barBG.alpha = 0;
		}
	}
}
