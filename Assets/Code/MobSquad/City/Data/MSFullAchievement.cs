using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Just a data structure for holding onto
/// both user achievements and their corresponding 
/// achievement protos
/// </summary>
[System.Serializable]
public class MSFullAchievement 
{
	public UserAchievementProto userAchievement;
	public AchievementProto achievement;

	public bool clanAchievement = false;

	public MSFullAchievement successor
	{
		get
		{
			return MSAchievementManager.instance.currAchievements.Find(x=>x.achievement.achievementId == achievement.successorId);
		}
	}

	/// <summary>
	/// Initializes a new full achievement according to the achievement id.
	/// Used when we complete an achievement, and need to add its successor
	/// to the list of achievements.
	/// </summary>
	/// <param name="achievementId">Achievement identifier.</param>
	public MSFullAchievement(int achievementId)
	{
		userAchievement = new UserAchievementProto();
		userAchievement.achievementId = achievementId;
		userAchievement.isComplete = false;
		userAchievement.isRedeemed = false;
		userAchievement.progress = 0;
		achievement = MSDataManager.instance.Get<AchievementProto>(achievementId);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MSFullAchievement"/> class from
	/// an achievement proto.
	/// Used for achievements that have no progress, but satisfied prereqs.
	/// </summary>
	/// <param name="achievement">Achievement.</param>
	public MSFullAchievement(AchievementProto achievement)
	{
		this.achievement = achievement;
		userAchievement = new UserAchievementProto();
		userAchievement.achievementId = achievement.achievementId;
		userAchievement.isComplete = false;
		userAchievement.isRedeemed = false;
		userAchievement.progress = 0;

		clanAchievement = achievement.achievementType == AchievementProto.AchievementType.JOIN_CLAN
			|| achievement.achievementType == AchievementProto.AchievementType.GIVE_HELP
				|| achievement.achievementType == AchievementProto.AchievementType.SOLICIT_HELP;
	}

	public MSFullAchievement(UserAchievementProto userAchievement)
	{
		this.userAchievement = userAchievement;
		this.achievement = MSDataManager.instance.Get<AchievementProto>(userAchievement.achievementId);
	}

	/// <summary>
	/// Adds progress, and then adds itself to the AchievementManager's list of Achievements
	/// to update on the next frame.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void AddProgress(int amount)
	{
		if (amount <= 0) return;
		userAchievement.progress = Mathf.Min(userAchievement.progress + amount, achievement.quantity);
		userAchievement.isComplete = (userAchievement.progress >= achievement.quantity);
		MSAchievementManager.instance.AddProgressedAchievement(this);
	}

}
