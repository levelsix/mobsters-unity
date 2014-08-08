using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public enum MonsterStatus
{
	HEALTHY, INJURED, HEALING, ENHANCING, EVOLVING, COMBINING, INCOMPLETE, ON_MINI_JOB
}

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

	public ClanRaidStageMonsterProto raidMonster;
	
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
			return (long)((maxHP - (currHP + healingMonster.healthProgress)) * 1000 * MSWhiteboard.constants.monsterConstants.secondsToHealPerHealthPoint);
		}
	}

	public float healProgressPercentage
	{
		get
		{
			if (healingMonster == null)
			{
				return 1;
			}
			float progress = healingMonster.healthProgress / totalHealthToHeal;
			for (int i = 0; i < hospitalTimes.Count; i++) {
				HospitalTime hosTime = hospitalTimes[i];
				if (hosTime.startTime <= MSUtil.timeNowMillis)
				{
					if (i < hospitalTimes.Count-1 && hospitalTimes[i].startTime < MSUtil.timeNowMillis)
					{
						progress += ((hospitalTimes[i].startTime - hosTime.startTime) / 1000
						             * hosTime.hospital.proto.healthPerSecond) / totalHealthToHeal;
					}
					else
					{
						progress += ((MSUtil.timeNowMillis - hosTime.startTime) / 1000
						             * hosTime.hospital.proto.healthPerSecond) / totalHealthToHeal;
					}
				}
			}
			//Debug.Log("Progress: " + progress);
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
			return finishHealTimeMillis - MSUtil.timeNowMillis;
		}
	}
	
	public int healCost
	{
		get
		{
			return (int)(totalHealthToHeal * MSWhiteboard.constants.monsterConstants.cashPerHealthPoint);
		}
	}
	
	public int healFinishGems
	{
		get
		{
			return Mathf.CeilToInt((healTimeLeftMillis/60000) / MSWhiteboard.constants.minutesPerGem);
		}
	}

	public int totalHealthToHeal
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
			return enhancement != null && enhancement.userMonsterId > 0;
		}
	}
	
	public int enhanceXP
	{
		get
		{
			int xp = userMonster.currentLvl * monster.lvlInfo[0].feederExp;
			if (MSMonsterManager.instance.currentEnhancementMonster != null
			    && MSMonsterManager.instance.currentEnhancementMonster.monster.monsterElement
			    	== monster.monsterElement)
			{
				xp = Mathf.FloorToInt(xp * 1.5f);
			}
			return xp;
		}
	}
	
	public int enhanceCost
	{
		get
		{
			int totalLevel = (int)LevelWithFeeders(MSMonsterManager.instance.enhancementFeeders);
			return Mathf.FloorToInt(MSWhiteboard.constants.monsterConstants.oilPerMonsterLevel * totalLevel);
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
			return finishEnhanceTime - MSUtil.timeNowMillis;
		}
	}

	public float enhanceProgress
	{
		get
		{
			return 1 - ((float)enhanceTimeLeft) / ((float)timeToUseEnhance);
		}
	}
	
	public long timeToUseEnhance
	{
		get
		{
			return (long)(enhanceXP/MSBuildingManager.enhanceLabs[0].combinedProto.lab.pointsPerSecond) * 1000;
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
			return finishCombineTime - MSUtil.timeNowMillis;
		}
	}
	
	public int combineFinishGems
	{
		get
		{
			return Mathf.CeilToInt((combineTimeLeft/60000f) / MSWhiteboard.constants.minutesPerGem);
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
			return Mathf.CeilToInt((userMonster.currentLvl + userMonster.currentExp) 
				* ((float)userMonster.numPieces) / monster.numPuzzlePieces);
		}
	}

	public bool isEvoloving
	{
		get
		{
			return MSEvolutionManager.instance.IsMonsterEvolving(userMonster.userMonsterId);
		}
	}

	/// <summary>
	/// Whether this monster can currently be sold.
	/// Basically, if it's not healing, enhancing, or evolving, and it's completed
	/// </summary>
	/// <value><c>true</c> if sellable; otherwise, <c>false</c>.</value>
	public bool sellable
	{
		get
		{
			return !isHealing && !isEnhancing && !isEvoloving;
		}
	}

	public MonsterStatus monsterStatus
	{
		get
		{
			if (userMonster.numPieces < monster.numPuzzlePieces)
			{
				return MonsterStatus.INCOMPLETE;
			}
			if (!userMonster.isComplete)
			{
				return MonsterStatus.COMBINING;
			}
			if (isEnhancing)
			{
				return MonsterStatus.ENHANCING;
			}
			if (isEvoloving)
			{
				return MonsterStatus.EVOLVING;
			}
			if (isHealing)
			{
				return MonsterStatus.HEALING;
			}
			if (MSMiniJobManager.instance.IsMonsterBusy(userMonster.userMonsterId))
			{
				return MonsterStatus.ON_MINI_JOB;
			}
			if (MSEvolutionManager.instance.IsMonsterEvolving(userMonster.userMonsterId))
			{
				return MonsterStatus.EVOLVING;
			}
			if (currHP < maxHP)
			{
				return MonsterStatus.INJURED;
			}
			return MonsterStatus.HEALTHY;
		}
	}

	float maxExp 
	{
		get
		{
			return maxLevelInfo.curLvlRequiredExp;
		}
	}

	MonsterLevelInfoProto baseLevelInfo
	{
		get
		{
			return monster.lvlInfo[0];
		}
	}

	MonsterLevelInfoProto maxLevelInfo
	{
		get
		{
			return monster.lvlInfo[monster.lvlInfo.Count-1];
		}
	}

	public int maxHP;
	public int currHP;

	/// <summary>
	/// The minimum damage.
	/// CLAN RAID BOSSES ONLY!
	/// </summary>
	public int minDamage;

	/// <summary>
	/// The max damage.
	/// CLAN RAID BOSSES ONLY!
	/// </summary>
	public int maxDamage;

	public float[] attackDamages = new float[PZPuzzleManager.GEM_TYPES];

	public float totalDamage = 0;

	public int level;
	
	public PZMonster(FullUserMonsterProto userMonster)
	{
		this.userMonster = userMonster;
		this.monster = MSDataManager.instance.Get(typeof(MonsterProto), userMonster.monsterId) as MonsterProto;
		
		SetupWithUser();
	}

	public PZMonster (MinimumUserMonsterProto pvpMonster)
	{
		this.monster = MSDataManager.instance.Get<MonsterProto>(pvpMonster.monsterId);

		currHP = maxHP = MaxHPAtLevel(pvpMonster.monsterLvl);
		SetAttackDamagesForLevel(pvpMonster.monsterLvl);

		level = pvpMonster.monsterLvl;
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
		this.monster = MSDataManager.instance.Get(typeof(MonsterProto), taskMonster.monsterId) as MonsterProto;

		level = taskMonster.level;

		SetupWithTask();
	}
	
	public PZMonster(MonsterProto monster, TaskStageMonsterProto taskMonster)
	{
		this.monster = monster;
		this.taskMonster = taskMonster;

		level = taskMonster.level;
		
		SetupWithTask();
	}

	public PZMonster(ClanRaidStageMonsterProto raidMonster)
	{
		this.raidMonster = raidMonster;
		this.monster = MSDataManager.instance.Get<MonsterProto>(raidMonster.monsterId);

		currHP = maxHP = raidMonster.monsterHp;

		minDamage = raidMonster.minDmg;
		maxDamage = raidMonster.maxDmg;

		level = 100;
	}

	public PZMonster(MonsterProto monster, int level)
	{
		this.monster = monster;

		currHP = maxHP = MSMath.MaxHPAtLevel(monster, level);
		SetAttackDamagesForLevel(level);

		this.level = level;
	}
	
	public void UpdateUserMonster(FullUserMonsterProto userMonster)
	{
		this.userMonster = userMonster;
		SetupWithUser();
	}
	
	void SetupWithUser()
	{
		level = userMonster.currentLvl = Math.Min(userMonster.currentLvl, monster.maxLevel);

		maxHP = MaxHPAtLevel(userMonster.currentLvl);
		currHP = userMonster.currentHealth;
		SetAttackDamagesForLevel(userMonster.currentLvl);
	}
	
	void SetupWithTask()
	{
		currHP = maxHP = MaxHPAtLevel(taskMonster.level);
		SetAttackDamagesForLevel(taskMonster.level);
	}

	int AttackDamageForElement(Element element, int level)
	{
		int baseDmg = 0;
		int finalDmg = 0;
		switch(element)
		{
		case Element.FIRE:
			baseDmg = baseLevelInfo.fireDmg;
			finalDmg = maxLevelInfo.fireDmg;
			break;
		case Element.EARTH:
			baseDmg = baseLevelInfo.grassDmg;
			finalDmg = maxLevelInfo.grassDmg;
			break;
		case Element.WATER:
			baseDmg = baseLevelInfo.waterDmg;
			finalDmg = maxLevelInfo.waterDmg;
			break;
		case Element.LIGHT:
			baseDmg = baseLevelInfo.lightningDmg;
			finalDmg = maxLevelInfo.lightningDmg;
			break;
		case Element.DARK:
			baseDmg = baseLevelInfo.darknessDmg;
			finalDmg = maxLevelInfo.darknessDmg;
			break;
		case Element.ROCK:
			baseDmg = baseLevelInfo.rockDmg;
			finalDmg = maxLevelInfo.rockDmg;
			break;
		default:
			break;
		}

		return (int)(baseDmg + (finalDmg-baseDmg) 
		             * Mathf.Pow((level-1)/((float)(monster.maxLevel-1)), 
		            	maxLevelInfo.dmgExponentBase));
	}

	void SetAttackDamagesForLevel(int level)
	{
		if (monster.lvlInfo.Count > 0)
		{
			for (int i = 0; i < attackDamages.Length; i++) 
			{
				//NOTE: Because Protocol Buffers are weird, the Element enum starts at 1
				//So we have to adjust for the off-by-one error -_-
				attackDamages[i] = AttackDamageForElement((Element)(i+1), level);
			}
		}
		else
		{
			for (int i = 0; i < attackDamages.Length; i++) 
			{
				attackDamages[i] = 1;
			}
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
		if (monster.lvlInfo.Count == 0)
		{
			return PZPuzzleManager.GEM_TYPES;
		}

		int dmg = 0;
		for (int i = 0; i < PZPuzzleManager.GEM_TYPES; i++) 
		{
			dmg += AttackDamageForElement((Element)(i+1), level); 
		}
		return dmg;
	}

	public int MaxHPAtLevel(int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}

		return (int)(baseLevelInfo.hp + (maxLevelInfo.hp - baseLevelInfo.hp)
			* Mathf.Pow((level-1)/((float)(monster.maxLevel-1)), maxLevelInfo.hpExponentBase));
	}

	public int SpeedAtLevel(int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}
		
		return (int)(baseLevelInfo.speed + (maxLevelInfo.speed - baseLevelInfo.speed)
		             * ((level-1) / (monster.maxLevel-1)));
	}

	public UserMonsterCurrentHealthProto GetCurrentHealthProto()
	{
		UserMonsterCurrentHealthProto umchp = new UserMonsterCurrentHealthProto();
		umchp.currentHealth = currHP;
		umchp.userMonsterId = userMonster.userMonsterId;
		return umchp;
	}

	public MinimumUserMonsterSellProto GetSellProto()
	{
		MinimumUserMonsterSellProto mumsp = new MinimumUserMonsterSellProto();
		mumsp.userMonsterId = userMonster.userMonsterId;
		mumsp.cashAmount = sellValue;
		return mumsp;
	}
	
	#region Experience

	/// <summary>
	/// Returns a value from [0,1) that designates the progress until this monster next levels up,
	/// Given a certain amount of experience
	/// </summary>
	/// <returns>The level up progress percentage</returns>
	/// <param name="totalExp">The total experience to use for this check</param>
	public float PercentageOfLevelup(int totalExp)
	{
		return LevelForMonster(totalExp) % 1;
	}

	/// <summary>
	/// Calculates the exp percentage if the added exp were to be added to this monster's current amount of exp
	/// Used to display on cards in the enhancement view how much they will add if used to enhance this mobster
	/// </summary>
	/// <returns>Exp percentage gained by adding this much exp</returns>
	/// <param name="withAddedExp">Exp being added to this monster</param>
	public float PercentageOfAddedLevelup(int withAddedExp, bool checkFeeders = true)
	{
		int currExp = 0;
		if (checkFeeders)
		{
			foreach (var item in MSMonsterManager.instance.enhancementFeeders) 
			{
				currExp += item.enhanceXP;
			}
		}
		currExp += userMonster.currentExp;

		float currLevel = LevelForMonster(currExp);
		float addedLevel = LevelForMonster(currExp + withAddedExp);

		return addedLevel - currLevel;
	}

	public float LevelWithFeeders(List<PZMonster> feeders)
	{
		return LevelForMonster(ExpWithFeeders(feeders));
	}

	public int ExpWithFeeders(List<PZMonster> feeders)
	{
		int xp = userMonster.currentExp;
		foreach (var item in feeders) 
		{
			xp += item.enhanceXP;
		}
		return xp;
	}

	public float LevelForMonster(float withExp)
	{
		float level = 1;
		if (monster.lvlInfo.Count > 0)
		{
			level = Mathf.Pow(withExp/maxExp, 1/monster.lvlInfo[0].expLvlExponent) * monster.lvlInfo[0].expLvlDivisor + 1;
		}
		return Mathf.Min(monster.maxLevel, level);
	}

	public int XpForLevel(int level)
	{
		int xp = 0;
		if (monster.lvlInfo.Count > 0)
		{
			xp = Mathf.CeilToInt(maxExp * Mathf.Pow((level-1) / monster.lvlInfo[0].expLvlDivisor, monster.lvlInfo[0].expLvlExponent));
		}
		return xp;
	}
	
	public void GainXP(int exp)
	{
		userMonster.currentExp += exp;
		level = userMonster.currentLvl = (int)LevelForMonster(userMonster.currentExp);
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

[Serializable]
public struct HospitalTime
{
	public MSHospital hospital;
	public long startTime;
	public HospitalTime(MSHospital hospital, long startTime)
	{
		this.hospital = hospital;
		this.startTime = startTime;
	}
}
