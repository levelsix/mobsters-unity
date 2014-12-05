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

	public List<PersistentEventProto> GetActiveEvents()
	{
		List<PersistentEventProto> list = new List<PersistentEventProto>();

		foreach (PersistentEventProto item in MSDataManager.instance.GetAll<PersistentEventProto>().Values) 
		{
			//Note: C# day of week ranges 0-6, our day of week ranges 1-7. Add one to DateTime.Now.DayOfWeek to make it work
			if ((int)item.dayOfWeek == (int)DateTime.Now.DayOfWeek+1)
			{
				if (item.startHour <= DateTime.Now.Hour && item.startHour * 60 + item.eventDurationMinutes >= DateTime.Now.Hour * 60 + DateTime.Now.Minute)
				{
					list.Add(item);
				}
			}
		}

		return list;
	}

	public bool IsOnCooldown(PersistentEventProto persisEvent)
	{
		if (!eventHistory.ContainsKey(persisEvent.eventId))
		{
			return false;
		}
		return (MSUtil.timeNowMillis - eventHistory[persisEvent.eventId].coolDownStartTime <= persisEvent.cooldownMinutes * 60000);
	}

	public long GetRemainingCoolDown(PersistentEventProto persisEvent)
	{
		if(IsOnCooldown(persisEvent))
		{
			return MSUtil.timeNowMillis - eventHistory[persisEvent.eventId].coolDownStartTime;
		}
		else
		{
			return 0;
		}
	}

	public void DoBeginDungeonRequest(PersistentEventProto pEvent, int gems, Action OnComplete = null)
	{
		StartCoroutine(BeginDungeonRequest(pEvent, gems, OnComplete));
	}

	IEnumerator BeginDungeonRequest(PersistentEventProto pEvent, int gems, Action OnComplete)
	{
		BeginDungeonRequestProto request = new BeginDungeonRequestProto();
		request.clientTime = MSUtil.timeNowMillis;
		request.gemsSpent = gems;//to speed up the cooldown
		request.persistentEventId = pEvent.eventId;
		request.sender = MSWhiteboard.localMup;
		request.taskId = pEvent.taskId;
		request.isEvent = true;//always true because this is for persistent events?

		request.userBeatAllCityTasks = false;//notused
		request.elem = Element.DARK;//notused
		request.forceEnemyElem = false;//notused
		//I guess we don't use quests any more but yaknow w/e
		foreach(MSFullQuest item in MSQuestManager.instance.currQuests)
		{
			if(!item.complete)
			{
				request.questIds.Add(item.quest.questId);
			}
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_DUNGEON_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		BeginDungeonResponseProto response = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if(response.status != BeginDungeonResponseProto.BeginDungeonStatus.SUCCESS)
		{
			Debug.LogError("Problem Begining Dungeon: " + response.status.ToString());
		}
		else
		{
			if(MSActionManager.Dungeon.OnBeginEventDungeonSuccess != null)
			{
				MSActionManager.Dungeon.OnBeginEventDungeonSuccess();
			}
		}
		
		if(OnComplete != null)
		{
			OnComplete();
		}
	}
}
