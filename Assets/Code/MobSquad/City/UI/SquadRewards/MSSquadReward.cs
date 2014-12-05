using UnityEngine;
using System.Collections;

public class MSSquadReward : MSAchievementEntry {

	//Inherits labels progressLabel and gemNumber

	[SerializeField]
	GameObject rewardBg;

	[SerializeField]
	GameObject collectButton;

	[SerializeField]
	GameObject completeSprite;

	[SerializeField]
	UISprite progressBg;

	const string COMPLETE_SQUAD_PROG_BG = "completedsquadrewards";
	const string INCOMPLETE_SQUAD_PROG_BG = "squadrewardsprogress";

	public override void Init (MSFullAchievement fullAch)
	{
		fullAchievement = fullAch;

		rewardBg.SetActive(false);
		collectButton.SetActive(false);
		completeSprite.SetActive(false);

		if (fullAch.userAchievement.isRedeemed)
		{
			InitComplete();
		}
		else if (fullAch.userAchievement.isComplete)
		{
			InitCollection();
		}
		else
		{
			InitIncomplete();
		}
	}

	void InitIncomplete()
	{
		progressBg.spriteName = INCOMPLETE_SQUAD_PROG_BG;
		progressLabel.text = fullAchievement.userAchievement.progress + "/" + fullAchievement.achievement.quantity;

		rewardBg.SetActive(true);
		gemNumber.text = "(g) " + fullAchievement.achievement.gemReward 
			+ "GEM" + (fullAchievement.achievement.gemReward>1?"S":"");
	}

	void InitCollection()
	{
		progressBg.spriteName = COMPLETE_SQUAD_PROG_BG;
		progressLabel.text = " ";

		collectButton.SetActive(true);
	}

	void InitComplete()
	{
		progressBg.spriteName = COMPLETE_SQUAD_PROG_BG;
		progressLabel.text = " ";

		completeSprite.SetActive(true);
	}

	protected override IEnumerator DoRedeem ()
	{
		yield return StartCoroutine(MSAchievementManager.instance.RedeemAchievement(fullAchievement, loadLock));

		collectButton.SetActive(false);
		InitComplete();
	}
}
