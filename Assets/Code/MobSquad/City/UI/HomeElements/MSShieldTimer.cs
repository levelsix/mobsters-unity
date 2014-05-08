using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSShieldTimer
/// </summary>
[RequireComponent (typeof (UILabel))]
public class MSShieldTimer : MonoBehaviour {

	long shieldTime;

	UILabel label;

	bool running = false;

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
		StartCoroutine(RunShieldClock());
	}
	
	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
	}
	
	void OnStartup(StartupResponseProto response)
	{
		shieldTime = response.sender.pvpLeagueInfo.shieldEndTime;
	}

	IEnumerator RunShieldClock()
	{
		if (label == null)
		{
			label = GetComponent<UILabel>();
		}

		running = true;

		while (running)
		{
			if (MSUtil.timeNowMillis > shieldTime)
			{
				label.text = "NONE";
				running = false;
			}
			else
			{
				long timeLeft = shieldTime - MSUtil.timeNowMillis;
				label.text = MSUtil.TimeStringShort(timeLeft);

				if (timeLeft < 1000)
				{

				}

				//If there is more than a day left, don't bother updating
				//If there is less than a day, update in an hour
				if (timeLeft < 1000 * 60 * 60 * 24)
				{

				}
				else if (timeLeft < 1000 * 60 * 60)
				{

				}
			}
		}
	}
}
