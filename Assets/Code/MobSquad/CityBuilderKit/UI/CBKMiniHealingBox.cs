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

	void Awake()
	{
		tweenPos = GetComponent<TweenPosition>();
	}

	public void Init(PZMonster monster, bool forTeam = false)
	{
		if (monster == null)
		{
			goonPortrait.alpha = 0;
			removeButton.gameObject.SetActive(false);
		}
		else
		{
			goonPortrait.spriteName = MSUtil.StripExtensions(monster.monster.imagePrefix) + "Card";	
			background.spriteName = elementBackgrounds[monster.monster.monsterElement];
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
			bar.fillAmount = 0;
			return;
		}
		if (monster.isHealing)
		{
			bar.fillAmount = 1 - monster.healProgressPercentage;
			timeLabel.text = MSUtil.TimeStringShort(monster.healTimeLeftMillis);
		}
		else if (monster.isEnhancing)
		{
			bar.fillAmount = 1 - ((float)monster.enhanceTimeLeft) / ((float)monster.timeToUseEnhance);
			timeLabel.text = MSUtil.TimeStringShort(monster.enhanceTimeLeft);
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
