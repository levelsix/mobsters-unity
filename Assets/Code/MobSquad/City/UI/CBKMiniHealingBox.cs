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
	public UILabel label;

	[SerializeField]
	public UISprite background;
	
	PZMonster monster;
	
	[SerializeField]
	public CBKActionButton removeButton;

	public bool on = false;

	TweenPosition tweenPos;

	static readonly Vector3 BOTTOM_Y_OFFSET = new Vector3(0, -100, 0);

	Dictionary<MonsterProto.MonsterElement, string> elementBackgrounds = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightteam"},
		{MonsterProto.MonsterElement.FIRE, "fireteam"},
		{MonsterProto.MonsterElement.GRASS, "earthteam"},
		{MonsterProto.MonsterElement.LIGHTNING, "lightteam"},
		{MonsterProto.MonsterElement.WATER, "waterteam"}
	};

	const string EMPTY = "hometeamslotopen";

	void Awake()
	{
		tweenPos = GetComponent<TweenPosition>();
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
			goonPortrait.spriteName = MSUtil.StripExtensions(monster.imagePrefix) + "Card";	
			background.spriteName = elementBackgrounds[monster.monsterElement];
		}
		gameObject.SetActive(true);
	}

	public void Init(PZMonster monster, bool forTeam = false)
	{
		if (monster == null)
		{
			goonPortrait.alpha = 0;
			background.spriteName = EMPTY;
			if (removeButton != null) removeButton.gameObject.SetActive(false);
		}
		else
		{
			goonPortrait.spriteName = MSUtil.StripExtensions(monster.monster.imagePrefix) + "Card";	
			background.spriteName = elementBackgrounds[monster.monster.monsterElement];
			if (removeButton != null) removeButton.gameObject.SetActive(true);
		}

		gameObject.SetActive(true);
		
		this.monster = monster;

		if (forTeam)
		{
			if (removeButton != null) removeButton.onClick = RemoveTeam;
		}
		else if (monster != null)
		{
			if (removeButton != null) removeButton.onClick = RemoveQueue;
			if (!on)
			{
				if (tweenPos != null)
				{
					tweenPos.to = transform.localPosition;
					tweenPos.from = transform.localPosition + BOTTOM_Y_OFFSET;
					tweenPos.ResetToBeginning();
					tweenPos.PlayForward();
				}
				on = true;
			}
		}
	}

	void RemoveTeam()
	{
		MSMonsterManager.instance.RemoveFromTeam(monster);
		on = false;
	}

	void RemoveQueue()
	{
		Debug.LogWarning("Removing " + monster.userMonster.userMonsterId);
		if (monster.isHealing)
		{
			MSHospitalManager.instance.RemoveFromHealQueue(monster);
		}
		else if (monster.isEnhancing)
		{
			if (monster == MSMonsterManager.currentEnhancementMonster)
			{
				Debug.Log("Clear enhance queue");
				MSMonsterManager.instance.ClearEnhanceQueue();
			}
			else
			{
				MSMonsterManager.instance.RemoveFromEnhanceQueue(monster);
			}
		}
		on = false;
	}
	
	void Update()
	{
		if (monster == null)
		{
			if (bar != null) bar.fillAmount = 0;
			return;
		}
		if (monster.isHealing)
		{
			if (bar != null) bar.fillAmount = 1 - monster.healProgressPercentage;
			label.text = MSUtil.TimeStringShort(monster.healTimeLeftMillis);
		}
		else if (monster.isEnhancing)
		{
			if (bar != null) bar.fillAmount = 1 - ((float)monster.enhanceTimeLeft) / ((float)monster.timeToUseEnhance);
			label.text = MSUtil.TimeStringShort(monster.enhanceTimeLeft);
		}
		else if (bar != null) bar.fillAmount = 0;
	}
	
	public void SetBar(bool on)
	{
		if (on)
		{
			label.alpha = 1;
			if (bar != null) bar.alpha = 1;
			barBG.alpha = 1;
		}
		else
		{
			label.alpha = 0;
			if (bar != null) bar.alpha = 0;
			barBG.alpha = 0;
		}
	}
}
