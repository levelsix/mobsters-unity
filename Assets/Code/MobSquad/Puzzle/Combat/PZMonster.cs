using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public enum MonsterStatus
{
	HEALTHY, INJURED, HEALING, ENHANCING, EVOLVING, COMBINING, INCOMPLETE
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
			return userMonster.currentLvl * 777;
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
			return finishEnhanceTime - MSUtil.timeNowMillis;
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
			return userMonster.isComplete && !isHealing && !isEnhancing && !isEvoloving;
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
			if (currHP < maxHP)
			{
				return MonsterStatus.INJURED;
			}
			return MonsterStatus.HEALTHY;
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
		
		SetupWithTask();
	}
	
	public PZMonster(MonsterProto monster, TaskStageMonsterProto taskMonster)
	{
		this.monster = monster;
		this.taskMonster = taskMonster;
		
		SetupWithTask();
	}

	public PZMonster(ClanRaidStageMonsterProto raidMonster)
	{
		this.raidMonster = raidMonster;
		this.monster = MSDataManager.instance.Get<MonsterProto>(raidMonster.monsterId);

		currHP = maxHP = raidMonster.monsterHp;

		minDamage = raidMonster.minDmg;
		maxDamage = raidMonster.maxDmg;
	}
	
	public void UpdateUserMonster(FullUserMonsterProto userMonster)
	{
		this.userMonster = userMonster;
		SetupWithUser();
	}
	
	void SetupWithUser()
	{
		userMonster.currentLvl = Math.Min(userMonster.currentLvl, monster.maxLevel);

		maxHP = MaxHPAtLevel(userMonster.currentLvl);
		currHP = userMonster.currentHealth;
		SetAttackDamagesForLevel(userMonster.currentLvl);
	}
	
	void SetupWithTask()
	{
		currHP = maxHP = MaxHPAtLevel(taskMonster.level);
		SetAttackDamagesForLevel(taskMonster.level);
	}

	void SetAttackDamagesForLevel(int level)
	{
		if (monster.lvlInfo.Count > 0)
		{
			attackDamages[0] = monster.lvlInfo[0].fireDmg * Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1);
			attackDamages[1] = monster.lvlInfo[0].grassDmg * Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1);
			attackDamages[2] = monster.lvlInfo[0].waterDmg * Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1);
			attackDamages[3] = monster.lvlInfo[0].lightningDmg * Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1);
			attackDamages[4] = monster.lvlInfo[0].darknessDmg * Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1);
			attackDamages[5] = monster.lvlInfo[0].rockDmg * Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1);
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
		if (monster.lvlInfo.Count == 0)
		{
			return PZPuzzleManager.GEM_TYPES;
		}

		return Mathf.FloorToInt((monster.lvlInfo[0].fireDmg + monster.lvlInfo[0].grassDmg + monster.lvlInfo[0].waterDmg
				+ monster.lvlInfo[0].lightningDmg + monster.lvlInfo[0].darknessDmg + monster.lvlInfo[0].rockDmg)
				* Mathf.Pow(monster.lvlInfo[0].dmgExponentBase, level-1));
	}

	public int MaxHPAtLevel(int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}

		return Mathf.FloorToInt(monster.lvlInfo[0].hp * Mathf.Pow(monster.lvlInfo[0].hpExponentBase, level-1));
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
		float currPerc = 0;
		if (checkFeeders)
		{
			foreach (var item in MSMonsterManager.instance.enhancementFeeders) 
			{
				currExp += item.enhanceXP;
			}
			currPerc = PercentageOfAddedLevelup(currExp, false);
		}
		currExp += userMonster.currentExp;

		float currLevel = LevelForMonster(currExp);
		float addedLevel = LevelForMonster(currExp + withAddedExp);

		return addedLevel - currLevel;
	}

	public float LevelForMonster(int withExp)
	{
		float level = 1;
		if (monster.lvlInfo.Count > 0)
		{
			float maxExp = monster.lvlInfo[0].curLvlRequiredExp;
			level = Mathf.Pow(withExp/maxExp, 1/monster.lvlInfo[0].expLvlExponent) * monster.lvlInfo[0].expLvlDivisor + 1;
		}
		return Mathf.Min(monster.maxLevel, level);
	}
	
	public void GainXP(int exp)
	{
		userMonster.currentExp += exp;
		userMonster.currentLvl = (int)LevelForMonster(userMonster.currentExp);
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
	public MSHospital hospital;
	public long startTime;
	public HospitalTime(MSHospital hospital, long startTime)
	{
		this.hospital = hospital;
		this.startTime = startTime;
	}
}
