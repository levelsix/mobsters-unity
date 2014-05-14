using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSHirePopup
/// </summary>
public class MSHirePopup : MonoBehaviour {

	[SerializeField]
	MSHireEntry hireEntryPrefab;

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	TweenPosition mover;

	[SerializeField]
	MSUIHelper back;

	[SerializeField]
	UILabel gemCost;

	List<MSHireEntry> hireEntries = new List<MSHireEntry>();

	int userStructId;

	ResidenceProto currResidenceLevel;

	public void Init(MSBuilding residence)
	{
		mover.Sample(0, true);
		back.gameObject.SetActive(false);

		userStructId = residence.userStructProto.userStructId;

		MSFullBuildingProto thisLevel = residence.combinedProto.baseLevel;
		int i = 0;
		while (thisLevel != null)
		{
			while (hireEntries.Count <= i)
			{
				AddHireEntry();
			}
			
			if (thisLevel.structInfo.level > residence.combinedProto.structInfo.level)
			{
				hireEntries[i].Init(thisLevel.residence, "Requires a level " + thisLevel.structInfo.level + " residence");
			}
			else if (thisLevel.structInfo.level > residence.userStructProto.fbInviteStructLvl + 1)
			{
				hireEntries[i].Init(thisLevel.residence, "Requires a " + thisLevel.predecessor.residence.occupationName);
			}
			else
			{
				hireEntries[i].Init(thisLevel.residence, thisLevel.structInfo.level <= residence.userStructProto.fbInviteStructLvl, residence.userStructProto.userStructId);
			}

			if (thisLevel.structInfo.level == residence.userStructProto.fbInviteStructLvl)
			{
				currResidenceLevel = thisLevel.residence;
			}
			
			thisLevel = thisLevel.successor;
			i++;
		}
		grid.Reposition();
	}

	void AddHireEntry()
	{
		MSHireEntry entry = Instantiate(hireEntryPrefab) as MSHireEntry;
		entry.transform.parent = grid.transform;
		entry.transform.localScale = Vector3.one;
		entry.transform.localPosition = Vector3.zero;
		hireEntries.Add(entry);
	}

	public void Back()
	{
		mover.PlayReverse();
		back.FadeOut();
	}

	public void SelectLevel()
	{
		back.TurnOn();
		back.ResetAlpha(false);
		back.Fade(true);
		mover.PlayForward();
		gemCost.text = currResidenceLevel.numGemsRequired.ToString();
	}

	public void LevelUpWithGems()
	{
		int numGems = currResidenceLevel.numGemsRequired;
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, numGems, LevelUpWithGems))
		{
			MSResidenceManager.instance.UpgradeResidenceFacebookLevelWithGems(userStructId, numGems);
		}
	}

	public void SendFriendRequests()
	{
		MSResidenceManager.instance.OpenRequestDialogue(userStructId);
	}
}
