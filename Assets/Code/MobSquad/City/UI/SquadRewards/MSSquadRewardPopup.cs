using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSSquadRewardPopup : MonoBehaviour 
{
	[SerializeField] MSSquadReward join;
	[SerializeField] MSSquadReward requestHelp;
	[SerializeField] MSSquadReward helpOthers;

	void OnEnable()
	{
		Init ();
	}

	void Init()
	{
		join.Init (MSAchievementManager.instance.clanAchievements.Find(x=>x.achievement.achievementType == AchievementProto.AchievementType.JOIN_CLAN));
		requestHelp.Init (MSAchievementManager.instance.clanAchievements.Find(x=>x.achievement.achievementType == AchievementProto.AchievementType.SOLICIT_HELP));
		helpOthers.Init (MSAchievementManager.instance.clanAchievements.Find(x=>x.achievement.achievementType == AchievementProto.AchievementType.GIVE_HELP));
	}

}
