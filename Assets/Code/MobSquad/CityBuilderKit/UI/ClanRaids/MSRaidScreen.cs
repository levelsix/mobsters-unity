using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRaidScreen
/// </summary>
public class MSRaidScreen : MonoBehaviour {

	[SerializeField]
	UI2DSprite enemy;

	[SerializeField]
	UILabel raidName;

	[SerializeField]
	UILabel dialogue;

	[SerializeField]
	MSRaidStageBoxUI stageBoxPrefab;

	[SerializeField]
	UIGrid stageBoxParent;

	List<MSRaidStageBoxUI> stageBoxes = new List<MSRaidStageBoxUI>();

	public void Init(ClanRaidProto raid, PersistentClanEventClanInfoProto clanRaidInfo = null)
	{
		enemy.sprite2D = MSAtlasUtil.instance.GetSprite("Raid/" + MSUtil.StripExtensions(raid.spotlightMonsterImgName));
		raidName.text = raid.clanRaidName;
		dialogue.text = raid.dialogueText;

		int i = 0;
		for (; i < raid.raidStages.Count; i++) 
		{
			while (stageBoxes.Count <= i) //This could really be an if instead of a while, but we'll keep it like this in case something gets strage
			{
				AddStageBox();
			}

			stageBoxes[i].gameObject.SetActive(true);
			stageBoxes[i].Init(raid.raidStages[i], clanRaidInfo);
		}

		for (; i < stageBoxes.Count; i++) 
		{
			stageBoxes[i].gameObject.SetActive(false);
		}

		stageBoxParent.Reposition();
	}

	void AddStageBox()
	{
		MSRaidStageBoxUI stageBox = Instantiate(stageBoxPrefab) as MSRaidStageBoxUI;
		stageBox.transform.parent = stageBoxParent.transform;
	}
}
