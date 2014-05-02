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

	TweenPosition tweenPos;

	static readonly Vector3 BOTTOM_Y_OFFSET = new Vector3(0, -100, 0);

	Dictionary<MonsterProto.MonsterElement, string> elementBackgrounds = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARK, "nightteam"},
		{MonsterProto.MonsterElement.FIRE, "fireteam"},
		{MonsterProto.MonsterElement.GRASS, "earthteam"},
		{MonsterProto.MonsterElement.LIGHT, "lightteam"},
		{MonsterProto.MonsterElement.WATER, "waterteam"}
	};

	const string EMPTY = "hometeamslotopen";

	void Awake()
	{
		tweenPos = GetComponent<TweenPosition>();
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
			StartCoroutine(MSAtlasUtil.instance.SetSprite(monsterPrefix, monsterPrefix + "Thumbnail", goonPortrait));
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
			StartCoroutine(MSAtlasUtil.instance.SetSprite(monsterPrefix, monsterPrefix + "Thumbnail", goonPortrait));
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
			barBG.alpha = 0;
		}
		if (monster.isHealing)
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
		else if (monster.isEnhancing)
		{
			if (MSUtil.timeNowMillis < monster.enhancement.expectedStartTimeMillis)
			{
				barBG.alpha = 0;
				return;
			}
			if (bar != null) bar.fill = 1 - ((float)monster.enhanceTimeLeft) / ((float)monster.timeToUseEnhance);
			label.text = MSUtil.TimeStringShort(monster.enhanceTimeLeft);
		}
		else if (bar != null) bar.fill = 0;
	}
}
