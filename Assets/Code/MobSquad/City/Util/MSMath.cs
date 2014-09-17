using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// An assortment of useful math and utility functions that are used by multiple classes
/// </summary>
public static class MSMath {
	
	const int SECONDS_PER_GEM = 400;
	
	const float MINUTES_TO_UPGRADE_FOR_NORM_STRUCT_MULTIPLIER = 1;
	
	/// <summary>
	/// Gets the square of the ground distance (y-axis ignored)
	/// between two points.
	/// </summary>
	/// <returns>
	/// The distance square
	/// </returns>
	/// <param name='ptA'>
	/// Point a.
	/// </param>
	/// <param name='ptB'>
	/// Point b.
	/// </param>
	public static float GroundDistanceSqr(Vector3 ptA, Vector3 ptB)
	{
		return Mathf.Pow(ptA.x-ptB.x,2) + Mathf.Pow (ptA.z-ptB.z,2);
	}
	
    /// <summary>
    /// Takes an amount of time and figures out how many gems to charge
    /// </summary>
    /// <returns>
    /// The number of gems
    /// </returns>
    /// <param name='time'>
    /// Time, in seconds
    /// </param>
	public static int GemsForTime(long time, bool canBeFree = true)
	{
		if (canBeFree && time < 60 * 1000 * MSWhiteboard.constants.maxMinutesForFreeSpeedUp) return 0;
		return 1 + (int)Mathf.Ceil((float)(time / 1000 / SECONDS_PER_GEM));
	}
	
    /// <summary>
    /// Takes a DateTime object and turns it into a timestamp that adheres to
    /// Unix Standard Time
    /// </summary>
    /// <returns>
    /// The time stamp.
    /// </returns>
    /// <param name='time'>
    /// Time.
    /// </param>
    public static long UnixTimeStamp(DateTime time)
    {
        return (long) (time - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
    }
	
	public static int TimeToBuildOrUpgradeStruct(int baseMin, int level)
	{
		if (level == 0)
		{
			return baseMin * 60 * 1000;
		}
		return Mathf.Max (1, 60 * 1000 * (int)(baseMin * (level+1) * MINUTES_TO_UPGRADE_FOR_NORM_STRUCT_MULTIPLIER));
	}
    
    /// <summary>
    /// Turns a ColorProto into a Color object
    /// </summary>
    /// <returns>
    /// The color.
    /// </returns>
    /// <param name='proto'>
    /// Proto of the color
    /// </param>
    public static Color ProtoToColor(ColorProto proto)
    {
        return new Color(proto.red, proto.green, proto.blue);
    }
    
    /// <summary>
    /// Figures the attack speed modifier out from the stat
    /// </summary>
    /// <returns>
    /// The speed mod.
    /// </returns>
    /// <param name='attackSpeed'>
    /// Attack speed character stat
    /// </param>
    public static float AttackSpeedMod(int attackSpeed)
    {
        return (100f / attackSpeed);
    }
    
	/// <summary>
	/// Calculates the damage multiplier based on the given resistance value
	/// </summary>
	/// <returns>
	/// The damage multiplier, between zero and one
	/// </returns>
	/// <param name='resistance'>
	/// Resistance stat
	/// </param>
    public static float ResistanceMod(int resistance)
    {
        return (100f - (resistance - 100f)) / 100f;
    }
	
	/// <summary>
	/// Merges a pair of dictionaries.
	/// Whatever values in the target that are greater than the destination
	/// will be copied over.
	/// Used at the beginning of a level to determine how many of each enemy type
	/// need to be warmed by the pre-spawner
	/// </summary>
	/// <param name='destination'>
	/// Dictionary that is accumulating merged data
	/// </param>
	/// <param name='target'>
	/// Dictionary that the destination is receiving information from
	/// </param>
	/// <typeparam name='T'>
	/// The index type of the dictionary key
	/// </typeparam>
	public static void MergeDicts<T>(Dictionary<T, int> destination, Dictionary<T, int> target)
	{
		foreach (KeyValuePair<T, int> item in target) 
		{
			if (!destination.ContainsKey(item.Key))
			{
				destination[item.Key] = item.Value;
			}
			else
			{
				if (item.Value > destination[item.Key])
				{
					destination[item.Key] = item.Value;
				}
			}
		}
	}
	
	/// <summary>
	/// Combines two dictionaries together.
	/// For every shared key, the values are added together and stored in the destination. 
	/// Each key that is unique to the target will be added to the destination.
	/// Used to tally up spawn counts for spawn groups.
	/// </summary>
	/// <param name='destination'>
	/// Dictionary collecting the values
	/// </param>
	/// <param name='target'>
	/// Dictionary being added to destination
	/// </param>
	/// <typeparam name='T'>
	/// The index type of the dictionary key 
	/// </typeparam>
	public static void AddDicts<T>(Dictionary<T, int> destination, Dictionary<T, int> target)
	{
		foreach (KeyValuePair<T, int> item in target) 
		{
			if (!destination.ContainsKey(item.Key))
			{
				destination[item.Key] = item.Value;
			}
			else
			{
				destination[item.Key] += item.Value;
			}
		}
	}
	
	/// <summary>
	/// Compresses a Color to a 32-bit integer
	/// </summary>
	/// <returns>
	/// The to int.
	/// </returns>
	/// <param name='color'>
	/// Color.
	/// </param>
	public static int ColorToInt(Color color)
	{
		return (((int)(color.r * 255) << 16) | ((int)(color.g * 255) << 8) | ((int)(color.b * 255)));
	}
	
	public static int DamageAtLevel(int baseDmg, int finalDmg, int level, int maxLevel, float dmgExponentBase)
	{

		return (int)(baseDmg + (finalDmg-baseDmg) 
		              * Mathf.Pow((level-1)/((float)(maxLevel-1)), dmgExponentBase));
	}
	
	public static int AttackAtLevel(MonsterProto monster, int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}
		MonsterLevelInfoProto baseLevelInfo = monster.lvlInfo[0];
		MonsterLevelInfoProto maxLevelInfo = monster.lvlInfo[monster.lvlInfo.Count-1];


		return DamageAtLevel(baseLevelInfo.fireDmg, maxLevelInfo.fireDmg, level, monster.maxLevel, maxLevelInfo.dmgExponentBase)
				+ DamageAtLevel(baseLevelInfo.grassDmg, maxLevelInfo.grassDmg, level, monster.maxLevel, maxLevelInfo.dmgExponentBase)
				+ DamageAtLevel(baseLevelInfo.waterDmg, maxLevelInfo.waterDmg, level, monster.maxLevel, maxLevelInfo.dmgExponentBase)
				+ DamageAtLevel(baseLevelInfo.lightningDmg, maxLevelInfo.lightningDmg, level, monster.maxLevel, maxLevelInfo.dmgExponentBase)
				+ DamageAtLevel(baseLevelInfo.darknessDmg, maxLevelInfo.darknessDmg, level, monster.maxLevel, maxLevelInfo.dmgExponentBase)
				+ DamageAtLevel(baseLevelInfo.rockDmg, maxLevelInfo.rockDmg, level, monster.maxLevel, maxLevelInfo.dmgExponentBase);
	}

	public static int SpeedAtLevel(MonsterProto monster, int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}
		
		MonsterLevelInfoProto baseLevelInfo = monster.lvlInfo[0];
		MonsterLevelInfoProto maxLevelInfo = monster.lvlInfo[monster.lvlInfo.Count-1];
		
		return (int)(baseLevelInfo.speed + (maxLevelInfo.speed - baseLevelInfo.speed)
		             * ((level-1) / (monster.maxLevel-1)));
	}
	
	public static int MaxHPAtLevel(MonsterProto monster, int level)
	{
		if (monster.lvlInfo.Count == 0)
		{
			return 1;
		}

		MonsterLevelInfoProto baseLevelInfo = monster.lvlInfo[0];
		MonsterLevelInfoProto maxLevelInfo = monster.lvlInfo[monster.lvlInfo.Count-1];

		return (int)(baseLevelInfo.hp + (maxLevelInfo.hp - baseLevelInfo.hp)
		             * Mathf.Pow((level-1)/((float)(monster.maxLevel-1)), maxLevelInfo.hpExponentBase));
		return Mathf.FloorToInt(monster.lvlInfo[0].hp * Mathf.Pow(monster.lvlInfo[0].hpExponentBase, level-1));
	}

	/// <summary>
	/// Ray cast from the specified camera and point in screen space
	/// </summary>
	/// <returns>Collider hit by cast.</returns>
	/// <param name="point">Point in screen space to cast from.</param>
	/// <param name="camera">Camera we're casting from.</param>
	public static Collider ClickRayCast(Vector2 point, Camera camera){
		if (camera != null)
		{
			//Cast a ray using the mouse position
			Ray ray = camera.ScreenPointToRay(point);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				//If our ray hits, select that object
				return hit.collider;
			}
			else
			{
				return null;
			}
		}
		return null;
	}

	/// <summary>
	/// Fucking magic
	/// </summary>
	/// <returns>The screen width.</returns>
	public static float uiScreenWidth
	{
		get
		{
			return Screen.width * 640f / Screen.height;
		}
	}

	/// <summary>
	/// Fucking magic
	/// </summary>
	/// <returns>The screen height.</returns>
	public static float uiScreenHeight
	{
		get
		{
			return 640f;
		}
	}

}
