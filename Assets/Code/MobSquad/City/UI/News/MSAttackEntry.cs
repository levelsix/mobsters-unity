using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSAttackEntry
/// @author Rob Giusti
/// </summary>
public class MSAttackEntry : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	UILabel attackerNameLabel;

	[SerializeField]
	UILabel timeAgoLabel;

	[SerializeField]
	MSMobsterIcon[] team;

	[SerializeField]
	UISprite rankIcon;

	[SerializeField]
	UILabel rankChangeLabel;

	[SerializeField]
	UILabel cashLostLabel;

	[SerializeField]
	UILabel oilLostLabel;

	[SerializeField]
	UILabel revengeLabel;

	[SerializeField]
	GameObject revengeButton;

	[SerializeField]
	Color hasRevengeLabelColor;

	[SerializeField]
	Color noRevengeLabelColor;

	#endregion

	public void Init(PvpHistoryProto proto)
	{
		attackerNameLabel.text = proto.attacker.name;
		timeAgoLabel.text = (proto.attackerWon ? "[ff0000]Defeat  " : "[00ff00]Victory  ")
			+ "[777777]" + MSUtil.TimeStringShort(MSUtil.timeNowMillis - proto.battleEndTime) + " ago";
		for (int i = 0; i < proto.attackersMonsters.Count; i++) 
		{
			team[i].Init(proto.attackersMonsters[i]);
		}
		rankIcon.spriteName = MSDataManager.instance.Get<PvpLeagueProto>(proto.defenderBefore.leagueId).imgPrefix + "icon";
		rankChangeLabel.text = (proto.attackerWon ? "[ff0000]" : "[00ff00]") + (proto.defenderAfter.rank - proto.defenderBefore.rank );
		cashLostLabel.text = proto.defenderCashChange.ToString();
		oilLostLabel.text = proto.defenderOilChange.ToString();

		if (!proto.exactedRevenge)
		{
			revengeButton.SetActive(true);
			revengeLabel.text = "Revenge";
		}
		else
		{
			revengeButton.SetActive(false);
			revengeLabel.text = "No revenge\nAvailable";
		}
	}

}
