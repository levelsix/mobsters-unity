using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSBuilding))]
public class MSTeamCenter : MonoBehaviour {

	Animator controller;

	void OnEnable()
	{
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
	}
}
