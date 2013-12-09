using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKMiniHealingBox : MonoBehaviour {

	[SerializeField]
	public UISprite goonPortrait;
	
	[SerializeField]
	UISprite bar;
	
	[SerializeField]
	UISprite barBG;
	
	[SerializeField]
	UILabel timeLabel;

	[SerializeField]
	public UISprite background;
	
	PZMonster monster;
	
	[SerializeField]
	public CBKActionButton removeButton;

	Dictionary<MonsterProto.MonsterElement, string> elementBackgrounds = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightteam"},
		{MonsterProto.MonsterElement.FIRE, "fireteam"},
		{MonsterProto.MonsterElement.GRASS, "earthteam"},
		{MonsterProto.MonsterElement.LIGHTNING, "lightteam"},
		{MonsterProto.MonsterElement.WATER, "waterteam"}
	};

	public void Init(PZMonster monster, bool forTeam = false)
	{
		if (monster == null)
		{
			goonPortrait.alpha = 0;
			removeButton.gameObject.SetActive(false);
		}
		else
		{
			goonPortrait.spriteName = CBKUtil.StripExtensions(monster.monster.imagePrefix) + "Card";	
			background.spriteName = elementBackgrounds[monster.monster.element];
			removeButton.gameObject.SetActive(true);
		}

		gameObject.SetActive(true);
		
		this.monster = monster;
		

		if (forTeam)
		{
			removeButton.onClick = RemoveTeam;
		}
		else if (monster != null)
		{
			removeButton.onClick = RemoveQueue;
		}
	}

	void RemoveTeam()
	{
		CBKMonsterManager.instance.RemoveFromTeam(monster);
	}

	void RemoveQueue()
	{
		if (monster.isHealing)
		{
			CBKMonsterManager.instance.RemoveFromHealQueue(monster);
		}
		else if (monster.isEnhancing)
		{
			if (monster == CBKMonsterManager.currentEnhancementMonster)
			{
				Debug.Log("Clear enhance queue");
				CBKMonsterManager.instance.ClearEnhanceQueue();
			}
			else
			{
				CBKMonsterManager.instance.RemoveFromEnhanceQueue(monster);
			}
		}
	}
	
	void Update()
	{
		if (monster == null)
		{
			bar.fillAmount = 0;
			return;
		}
		if (monster.isHealing)
		{
			bar.fillAmount = ((float)monster.healTimeLeftMillis) / ((float)monster.timeToHealMillis);
			timeLabel.text = CBKUtil.TimeStringShort(monster.healTimeLeftMillis);
		}
		else if (monster.isEnhancing)
		{
			bar.fillAmount = ((float)monster.enhanceTimeLeft) / ((float)monster.timeToUseEnhance);
			timeLabel.text = CBKUtil.TimeStringShort(monster.enhanceTimeLeft);
		}
		else bar.fillAmount = 0;
	}
	
	public void SetBar(bool on)
	{
		if (on)
		{
			timeLabel.alpha = 1;
			bar.alpha = 1;
			barBG.alpha = 1;
		}
		else
		{
			timeLabel.alpha = 0;
			bar.alpha = 0;
			barBG.alpha = 0;
		}
	}
}
