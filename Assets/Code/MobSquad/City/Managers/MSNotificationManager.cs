//#define ANDROID_DEVICE (UNITY_ANDROID && !UNITY_EDITOR)
#define ANDROID_DEVICE

using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSNotificationManager : MonoBehaviour 
{
	const string TITLE = "Toon Squad";

	const long DAY_MILLIS = 1000 * 60 * 60 * 24;

#if ANDROID_DEVICE
	void PushHealFinished()
	{
		if (MSHospitalManager.instance.timeLeft > 0)
		{
			PushNotification("Your " + MSValues.monsterName + "s have finished healing!", MSHospitalManager.instance.timeLeft);
		}
	}

	void PushEnhanceFinished()
	{
		if (MSEnhancementManager.instance.timeLeft > 0)
		{
			PushNotification(MSEnhancementManager.instance.enhancementMonster.monster.displayName + " has finished enhancing!",
			                 MSEnhancementManager.instance.timeLeft);
		}
	}

	void PushEvolutionFinished()
	{
		if (MSEvolutionManager.instance.timeLeftMillis > 0)
		{
			PushNotification(MSEvolutionManager.instance.resultMonster + " has finished evolving!"
			                 , MSEvolutionManager.instance.timeLeftMillis);
		}
	}

	void PushBuildingFinished()
	{
		if (MSBuildingManager.instance.currentUnderConstruction != null)
		{
			if (MSBuildingManager.instance.currentUnderConstruction.upgrade != null)
			{
				PushNotification("Your " + MSBuildingManager.instance.currentUnderConstruction.combinedProto.structInfo.name + " has finished construction!",
				                 MSBuildingManager.instance.currentUnderConstruction.upgrade.timeRemaining);
			}
			else if (MSBuildingManager.instance.currentUnderConstruction.obstacle != null)
			{
				PushNotification("Your " + MSBuildingManager.instance.currentUnderConstruction.obstacle.name + " has finished removal!",
				                 MSBuildingManager.instance.currentUnderConstruction.obstacle.millisLeft);
			}
		}
	}

	void PushMinijobFinished()
	{
		if (MSMiniJobManager.instance.timeLeft > 0)
		{
			PushNotification("Your " + MSValues.monsterName + "s have returned from their " 
			                 + MSMiniJobManager.instance.currActiveJob.miniJob.name + "!", 
			                 MSMiniJobManager.instance.timeLeft);
		}
	}

	void PushOilFull()
	{
		long oilTime = MSResourceManager.instance.TimeUntilResourceCollectorsFull(ResourceType.OIL);
		if (oilTime > 0)
		{
			PushResourceFull("Oil Drill", oilTime);
		}
	}

	void PushCashFull()
	{
		long cashTime = MSResourceManager.instance.TimeUntilResourceCollectorsFull(ResourceType.CASH);
		if (cashTime > 0)
		{
			PushResourceFull("Cash Printer", cashTime);
		}
	}

	void PushResourceFull(string buildingName, long time)
	{
		PushNotification("Your " + buildingName + "s have just filled up. Collect them now!", time);
	}

	void PushTime()
	{
		if (MSWhiteboard.localUser != null && !MSWhiteboard.localUser.userUuid.Equals(""))
		{

			//24 hours
			PushNotification("Hey " + MSWhiteboard.localUser.name + ", come back! Your " + MSValues.monsterName + "s need a leader!", DAY_MILLIS);

			//3 days
			PushNotification("Hey " + MSWhiteboard.localUser.name + ", come back! Your " + MSValues.monsterName + "s really need a leader!", DAY_MILLIS * 3);

			//7 days
			PushNotification("Hey " + MSWhiteboard.localUser.name + ", come back! Your " + MSValues.monsterName + "s really, really need a leader!", DAY_MILLIS * 7);

			//30 days
			PushNotification("Hey " + MSWhiteboard.localUser.name + ", come back! Your " + MSValues.monsterName + "s really, really, REALLY need a leader!", DAY_MILLIS * 30);
		
		}

	}

	void PushNotification(string message, long delayMillis)
	{
		ELANNotification notification = new ELANNotification();
		notification.Start();
		notification.message = message;
		notification.title = TITLE;
		notification.delayTypeTime = EnumTimeType.Seconds;
		notification.delay = (int)(delayMillis / 1000 + 1);
		notification.useSound = false;
		notification.useVibration = false;
		notification.send();
	}

	void PushAll()
	{
		ELANManager.CancelAllNotifications();
		PushHealFinished();
		PushEnhanceFinished();
		PushEvolutionFinished();
		PushBuildingFinished();
		PushMinijobFinished();
		PushOilFull();
		PushCashFull();
		PushTime();
	}

	void OnApplicationPause()
	{
		PushAll();
	}

	void OnApplicationQuit()
	{
		PushAll();
	}
#endif
}
