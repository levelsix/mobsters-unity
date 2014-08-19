using UnityEngine;
using Serialization;
using System.Collections;
using System;
using com.lvl6.proto;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class MSUtil {
	
	
	#region Time
	
	//Don't fuck with these unless you're a wizard
	const int SECS_PER_MIN = 60;
	const int SECS_PER_HOUR = 60 * 60;
	const int SECS_PER_DAY = 60 * 60 * 24;
	
	public static long timeNowMillis
	{
		get
		{
			return MSMath.UnixTimeStamp(DateTime.UtcNow);
		}
	}

	public static long timeNowLocalMillis
	{
		get
		{
			return MSMath.UnixTimeStamp(DateTime.Now);
		}
	}

	public static long timeUntil(long time)
	{
		return Math.Max(time - timeNowMillis, 0);
	}
	
	public static long timeSince(long time)
	{
		return Math.Max(timeNowMillis - time, 0);
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
	public static string TimeStringLong(/*me love you*/ long time, bool onlyOne = false)
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
			if (onlyOne)
			{
				return str;
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
			if (onlyOne)
			{
				return str;
			}
			long min = ((time % SECS_PER_HOUR) / SECS_PER_MIN);
			if (min > 0)
			{
				str += " " + min + " minute";
				if (min > 1)
				{
					str += "s";
				}
			}
			return str;
		}
		else if (time > SECS_PER_MIN)
		{
			string str = "";
			long min = (time / SECS_PER_MIN);
			str += min + " minute";
			if (min > 1)
			{
				str += "s";
			}
			if (onlyOne)
			{
				return str;
			}
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

	enum Comparison {STRONG, WEAK, NORMAL};
	
	//Maps Monster Type -> Type of the damage it is taking -> damage multiplier
	static readonly Dictionary<Element, Dictionary<Element, Comparison>> elementStrengths = 
		new Dictionary<Element, Dictionary<Element, Comparison>>()
	{
		{
			Element.FIRE, new Dictionary<Element, Comparison>()
			{
				{Element.EARTH, Comparison.STRONG},
				{Element.WATER, Comparison.WEAK}
			}
		},
		{
			Element.WATER, new Dictionary<Element, Comparison>()
			{
				{Element.EARTH, Comparison.WEAK},
				{Element.FIRE, Comparison.STRONG}
			}
		},
		{
			Element.EARTH, new Dictionary<Element, Comparison>()
			{
				{Element.FIRE, Comparison.WEAK},
				{Element.WATER, Comparison.STRONG}
			}
		},
		{
			Element.DARK, new Dictionary<Element, Comparison>()
			{
				{Element.DARK, Comparison.STRONG},
				{Element.LIGHT, Comparison.WEAK}
			}
		},
		{
			Element.LIGHT, new Dictionary<Element, Comparison>()
			{
				{Element.DARK, Comparison.WEAK},
				{Element.LIGHT, Comparison.STRONG}
			}
		}
	};
	
	public static float GetTypeDamageMultiplier(Element monsterType, Element attackType)
	{
		if (!MSTutorialManager.instance.inTutorial && elementStrengths.ContainsKey(monsterType) && elementStrengths[monsterType].ContainsKey(attackType))
		{
			switch (elementStrengths[monsterType][attackType]) {
			case Comparison.WEAK:
				Debug.Log(monsterType + " hit by " + attackType + ": Super Effective!");
				return MSWhiteboard.constants.monsterConstants.elementalStrength;
			case Comparison.STRONG:
				Debug.Log(monsterType + " hit by " + attackType + ": Not Effective!");
				return MSWhiteboard.constants.monsterConstants.elementalWeakness;
			default:
				break;
			}
		}
		return 1;
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
		if (user == null)
		{
			Debug.LogError("User is null...");
			return;
		}
		MSWhiteboard.localUser = user;
		MSWhiteboard.localMup = new MinimumUserProto();
		MSWhiteboard.localMup.userId = user.userId;
		MSWhiteboard.localMup.clan = user.clan;
		
		MSWhiteboard.cityID = MSWhiteboard.localMup.userId;
		
		MSWhiteboard.nextLevelInfo = MSDataManager.instance.Get(typeof(StaticUserLevelInfoProto), user.level+1) as StaticUserLevelInfoProto;
		
		UMQNetworkManager.instance.CreateUserIDQueue(MSWhiteboard.localMup);
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

	public static string LeagueRankSuffix(int rank)
	{
		switch(rank % 10)
		{
		case 1:
			return "st";
			break;
		case 2:
			return "nd";
			break;
		case 3:
			return "rd";
			break;
		default:
			return "th";
			break;
		}
	}

	public static void Save(string key, object obj)
	{
		//Debug.Log("Saving: " + key);
		if (obj == null)
		{
			PlayerPrefs.DeleteKey(key);
		}
		else
		{
			var data = Convert.ToBase64String(UnitySerializer.Serialize(obj));
			PlayerPrefs.SetString(key, data);
			PlayerPrefs.Save();
		}
	}

	public static T Load<T>(string key) where T : class
	{
		Debug.Log("Loading: " + key);
		T result = default(T);
		var data = PlayerPrefs.GetString(key);
		
		if (!string.IsNullOrEmpty(data))
		{
			try
			{
				result = UnitySerializer.Deserialize<T>(Convert.FromBase64String(data));
			}
			catch {}
		}
		
		return result;
	}

}
