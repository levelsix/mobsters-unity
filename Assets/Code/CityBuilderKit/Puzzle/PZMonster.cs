using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System;

[System.Serializable]
/// <summary>
/// @author Rob Giusti
/// Data structure for containing and managing a FullUserMonsterProto and MonsterProto
/// together. 
/// </summary>
public class PZMonster {
	
	public MonsterProto monster;
	
	public FullUserMonsterProto userMonster;
	
	public TaskStageMonsterProto taskMonster;
	
	public UserMonsterHealingProto healingMonster = null;
	
	public bool isHealing
	{
		get
		{
			return (healingMonster != null);
		}
	}
	
	public long timeToHealMillis
	{
		get
		{
			return (maxHP - currHP) * 10000;
		}
	}
	
	public long finishHealTimeMillis
	{
		get
		{
			if (healingMonster == null)
			{
				return 0;
			}
			return healingMonster.expectedStartTimeMillis + timeToHealMillis; 
		}
	}
	
	public long healTimeLeft
	{
		get
		{
			if (healingMonster == null)
			{
				return 0;
			}
			return finishHealTimeMillis - CBKUtil.timeNowMillis;
		}
	}
	
	public int healCost
	{
		get
		{
			return maxHP - currHP;
		}
	}
	
	public int maxHP;
	public int currHP;
	
	public float[] attackDamages = new float[5];
	
	public PZMonster(FullUserMonsterProto userMonster)
	{
		this.userMonster = userMonster;
		this.monster = CBKDataManager.instance.Get(typeof(MonsterProto), userMonster.monsterId) as MonsterProto;
		
		SetupWithUser();
	}
	
	public PZMonster(MonsterProto monster, FullUserMonsterProto userMonster)
	{
		this.monster = monster;
		this.userMonster = userMonster;
		
		SetupWithUser();
	}
	
	public PZMonster(TaskStageMonsterProto taskMonster)
	{
		this.taskMonster = taskMonster;
		this.monster = CBKDataManager.instance.Get(typeof(MonsterProto), taskMonster.monsterId) as MonsterProto;
		
		SetupWithTask();
	}
	
	public PZMonster(MonsterProto monster, TaskStageMonsterProto taskMonster)
	{
		this.monster = monster;
		this.taskMonster = taskMonster;
		
		SetupWithTask();
	}
	
	public void UpdateUserMonster(FullUserMonsterProto userMonster)
	{
		this.userMonster = userMonster;
		SetupWithUser();
	}
	
	void SetupWithUser()
	{
		SetMaxHP(monster.baseHp, monster.hpLevelMultiplier, userMonster.currentLvl);
		SetAttackDamagesFromMonster(userMonster.currentLvl);
		currHP = userMonster.currentHealth;
	}
	
	void SetupWithTask()
	{
		SetMaxHP(monster.baseHp, monster.hpLevelMultiplier, taskMonster.level);
		SetAttackDamagesFromMonster(taskMonster.level);
		currHP = maxHP;
	}

	void SetMaxHP(int baseHP, float hpLevelMux, int level)
	{
		maxHP = (int)(baseHP + Mathf.Pow(hpLevelMux, level));
	}
	
	void SetAttackDamagesFromMonster(int level)
	{
		float attackMux = Mathf.Pow(monster.attackLevelMultiplier, level);
		
		attackDamages[0] = attackMux * monster.elementOneDmg;
		attackDamages[1] = attackMux * monster.elementTwoDmg;
		attackDamages[2] = attackMux * monster.elementThreeDmg;
		attackDamages[3] = attackMux * monster.elementFourDmg;
		attackDamages[4] = attackMux * monster.elementFiveDmg;
	}

	
}
