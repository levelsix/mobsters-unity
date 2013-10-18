using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;
using System.Collections.Generic;

public static class CBKUtil {
	
	//Don't fuck with these unless you're a wizard
	const int SECS_PER_MIN = 60;
	const int SECS_PER_HOUR = 60 * 60;
	const int SECS_PER_DAY = 60 * 60 * 24;
	
	public static long timeNow
	{
		get
		{
			return CBKMath.UnixTimeStamp(DateTime.UtcNow) * 1000;
		}
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
	
	public static void LoadLocalUser (FullUserProto user)
	{
		CBKWhiteboard.localUser = user;
		CBKWhiteboard.localMup = new MinimumUserProto();
		CBKWhiteboard.localMup.name = user.name;
		CBKWhiteboard.localMup.userId = user.userId;
		CBKWhiteboard.localMup.clan = user.clan;
		
		
		
		CBKWhiteboard.cityID = CBKWhiteboard.localMup.userId;
		
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
