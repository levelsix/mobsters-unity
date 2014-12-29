using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSSlideUp : MonoBehaviour 
{
	public static MSSlideUp instance;

	#region UI Elements

	[SerializeField]
	UILabel topLabel;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	UISprite goonBg;

	[SerializeField]
	UI2DSprite goonSprite;

	[SerializeField]
	UILabel progressLabel;

	#endregion

	bool isSliding;

	[SerializeField]
	TweenPosition slideTween;

	void Awake()
	{
		instance = this;
	}

	public void QueueQuestProgress(UserQuestJobProto userJob, QuestJobProto job, FullQuestProto quest)
	{
		Queue(
			"PROGRESS ALERT",
			quest.name,
			userJob.progress + "/" + job.quantity,
			"greenmediumsquare",
			quest.questGiverImagePrefix
		);
	}

	public void QueueMonsterFinishHealing(PZMonster monster)
	{
		Queue(
			"HEALING COMPLETE",
			monster.monster.displayName,
			"Done!",
			MSGoonCard.smallBackgrounds[monster.monster.monsterElement],
			monster.monster.imagePrefix
		);
	}

	public void Queue(string top, string bot, string progress, string bgName, string goonImgPrefix)
	{
		StartCoroutine(Slide(top, bot, progress, bgName, goonImgPrefix));
	}

	IEnumerator Slide(string top, string bot, string progress, string bgName, string goonImgPrefix)
	{
		while (isSliding)
		{
			yield return null;
		}

		topLabel.text = top;
		bottomLabel.text = bot;
		progressLabel.text = progress;

		goonBg.spriteName = bgName;
		MSSpriteUtil.instance.SetSprite(goonImgPrefix, goonImgPrefix + "Card",
		                                goonSprite);


		slideTween.Play();
		isSliding = true;
		while (slideTween.tweenFactor < 1)
		{
			yield return null;
		}
		isSliding = false;
	}
}
