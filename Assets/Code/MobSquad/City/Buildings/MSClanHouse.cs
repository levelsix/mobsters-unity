using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSClanHouse : MSBuildingFrame {

	const string HELP_NAME = "helpredbubble";

	void OnEnable()
	{
		MSActionManager.Clan.OnEndClanHelp += DealwithEndHelp;
		MSActionManager.Clan.OnGiveClanHelp += DealWithGiveHelp;
		MSActionManager.Clan.OnSolicitClanHelp += DealWithSolicitHelp;
		FirstFrameCheck();
	}

	void OnDisable()
	{
		MSActionManager.Clan.OnEndClanHelp -= DealwithEndHelp;
		MSActionManager.Clan.OnGiveClanHelp -= DealWithGiveHelp;
		MSActionManager.Clan.OnSolicitClanHelp -= DealWithSolicitHelp;
	}

	void DealWithGiveHelp(GiveClanHelpResponseProto response, bool self)
	{
		CheckTag();
	}

	void DealWithSolicitHelp(SolicitClanHelpResponseProto response, bool self)
	{
		CheckTag();
	}

	void DealwithEndHelp(EndClanHelpResponseProto response, bool self)
	{
		CheckTag();
	}

	public override void CheckTag(){

		if(Precheck())
		{
			int helpableCount = MSClanManager.instance.currHelpable;

			if(helpableCount > 0)
			{
				bubbleIcon.gameObject.SetActive(true);
				if(helpableCount < 9)
				{
					bubbleIcon.spriteName = HELP_NAME + helpableCount.ToString();
				}
				else
				{
					bubbleIcon.spriteName = HELP_NAME + "exclemation";
				}
				bubbleIcon.MakePixelPerfect();
			}
		}
	}
}
