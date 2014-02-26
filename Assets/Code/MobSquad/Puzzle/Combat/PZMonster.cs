using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	public int userHospitalID = 0;

	public List<HospitalTime> hospitalTimes = new List<HospitalTime>();
	
	public bool isHealing
	{
		get
		{
			return healingMonster != null && healingMonster.userMonsterId > 0;
		}
	}

	public long healStartTime
	{
		get
		{
			return healingMonster.queuedTimeMillis;
		}
	}

	public long timeToHealMillis
	{
		get
		{
			return (long)((maxHP - (currHP + healingMonster.healthProgress)) * 1000 * CBKWhiteboard.constants.monsterConstants.secondsToHealPerHealthPoint);
		}
	}

	public float healProgressPercentage
	{
		get
		{
			float progress = healingMonster.healthProgress;
			for (int i = 0; i < hospitalTimes.Count; i++) {
				HospitalTime hosTime = hospitalTimes[i];
				if (hosTime.startTime < CBKUtil.timeNowMillis)
				{
					if (i < hospitalTimes.Count-1 && hospitalTimes[i].startTime < CBKUtil.timeNowMillis)
					{
						progress += ((hospitalTimes[i].startTime - hosTime.startTime) * hosTime.hospital.combinedProto.hospital.healthPerSecond) / (maxHP - currHP);
					}
					else
					{
						progress += ((CBKUtil.timeNowMillis - hosTime.startTime) * hosTime.hospital.combinedProto.hospital.healthPerSecond) / (maxHP - currHP);
					}
				}
			}
			return progress;
		}
	}
	
	public long finishHealTimeMillis;
	
	public long healTimeLeftMillis
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
			return (int)((maxHP - currHP) * CBKWhiteboard.constants.monsterConstants.cashPerHealthPoint);
		}
	}
	
	public int healFinishGems
	{
		get
		{
			return Mathf.CeilToInt((healTimeLeftMillis/60000) / CBKWhiteboard.constants.minutesPerGem);
		}
	}
	
	public bool isEnhancing
	{
		get
		{
			return enhancement != null && enhancement.userMonsterId > 0;
		}
	}
	
	public int enhanceXP
	{
		get
		{
			MonsterLevelInfoProto lvlInfo = monster.lvlInfo.Find(x=>x.lvl==userMonster.currentLvl);
			if (lvlInfo == null)
			{
				Debug.LogError("Probleming finding xp for mobster " + monster.monsterId + " for level " + userMonster.currentLvl);
				return 0;
			}
			return lvlInfo.feederExp;
		}
	}
	
	public int enhanceCost
	{
		get
		{
			return 100;
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
			return 30000;
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
	
	public long finishCombineTime
	{
		get
		{
			return userMonster.combineStartTime + (monster.minutesToCombinePieces * 60 * 1000);
		}
	}
	
	public long combineTimeLeft
	{
		get
		{
			return finishCombineTime - CBKUtil.timeNowMillis;
		}
	}
	
	public int combineFinishGems
	{
		get
		{
			return Mathf.CeilToInt((combineTimeLeft/60000f) / CBKWhiteboard.constants.minutesPerGem);
		}
	}

	public float percentageTowardsNextLevel
	{
		get
		{
			return PercentageOfLevelup(userMonster.currentExp);
		}
	}

	public int sellValue
	{
		get
		{
			return userMonster.currentLvl + userMonster.currentExp;
		}
	}

	public bool isEvoloving
	{
		get
		{
			return CBKEvolutionManager.instance.IsMonsterEvolving(userMonster.userMonsterId);
		}
	}

	public int maxHP;
	public int currHP;

	public float[] attackDamages = new float[PZPuzzleManager.GEM_TYPES];

	public float totalDamage = 0;
	
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
		userMonster.currentLvl = Math.Min(userMonster.currentLvl, monster.maxLevel);

		MonsterLevelInfoProto lvlInfo = monster.lvlInfo.Find(x=>x.lvl==userMonster.currentLvl);
		if (lvlInfo != null)
		{
			maxHP = lvlInfo.hp;
		}
		else
		{
			maxHP = 1;
		}

		SetAttackDamagesFromMonsterLevelInfo(lvlInfo);
		currHP = userMonster.currentHealth;
	}
	
	void SetupWithTask()
	{
		MonsterLevelInfoProto levelInfo = monster.lvlInfo.Find(x=>x.lvl==taskMonster.level);
		maxHP = levelInfo.hp;
		SetAttackDamagesFromMonsterLevelInfo(levelInfo);
		currHP = maxHP;
	}

	void SetAttackDamagesFromMonsterLevelInfo(MonsterLevelInfoProto lvlInfo)
	{
		if (lvlInfo != null)
		{
			attackDamages[0] = lvlInfo.fireDmg;
			attackDamages[1] = lvlInfo.grassDmg;
			attackDamages[2] = lvlInfo.waterDmg;
			attackDamages[3] = lvlInfo.lightningDmg;
			attackDamages[4] = lvlInfo.darknessDmg;
			attackDamages[5] = lvlInfo.rockDmg;
		}
		else
		{
			attackDamages[0] = 1;
			attackDamages[1] = 1;
			attackDamages[2] = 1;
			attackDamages[3] = 1;
			attackDamages[4] = 1;
			attackDamages[5] = 1;
		}

		float total = 0;
		foreach (var damage in attackDamages) 
		{
			total += damage;
		}
		totalDamage = total;
	}

	public int TotalAttackAtLevel(int level)
	{
		MonsterLevelInfoProto levelInfo = monster.lvlInfo.Find(x=>x.lvl==level);
		return levelInfo.fireDmg + levelInfo.grassDmg + levelInfo.waterDmg
				+ levelInfo.lightningDmg + levelInfo.darknessDmg + levelInfo.rockDmg;
	}

	public int MaxHPAtLevel(int level)
	{
		return monster.lvlInfo.Find(x=>x.lvl==level).hp;
	}
	
	#region Experience
	
	public float PercentageOfLevelup(float totalExp)
	{
		return (totalExp - ExpForLevel(userMonster.currentLvl)) / (ExpForLevel(userMonster.currentLvl + 1) - ExpForLevel(userMonster.currentLvl));
	}

	/// <summary>
	/// Calculates the exp percentage if the added exp were to be added to this monster's current amount of exp
	/// </summary>
	/// <returns>Exp percentage gained by adding this much exp</returns>
	/// <param name="withAddedExp">Exp being added to this monster</param>
	public float PercentageOfAddedLevelup(float withAddedExp, bool checkFeeders = true)
	{
		int currExp = 0;
		float currPerc = 0;
		if (checkFeeders)
		{
			foreach (var item in CBKMonsterManager.enhancementFeeders) 
			{
				currExp += item.enhanceXP;
			}
			currPerc = PercentageOfAddedLevelup(currExp, false);
		}
		currExp += userMonster.currentExp;

		if (userMonster.currentLvl == monster.maxLevel)
		{
			return 0;
		}
		if (currExp + withAddedExp > ExpForLevel(userMonster.currentLvl + 1))
		{
			float total = 1 - percentageTowardsNextLevel; //Because we know this finishes the current level
			int levelsUp = 1;
			while(currExp + withAddedExp > ExpForLevel(userMonster.currentLvl + levelsUp + 1) && userMonster.currentLvl + levelsUp < monster.maxLevel)
	 	    {
				levelsUp++;
			}
			total += levelsUp - 1; //One less, because the first level is the partial level that we already added
			if (levelsUp + userMonster.currentLvl == monster.maxLevel)
			{
				return total - currPerc;
			}
			//Now get that level beyond the last level
			total += (float)(currExp + withAddedExp - ExpForLevel(userMonster.currentLvl + levelsUp)) / (ExpForLevel(userMonster.currentLvl + 1 + levelsUp) - ExpForLevel(userMonster.currentLvl + levelsUp));
			return total - currPerc;
		}
		else
		{
			return ((float)(currExp + withAddedExp - ExpForLevel(userMonster.currentLvl)) / (ExpForLevel(userMonster.currentLvl + 1) - ExpForLevel(userMonster.currentLvl))) - currPerc;
		}
	}
	
	int ExpForLevel(int level)
	{
		if (level > monster.maxLevel)
		{
			Debug.LogWarning("Attempting to get exp for a higher level than this monster can go");
			return ExpForLevel(monster.maxLevel);
		}
		return monster.lvlInfo.Find(x=>x.lvl==level).curLvlRequiredExp;
	}
	
	public void GainXP(int exp)
	{
		userMonster.currentExp += exp;
		while (userMonster.currentLvl < monster.maxLevel && userMonster.currentExp > ExpForLevel(userMonster.currentLvl + 1))
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

public struct HospitalTime
{
	public CBKBuilding hospital;
	public long startTime;
	public HospitalTime(CBKBuilding hospital, long startTime)
	{
		this.hospital = hospital;
		this.startTime = startTime;
	}
}
