using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSClanHouse : MSBuildingFrame {

	const string HELP_NAME = "helpredbubble";

	void OnEnable()
	{
//		MSActionManager.Clan.OnEndClanHelp += DealwithEndHelp;
//		MSActionManager.Clan.OnGiveClanHelp += DealWithGiveHelp;
//		MSActionManager.Clan.OnSolicitClanHelp += DealWithSolicitHelp;
//		FirstFrameCheck();

		MSActionManager.Clan.OnUpdateNumberOfAvailableHelpRequests += ChangeNumber;
	}

	void OnDisable()
	{
//		MSActionManager.Clan.OnEndClanHelp -= DealwithEndHelp;
//		MSActionManager.Clan.OnGiveClanHelp -= DealWithGiveHelp;
//		MSActionManager.Clan.OnSolicitClanHelp -= DealWithSolicitHelp;

		MSActionManager.Clan.OnUpdateNumberOfAvailableHelpRequests -= ChangeNumber;
	}

	void DealWithGiveHelp(GiveClanHelpResponseProto response, bool self)
	{
		FirstFrameCheck();
	}

	void DealWithSolicitHelp(SolicitClanHelpResponseProto response, bool self)
	{
		FirstFrameCheck();
	}

	void DealwithEndHelp(EndClanHelpResponseProto response, bool self)
	{
		FirstFrameCheck();
	}

	void ChangeNumber(int helpableCount)
	{
		bubbleIcon.gameObject.SetActive(false);
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

	public override void CheckTag(){

		if(Precheck())
		{
			bubbleIcon.gameObject.SetActive(false);
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
