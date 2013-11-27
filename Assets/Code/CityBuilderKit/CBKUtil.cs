using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class CBKUtil {
	
	
	#region Time
	
	//Don't fuck with these unless you're a wizard
	const int SECS_PER_MIN = 60;
	const int SECS_PER_HOUR = 60 * 60;
	const int SECS_PER_DAY = 60 * 60 * 24;
	
	public static long timeNowMillis
	{
		get
		{
			return CBKMath.UnixTimeStamp(DateTime.UtcNow) * 1000;
		}
	}
	
	public static long timeSince(long time)
	{
		return timeNowMillis - time;
	}
	
	/// <summary>
	/// Returns a string describing an amount of time in short form
	/// Examples: 
	/// 7d 3h
	/// 3h 32m
	/// 54s
	/// 1s
	/// </summary>
	/// <returns>
	/// The short-form string
	/// </returns>
	/// <param name='time'>
	/// Amount of time to be written to a string
	/// </param>
	public static string TimeStringShort(/*me love you*/ long time)
	{
		time /= 1000;
		if (time > SECS_PER_DAY)
		{
			string str = (time / SECS_PER_DAY) + "d";
			long hours = ((time % SECS_PER_DAY) / SECS_PER_HOUR);
			if (hours > 0)
			{
				str += " " + hours + "h";
			}
			return str;
		}
		else if (time > SECS_PER_HOUR)
		{
			string str = (time / SECS_PER_HOUR) + "h";
			long min = ((time % SECS_PER_HOUR) / SECS_PER_MIN);
			if (min > 0)
			{
				str += " " + min + "m";
			}
			return str;
		}
		else if (time > SECS_PER_MIN)
		{
			string str = (time / SECS_PER_MIN) + "m";
			long sec = (time % SECS_PER_MIN);
			if (sec > 0)
			{
				str += " " + sec + "s";
			}
			return str;
		}
		else
		{
			return time + "s";
		}
	}
	
	/// <summary>
	/// Returns a string describing an amount of time in medium form
	/// Examples: 
	/// 7days 3hrs
	/// 3hrs 32min
	/// 54sec
	/// 1sec
	/// </summary>
	/// <returns>
	/// The medium-form string
	/// </returns>
	/// <param name='time'>
	/// Amount of time to be written to a string
	/// </param>	
	public static string TimeStringMed(/*me love you*/ long time)
	{
		time /= 1000;
		if (time > SECS_PER_DAY)
		{
			string str = "";
			long days = (time / SECS_PER_DAY);
			str += days + " day";
			if (days > 1)
			{
				str += "s";
			}
			long hours = ((time % SECS_PER_DAY) / SECS_PER_HOUR);
			if (hours > 0)
			{
				str += " " + hours + " hr";
				if (hours > 1)
				{
					str += "s";
				}
			}
			return str;
		}
		else if (time > SECS_PER_HOUR)
		{
			string str = "";
			long hours = (time / SECS_PER_HOUR);
			str += hours + " hr";
			if (hours > 1)
			{
				str += "s";
			}
			long min = ((time % SECS_PER_HOUR) / SECS_PER_MIN);
			if (min > 0)
			{
				str += " " + min + " min";
			}
			return str;
		}
		else if (time > SECS_PER_MIN)
		{
			string str = "";
			long min = (time / SECS_PER_MIN);
			str += min + " min";
			long sec = (time % SECS_PER_MIN);
			if (sec > 0)
			{
				str += " " + sec + " sec";
			}
			return str;
		}
		else
		{
			return time + " sec";
		}
	}

	/// <summary>
	/// Returns a string describing an amount of time in long form
	/// Examples: 
	/// 7 days 3 hours
	/// 3 hours 32 minutes
	/// 54 seconds
	/// 1 second
	/// </summary>
	/// <returns>
	/// The long-form string
	/// </returns>
	/// <param name='time'>
	/// Amount of time to be written to a string
	/// </param>
	public static string TimeStringLong(/*me love you*/ long time)
	{
		time /= 1000;
		if (time > SECS_PER_DAY)
		{
			string str = "";
			long days = (time / SECS_PER_DAY);
			str += days + " day";
			if (days > 1)
			{
				str += "s";
			}
			long hours = ((time % SECS_PER_DAY) / SECS_PER_HOUR);
			if (hours > 0)
			{
				str += " " + hours + " hour";
				if (hours > 1)
				{
					str += "s";
				}
			}
			return str;
		}
		else if (time > SECS_PER_HOUR)
		{
			string str = "";
			long hours = (time / SECS_PER_HOUR);
			str += hours + " hour";
			if (hours > 1)
			{
				str += "s";
			}
			long min = ((time % SECS_PER_HOUR) / SECS_PER_MIN);
			if (min > 0)
			{
				str += " " + min + " minute";
			}
			return str;
		}
		else if (time > SECS_PER_MIN)
		{
			string str = "";
			long min = (time / SECS_PER_MIN);
			str += min + " minute";
			long sec = (time % SECS_PER_MIN);
			if (sec > 0)
			{
				str += " " + sec + " second";
				if (sec > 1)
				{
					str += "s";
				}
			}
			return str;
		}
		else
		{
			if (time != 1)
			{
				return time + " seconds";
			}
			return time + " second";
		}
	}
	
	#endregion

	#region Monster Type Comparisons

	const float normalDamageRatio = 1f;
	const float weakDamageRatio = 1.25f;
	const float resistantDamageRatio = .75f;
	
	//Maps Monster Type -> Type of the damage it is taking -> damage multiplier
	public static readonly Dictionary<MonsterProto.MonsterElement, Dictionary<MonsterProto.MonsterElement, float>> elementStrengths = 
		new Dictionary<MonsterProto.MonsterElement, Dictionary<MonsterProto.MonsterElement, float>>()
	{
		{
			MonsterProto.MonsterElement.FIRE, new Dictionary<MonsterProto.MonsterElement, float>()
			{
				{MonsterProto.MonsterElement.GRASS, resistantDamageRatio},
				{MonsterProto.MonsterElement.WATER, weakDamageRatio}
			}
		},
		{
			MonsterProto.MonsterElement.WATER, new Dictionary<MonsterProto.MonsterElement, float>()
			{
				{MonsterProto.MonsterElement.GRASS, weakDamageRatio},
				{MonsterProto.MonsterElement.FIRE, resistantDamageRatio}
			}
		},
		{
			MonsterProto.MonsterElement.GRASS, new Dictionary<MonsterProto.MonsterElement, float>()
			{
				{MonsterProto.MonsterElement.FIRE, weakDamageRatio},
				{MonsterProto.MonsterElement.WATER, resistantDamageRatio}
			}
		},
		{
			MonsterProto.MonsterElement.DARKNESS, new Dictionary<MonsterProto.MonsterElement, float>()
			{
				{MonsterProto.MonsterElement.DARKNESS, resistantDamageRatio},
				{MonsterProto.MonsterElement.LIGHTNING, weakDamageRatio}
			}
		},
		{
			MonsterProto.MonsterElement.LIGHTNING, new Dictionary<MonsterProto.MonsterElement, float>()
			{
				{MonsterProto.MonsterElement.DARKNESS, weakDamageRatio},
				{MonsterProto.MonsterElement.LIGHTNING, resistantDamageRatio}
			}
		}
	};
	
	public static float GetTypeDamageMultiplier(MonsterProto.MonsterElement monsterType, MonsterProto.MonsterElement attackType)
	{
		if (elementStrengths[monsterType].ContainsKey(attackType))
		{
			return elementStrengths[monsterType][attackType];
		}
		return normalDamageRatio;
	}

	#endregion

	public static string StripExtensions(string file)
	{
		return StripSpaces(Path.GetFileNameWithoutExtension(file));
	}

	public static string StripSpaces(string word)
	{
		StringBuilder sb = new StringBuilder(word.Length);
		char c;
		for (int i = 0; i < word.Length; i++) {
			c = word[i];
			if (c != ' ')
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}

	public static void LoadLocalUser (FullUserProto user)
	{
		CBKWhiteboard.localUser = user;
		CBKWhiteboard.localMup = new MinimumUserProto();
		CBKWhiteboard.localMup.userId = user.userId;
		CBKWhiteboard.localMup.clan = user.clan;
		
		CBKWhiteboard.cityID = CBKWhiteboard.localMup.userId;
		
		CBKWhiteboard.nextLevelInfo = CBKDataManager.instance.Get(typeof(StaticUserLevelInfoProto), user.level+1) as StaticUserLevelInfoProto;
		
		UMQNetworkManager.instance.CreateUserIDQueue(CBKWhiteboard.localMup);
	}
	
	public static List<T> CopyList<T>(List<T> original)
	{
		List<T> copy = new List<T>();
		foreach (var item in original) 
		{
			copy.Add(item);
		}
		return copy;
	}
}
