using UnityEngine;
using System.Collections;

public class MSTeamScreen : MonoBehaviour {

	public static MSTeamScreen instance;

	public MSGoonTeamCard[] playerTeam;

	public MSMobsterGrid mobsterGrid;

	[SerializeField]
	UILabel title;

	int currTeammates;

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterAddTeam += OnMobsterAdded;
		MSActionManager.Goon.OnMonsterRemoveTeam += OnMobsterRemoved;
		Init ();
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterAddTeam -= OnMobsterAdded;
		MSActionManager.Goon.OnMonsterRemoveTeam -= OnMobsterRemoved;
	}

	public void Init()
	{
		mobsterGrid.Init(GoonScreenMode.TEAM);
		currTeammates = 0;
		for (int i = 0; i < playerTeam.Length; i++) 
		{
			playerTeam[i].Init(MSMonsterManager.instance.userTeam[i]);
			if (MSMonsterManager.instance.userTeam[i] != null 
			    && MSMonsterManager.instance.userTeam[i].monster != null
			    && MSMonsterManager.instance.userTeam[i].monster.monsterId > 0)
			{
				currTeammates++;
			}
		}
		RefreshTitle(currTeammates);
	}

	void RefreshTitle(int newTeammates)
	{
		title.text = "MY TEAM (" + newTeammates + "/3)";
	}

	void OnMobsterAdded(PZMonster monster)
	{
		playerTeam[monster.userMonster.teamSlotNum-1].Init(monster);
		RefreshTitle(++currTeammates);
	}

	void OnMobsterRemoved(PZMonster monster)
	{
		foreach (var card in playerTeam) 
		{
			if (card.goon == monster)
			{
				card.Init(null);
			}
		}
		RefreshTitle(--currTeammates);
	}
}
