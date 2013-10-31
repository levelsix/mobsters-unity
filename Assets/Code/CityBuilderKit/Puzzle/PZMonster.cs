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
	
	public UserEnhancementItemProto enhancement = null;
	
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
			return (maxHP - currHP) * 1000;
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
	
	public bool isEnhancing
	{
		get
		{
			return enhancement != null;
		}
	}
	
	public int enhanceXP
	{
		get
		{
			return maxHP;
		}
	}
	
	public int enhanceCost
	{
		get
		{
			return enhanceXP;
		}
	}
	
	public long enhanceTimeLeft
	{
		get
		{
			if (enhancement == null)
			{
				return 0;
			}
			return finishEnhanceTime - CBKUtil.timeNowMillis;
		}
	}
	
	public long timeToUseEnhance
	{
		get
		{
			return enhanceXP * 1000;
		}
	}
	
	public long finishEnhanceTime
	{
		get
		{
			if (enhancement == null || enhancement.expectedStartTimeMillis == 0) 
			{
				return 0;
			}
			else
			{
				return enhancement.expectedStartTimeMillis + timeToUseEnhance;
			}
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
	
	#region Experience
	
	int ExpForLevel(int level)
	{
		if (level <= 1)
		{
			return 0;
		}
		return level * 1000 + ExpForLevel(level-1);
	}
	
	public void GainXP(int exp)
	{
		userMonster.currentExp += exp;
		while (userMonster.currentExp > ExpForLevel(userMonster.currentLvl))
		{
			userMonster.currentLvl++;
		}
	}
	
	public UserMonsterCurrentExpProto GetCurrentExpProto()
	{
		UserMonsterCurrentExpProto umcep = new UserMonsterCurrentExpProto();
		umcep.userMonsterId = userMonster.userMonsterId;
		umcep.expectedExperience = userMonster.currentExp;
		umcep.expectedLevel = userMonster.currentLvl;
		return umcep;
	}
	
	#endregion
}
