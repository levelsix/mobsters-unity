using UnityEngine;
using System.Collections;

public class CBKMiniHealingBox : MonoBehaviour {

	[SerializeField]
	UISprite goonPortrait;
	
	[SerializeField]
	UISprite bar;
	
	[SerializeField]
	UISprite barBG;
	
	[SerializeField]
	UILabel timeLabel;
	
	PZMonster monster;
	
	[SerializeField]
	CBKActionButton removeButton;
	
	void OnEnable()
	{
		removeButton.onClick += Remove;
	}
	
	void OnDisable()
	{
		removeButton.onClick -= Remove;
	}
	
	public void Init(PZMonster monster)
	{
		gameObject.SetActive(true);
		
		this.monster = monster;
		
		goonPortrait.spriteName = CBKAtlasUtil.instance.StripExtensions(monster.monster.imagePrefix) + "Card";
	}
	
	void Remove()
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
