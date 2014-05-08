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

	long shieldTime = long.MaxValue;

	UILabel label;

	bool running = false;

	bool set = false;

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
		set = true;
	}

	IEnumerator RunShieldClock()
	{
		while (!set)
		{
			yield return null;
		}

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
					yield return null;
				}
				else if (timeLeft < 1000 * 60)
				{
					yield return new WaitForSeconds(1);
				}
				else
				{
					yield return new WaitForSeconds(60);
				}
			}
		}
	}
}
