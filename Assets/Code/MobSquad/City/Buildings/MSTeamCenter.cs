using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSBuilding))]
public class MSTeamCenter : MSBuildingFrame {

	Animator controller;

	private readonly Vector3 OFFSET = new Vector3(0f, 3.25f, 0f);

	void Awake()
	{
		base.Awake();
	}

	void OnEnable()
	{
		MSActionManager.Goon.OnTeamChanged += OnTeamChange;
		MSActionManager.Scene.OnCity += OnTeamChange;
		FirstFrameCheck();
		OnTeamChange();
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnTeamChanged -= OnTeamChange;
		MSActionManager.Scene.OnCity -= OnTeamChange;
	}

	void Update()
	{
		bubbleIcon.transform.localPosition = OFFSET;
		bubbleIcon.MarkAsChanged();
	}

	public void Init(Animator controller)
	{
		this.controller = controller;
	}

	public override void CheckTag(){
		bubbleIcon.gameObject.SetActive(false);
		if(Precheck())
		{
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
