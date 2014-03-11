using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRaidStageBoxUI
/// </summary>
public class MSRaidStageBoxUI : MonoBehaviour {

	[SerializeField]
	UILabel stageName;

	[SerializeField]
	MSCenterGrid prizeGrid;

	[SerializeField]
	UILabel unlockLabel;

	[SerializeField]
	GameObject beginParent;

	[SerializeField]
	GameObject battleParent;

	[SerializeField]
	UISprite progressBar;

	[SerializeField]
	UILabel progressLabel;

	[SerializeField]
	UISprite header;

	[SerializeField]
	CBKMiniHealingBox[] prizes;

	const string openStage = "openstage";
	const string closedStage = "lockedstage";

	public void Init(ClanRaidStageProto stage, PersistentClanEventClanInfoProto clanInfo = null)
	{
		stageName.text = stage.name;

		//Prizes
		int i = 0;
		while (i < stage.possibleRewards.Count)
		{
			prizes[i++].Init(MSDataManager.instance.Get<MonsterProto>(stage.possibleRewards[i].monsterId));
		}

		while (i < prizes.Length)
		{
			prizes[i].gameObject.SetActive(false);
		}

		prizeGrid.Reposition();

		unlockLabel.text = " ";
		if (clanInfo == null)
		{

		}
		else if (stage.clanRaidStageId == clanInfo.clanRaidStageId)
		{
			battleParent.SetActive(true);
			beginParent.SetActive(false);

			//Figure out percentage
			float percentage = clanInfo.crsmId * (1f / stage.monsters.Count);

			progressBar.fillAmount = percentage;

			long timeLeft = (clanInfo.stageStartTime + stage.durationMinutes * 60000) - MSUtil.timeNowMillis;

			int percentageDisplay = (int)(percentage*100);

			progressLabel.text = percentageDisplay + "% Done / " + MSUtil.TimeStringShort(timeLeft) + " Left";

		}
		else if (stage.clanRaidStageId > clanInfo.clanRaidStageId) //Closed stage
		{
			battleParent.SetActive(false);
			beginParent.SetActive(false);

			unlockLabel.text = "Complete stage " + (stage.clanRaidStageId - 1) + " to Unlock";
		}
		else //Completed stage
		{

		}
	}


}
