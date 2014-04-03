using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRaidList
/// </summary>
public class MSRaidListEntry : MonoBehaviour {

	[SerializeField] UI2DSprite background;

	[SerializeField] UI2DSprite face;

	[SerializeField] UILabel activeName;
	[SerializeField] UILabel activeDescription;
	[SerializeField] UILabel activeTimeLeft;

	[SerializeField] UILabel inactiveName;
	[SerializeField] UILabel inactiveDescription;

	PersistentClanEventProto info;
	ClanRaidProto raid;

	const string lockedBackground = "lockedraidbg";

	public void Init(PersistentClanEventProto info)
	{
		this.info = info;
		raid = MSDataManager.instance.Get<ClanRaidProto>(info.clanRaidId);

		if (IsActive(info))
		{
			inactiveName.text = " ";
			inactiveDescription.text = " ";

			activeName.text = raid.clanRaidName;
			activeDescription.text = raid.activeDescription;
			if (MSClanEventManager.instance.currPersisRaid == info)
			{
				activeTimeLeft.text = "Raiding! Stage: " + MSClanEventManager.instance.currClanInfo.clanRaidStageId + " / Time Remaining: " + MSUtil.TimeStringShort(MSClanEventManager.instance.currStageTimeLeft);
				name = "active 0";
			}
			else
			{
				activeTimeLeft.text = "Raid Active Now / " + MSUtil.TimeStringShort(TimeLeft(info));
				name = "active " + info.clanRaidId;
			}

			background.sprite2D = MSAtlasUtil.instance.GetSprite("Raid/" + MSUtil.StripExtensions(raid.activeBackgroundImgName));
			if (background.sprite2D != null)
			{
				background.height = (int)background.sprite2D.textureRect.height;
				background.width = (int)background.sprite2D.textureRect.width;
				(collider as BoxCollider).size = new Vector3(background.localSize.x, background.localSize.y);
			}
			else
			{
				background.height = background.width = 0;
			}
			collider.enabled = true;
			//collider.bounds = background.sprite2D.bounds;

			face.sprite2D = null;
		}
		else
		{
			name = "inactive " + info.clanRaidId;

			collider.enabled = false;

			inactiveName.text = raid.clanRaidName;
			inactiveDescription.text = raid.inactiveDescription;

			activeName.text = " ";
			activeDescription.text = " ";
			activeTimeLeft.text = " ";

			background.sprite2D = MSAtlasUtil.instance.GetSprite("Raid/" + lockedBackground);
			if (background.sprite2D != null)
			{
				background.height = (int)background.sprite2D.textureRect.height;
				background.width = (int)background.sprite2D.textureRect.width;
			}
			else
			{
				background.height = background.width = 0;
			}
			
			face.sprite2D = MSAtlasUtil.instance.GetSprite("Raid/" + MSUtil.StripExtensions(raid.inactiveMonsterImgName));
			if (face.sprite2D != null)
			{
				face.height = (int)face.sprite2D.textureRect.height;
				face.width = (int)face.sprite2D.textureRect.width;
			}
			else
			{
				face.height = face.width = 0;
			}
		}
	}

	bool IsActive(PersistentClanEventProto info)
	{
		int dayAdjustment = ((int)DateTime.UtcNow.DayOfWeek) - (int)(info.dayOfWeek-1);
		if (dayAdjustment < 0)
		{	
			dayAdjustment += 7;
		}

		return DateTime.UtcNow.Hour > (info.startHour - dayAdjustment * 24)
			&& DateTime.UtcNow.Minute < (info.startHour - dayAdjustment * 24) * 60 + info.eventDurationMinutes;
	}

	long TimeLeft(PersistentClanEventProto info)
	{
		return ((info.eventDurationMinutes + info.startHour * 60)- (DateTime.UtcNow.Minute + DateTime.UtcNow.Hour * 60)) * 60000L;
	}

	public void OnClick()
	{
		MSPopupManager.instance.popups.raidScreen.Init(raid, info, MSClanEventManager.instance.inProgress ? MSClanEventManager.instance.currClanInfo : null);
		MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.raidScreen.GetComponent<MSPopup>());
	}
}
