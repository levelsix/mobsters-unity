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
			return MSEnhancementManager.instance.hasEnhancement && (MSEnhancementManager.instance.enhancementMonster == this || MSEnhancementManager.instance.feeders.Contains(this));
		}
	}
	
	public int enhanceXP
	{
		get
		{
			if (restricted) return 0;
			int xp = Mathf.FloorToInt(Mathf.Lerp(baseLevelInfo.feederExp, maxLevelInfo.feederExp, ((float)level)/maxLevelInfo.lvl));
			if (MSEnhancementManager.instance.enhancementMonster != null && MSEnhancementManager.instance.enhancementMonster.monster.monsterId != 0
			    && MSEnhancementManager.instance.enhancementMonster.monster.monsterElement
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
			return Mathf.FloorToInt(MSWhiteboard.constants.monsterConstants.oilPerMonsterLevel * level);
		}
	}

	public long enhanceTime
	{
		get
		{
			return (long)((enhanceXP * 1000f) / MSEnhancementManager.instance.currLab.pointsPerSecond);
		}
	}

	public long startEnhanceTime
	{
		get
		{
			if (!isEnhancing) return 0;
			return MSEnhancementManager.instance.currEnhancement.feeders.Find(x=>x.userMonsterId.Equals(userMonster.userMonsterId)).expectedStartTimeMillis;
		}
	}

	public long finishEnhanceTime
	{
		get
		{
			if (!isEnhancing) return 0;
			return enhanceTime + startEnhanceTime;
		}
	}

	public long enhanceTimeLeft
	{
		get
		{
			if (!isEnhancing) return 0;
			return finishEnhanceTime - MSUtil.timeNowMillis;
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
			if (restricted) return 0;

			int price;
			if (userMonster.isComplete)
			{
				price = Mathf.FloorToInt(Mathf.Lerp(baseLevelInfo.sellAmount, maxLevelInfo.sellAmount, ((level-1f)/(maxLevelInfo.lvl-1f))));
			}
			else
			{
				price = Mathf.FloorToInt(baseLevelInfo.sellAmount * ((float)(userMonster.numPieces)) / monster.numPuzzlePieces);
			}
			return Mathf.Max(1, price);
		}
	}

	public bool isEvoloving
	{
		get
		{
			return userMonster != null && MSEvolutionManager.instance.IsMonsterEvolving(userMonster.userMonsterId);
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
			if (userMonster != null && !userMonster.isComplete)
			{
				if (userMonster.numPieces < monster.numPuzzlePieces)
				{
					return MonsterStatus.INCOMPLETE;
				}
				return MonsterStatus.COMBINING;
			}
			if (isEnhancing || MSEnhancementManager.instance.feeders.Contains(this))
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

	public bool restricted
	{
		get
		{
			return userMonster.isRestrictd;
		}
		set
		{
			userMonster.isRestrictd = value;
			if (userMonster.isRestrictd)
			{
				MSMonsterManager.instance.RestrictMonster(userMonster.userMonsterId);
			}
			else
			{
				MSMonsterManager.instance.UnrestrictMonster(userMonster.userMonsterId);
			}
		}
	}

	public SkillProto offensiveSkill;
	public SkillProto defensiveSkill;

	public float speed;
	public int teamCost;

	public int maxHP;
	public int currHP;
	/// <summary>
	/// This is just for convient scratch board math use.
	/// </summary>
	public int tempHP;

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

		SetOffensiveSkill(userMonster.offensiveSkillId);
		SetDefensiveSkill(userMonster.defensiveSkillId);

		SetupWithUser();
	}

	public PZMonster (MinimumUserMonsterProto pvpMonster)
	{
		this.monster = MSDataManager.instance.Get<MonsterProto>(pvpMonster.monsterId);

		currHP = maxHP = MaxHPAtLevel(pvpMonster.monsterLvl);
		SetAttackDamagesForLevel(pvpMonster.monsterLvl);
		speed = SpeedAtLevel(pvpMonster.monsterLvl);
		SetDefensiveSkill(monster.baseDefensiveSkillId);

		level = pvpMonster.monsterLvl;
	}
	
	public PZMonster(MonsterProto monster, FullUserMonsterProto userMonster)
	{
		this.monster = monster;
		this.userMonster = userMonster;

		SetOffensiveSkill(userMonster.offensiveSkillId);
		SetDefensiveSkill(userMonster.defensiveSkillId);
		
		SetupWithUser();
	}
	
	public PZMonster(TaskStageMonsterProto taskMonster)
	{
		this.taskMonster = taskMonster;
		this.monster = MSDataManager.instance.Get(typeof(MonsterProto), taskMonster.monsterId) as MonsterProto;

		level = taskMonster.level;

		SetupWithTask();

		//Since skills can fuck with other stats, do that last
		SetDefensiveSkill(taskMonster.defensiveSkillId);
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

		SetOffensiveSkill(monster.baseOffensiveSkillId);
		SetDefensiveSkill(monster.baseDefensiveSkillId);

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
		speed = SpeedAtLevel(userMonster.currentLvl);
		teamCost = CostAtLevel(userMonster.currentLvl);
		maxHP = MaxHPAtLevel(userMonster.currentLvl);
		currHP = userMonster.currentHealth;
		SetAttackDamagesForLevel(userMonster.currentLvl);
	}
	
	void SetupWithTask()
	{
		currHP = maxHP = MaxHPAtLevel(taskMonster.level);
		speed = SpeedAtLevel(taskMonster.level);
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

	public int CostAtLevel(int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}

		return Mathf.FloorToInt(Mathf.Lerp (baseLevelInfo.teamCost, maxLevelInfo.teamCost, ((level-1f)/(monster.maxLevel-1f))));
	}

	public float SpeedAtLevel(int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}
		
		return (baseLevelInfo.speed + (maxLevelInfo.speed - baseLevelInfo.speed)
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

	public void SetOffensiveSkill(int skillId)
	{
		if (skillId > 0)
		{
			offensiveSkill = MSDataManager.instance.Get<SkillProto>(skillId);
		}
		else
		{
			offensiveSkill = null;
		}
	}
	public void SetDefensiveSkill(int skillId)
	{
		if (skillId > 0)
		{
			defensiveSkill = MSDataManager.instance.Get<SkillProto>(skillId);
			switch (defensiveSkill.type) 
			{
			case SkillType.CAKE_DROP:
				speed = defensiveSkill.properties.Find(x=>x.name == "INITIAL_SPEED").skillValue;
				break;
			}
		}
		else
		{
			defensiveSkill = null;
		}
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
			foreach (var item in MSEnhancementManager.instance.feeders) 
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
		maxHP = MaxHPAtLevel(level);
		speed = SpeedAtLevel(level);
		teamCost = CostAtLevel(level);
		SetAttackDamagesForLevel(level);
	}
	
	public UserMonsterCurrentExpProto GetCurrentExpProto()
	{
		UserMonsterCurrentExpProto umcep = new UserMonsterCurrentExpProto();
		umcep.userMonsterId = userMonster.userMonsterId;
		umcep.expectedExperience = userMonster.currentExp;
		umcep.expectedLevel = userMonster.currentLvl;
		return umcep;
	}

	public UserMonsterCurrentExpProto GetExpProtoWithMonsters(List<UserEnhancementItemProto> feeders)
	{
		UserMonsterCurrentExpProto umcep = new UserMonsterCurrentExpProto();
		umcep.userMonsterId = userMonster.userMonsterId;
		umcep.expectedExperience = userMonster.currentExp;

		List<PZMonster> feedMonsters = new List<PZMonster>();
		PZMonster monster;
		foreach (var item in feeders) {
			monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId.Equals(item.userMonsterId));
			feedMonsters.Add(monster);
			umcep.expectedExperience += monster.enhanceXP;
		}
		umcep.expectedLevel = (int)LevelWithFeeders(feedMonsters);
		umcep.expectedHp = MaxHPAtLevel(umcep.expectedLevel);
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
