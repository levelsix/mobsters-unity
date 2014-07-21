using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSBuilding))]
public class MSTeamCenter : MSBuildingFrame {

	Animator controller;

	void OnEnable()
	{
		CheckTag();
		MSActionManager.Goon.OnTeamChanged += OnTeamChange;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnTeamChanged -= OnTeamChange;
	}

	public void Init(Animator controller)
	{
		this.controller = controller;
	}

	public override void CheckTag(){
		if(bubbleIcon != null){
			bubbleIcon.gameObject.SetActive(true);
			int count = 0;
			foreach (var item in MSMonsterManager.instance.userTeam) 
			{
				if (item != null && item.userMonster != null && item.userMonster.userMonsterId > 0)
				{
					count++;
				}
			}
			bubbleIcon.spriteName = "teambubble" + count;
			bubbleIcon.MakePixelPerfect();
		}
	}

	void OnTeamChange()
	{
		int members = 0;
		foreach (var item in MSMonsterManager.instance.userTeam) 
		{
			if (item != null && item.monster != null && item.monster.monsterId > 0)
			{
				members++;
			}
		}

		controller.SetInteger("Members", members);
		CheckTag();
	}
}
