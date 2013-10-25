using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK monster manager.
/// Keeps track of what monsters are healing, what monsters are 
/// </summary>
public class CBKMonsterManager : MonoBehaviour {
	
	public Dictionary<long, PZMonster> userMonsters = new Dictionary<long, PZMonster>();
	
	public List<PZMonster> userTeam = new List<PZMonster>();
	
	public static CBKMonsterManager instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void Init(List<FullUserMonsterProto> monsters)
	{
		PZMonster mon;
		foreach (FullUserMonsterProto item in monsters) 
		{
			mon = new PZMonster(item);
			Debug.Log("Adding monster " + item.monsterId);
			userMonsters.Add(item.userMonsterId, mon);
			if (item.teamSlotNum > 0)
			{
				userTeam.Add(mon);
			}
		}
		userTeam.Sort(new TeamSorter());
	}
	
	public void UpdateOrAddAll(List<FullUserMonsterProto> monsters)
	{
		foreach (FullUserMonsterProto item in monsters) 
		{
			UpdateOrAdd(item);
		}
	}
	
	public void UpdateOrAdd(FullUserMonsterProto monster)
	{
		if (userMonsters.ContainsKey(monster.userMonsterId))
		{
			Debug.Log("Updating monster: " + monster.userMonsterId);
			userMonsters[monster.userMonsterId].UpdateUserMonster(monster);
		}
		else
		{
			Debug.Log("Adding monster: " + monster.monsterId);
			userMonsters.Add(monster.userMonsterId, new PZMonster(monster));
		}
	}
	
	private class TeamSorter : Comparer<PZMonster>
	{
		public override int Compare(PZMonster x, PZMonster y)
		{
			return -1 * x.userMonster.teamSlotNum.CompareTo(y.userMonster.teamSlotNum);
		}
	}			
}
					

