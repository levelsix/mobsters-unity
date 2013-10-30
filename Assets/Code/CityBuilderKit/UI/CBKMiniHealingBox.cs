using UnityEngine;
using System.Collections;

public class CBKMiniHealingBox : MonoBehaviour {

	[SerializeField]
	UISprite goonPortrait;
	
	[SerializeField]
	UISprite bar;
	
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
		
	}
	
	void Remove()
	{
		CBKMonsterManager.instance.RemoveFromHealQueue(monster);
	}
	
	void Update()
	{
		if (CBKMonsterManager.instance.healingMonsters[0] == monster)
		{
			timeLabel.text = CBKUtil.TimeStringShort(monster.healTimeLeft);
			timeLabel.alpha = 1;
			bar.fillAmount = ((float)monster.timeToHealMillis) / ((float)monster.finishHealTimeMillis);
			bar.alpha = 0;
		}
		else
		{
			timeLabel.alpha = 0;
			bar.alpha = 0;
		}
	}
}
