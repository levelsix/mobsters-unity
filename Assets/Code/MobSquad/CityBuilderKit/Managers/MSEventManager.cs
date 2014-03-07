using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSEventManager
/// </summary>
public class MSEventManager : MonoBehaviour {

	public static MSEventManager instance;

	Dictionary<int, UserPersistentEventProto> eventHistory = new Dictionary<int, UserPersistentEventProto>();

	void Awake()
	{
		instance = this;
	}

	public PersistentEventProto GetActiveEvent(PersistentEventProto.EventType type)
	{
		foreach (PersistentEventProto item in MSDataManager.instance.GetAll<PersistentEventProto>().Values) 
		{
			//Note: C# day of week ranges 0-6, our day of week ranges 1-7. Add one to DateTime.Now.DayOfWeek to make it work
			if (item.type == type && (int)item.dayOfWeek == (int)DateTime.Now.DayOfWeek+1)
			{
				if (item.startHour <= DateTime.Now.Hour && item.startHour * 60 + item.eventDurationMinutes >= DateTime.Now.Hour * 60 + DateTime.Now.Minute)
				{
					return item;
				}
			}
		}
		return null;
	}

	public bool IsOnCooldown(PersistentEventProto persisEvent)
	{
		if (!eventHistory.ContainsKey(persisEvent.eventId))
		{
			return false;
		}
		return (MSUtil.timeNowMillis - eventHistory[persisEvent.eventId].coolDownStartTime <= persisEvent.cooldownMinutes * 60000);
	}
}
