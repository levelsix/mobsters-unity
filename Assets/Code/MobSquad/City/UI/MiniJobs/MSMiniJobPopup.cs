using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobPopup
/// </summary>
public class MSMiniJobPopup : MonoBehaviour {

	#region Prefabs

	[SerializeField]
	MSMiniJobEntry jobEntryPrefab;

	[SerializeField]
	MSMiniJobGoonie goonEntryPrefab;

	#endregion

	#region Job Screen

	[SerializeField]
	UIGrid miniJobGrid;

	[SerializeField]
	MSUIHelper noJobsLabel;

	[SerializeField]
	UILabel newJobSpawnTimer;

	#endregion

	#region Details Screen

	[SerializeField]
	UIGrid goonGrid;

	[SerializeField]
	MSFillBar reqHpBar;

	[SerializeField]
	UILabel reqHpLabel;

	[SerializeField]
	MSFillBar reqAtkBar;

	[SerializeField]
	UILabel reqAtkLabel;

	[SerializeField]
	UISprite engageButton;

	[SerializeField]
	UISprite engageArrow;

	[SerializeField]
	UISprite engageText;

	[SerializeField]
	UILabel time;

	[SerializeField]
	MSUIHelper tapHint;

	[SerializeField]
	MSMiniJobGoonSlot[] goonSlots;

	#endregion

	[SerializeField]
	TweenPosition mover;

	public void Init()
	{
		mover.Sample(0, true);

		SetupJobGrid();
	}

	void SetupJobGrid()
	{
		foreach (var item in MSMiniJobManager.instance.userMiniJobs) 
		{
			
		}
	}

	public void OnJobClicked(UserMiniJobProto job)
	{
		mover.PlayForward();
	}
}
