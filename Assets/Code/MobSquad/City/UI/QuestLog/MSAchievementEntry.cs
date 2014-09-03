using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[RequireComponent (typeof (MSSimplePoolable))]
public class MSAchievementEntry : MonoBehaviour {

	#region UI elements

	[SerializeField]
	UILabel rankNumber;

	[SerializeField]
	UISprite[] stars;

	[SerializeField]
	UILabel achName;

	[SerializeField]
	UILabel achDescription;

	[SerializeField]
	UILabel gemNumber;

	[SerializeField]
	MSUIHelper barHelper;

	[SerializeField]
	MSFillBar fillBar;

	[SerializeField]
	UILabel progressLabel;

	[SerializeField]
	MSUIHelper buttonHelper;

	[SerializeField]
	MSLoadLock loadLock;

	#endregion

	MSFullAchievement fullAchievement;

	const string fullStar = "fullachievementstar";
	const string emptyStar = "emptyachievementstar";

	public void Init(MSFullAchievement fullAch)
	{
		fullAchievement = fullAch;

		rankNumber.text = fullAch.achievement.lvl.ToString();

		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].spriteName = i < fullAch.achievement.lvl ? fullStar : emptyStar;
		}

		achName.text = fullAch.achievement.name;
		achDescription.text = fullAch.achievement.description;
		gemNumber.text = fullAch.achievement.gemReward.ToString();

		if (fullAch.userAchievement.isComplete)
		{
			barHelper.TurnOff();
			buttonHelper.TurnOn();
		}
		else
		{
			buttonHelper.TurnOff();
			barHelper.TurnOn();

			fillBar.fill = ((float)fullAch.userAchievement.progress) / fullAch.achievement.quantity;

			if (fullAch.achievement.resourceType == ResourceType.CASH)
			{
				progressLabel.text = "$" + fullAch.userAchievement.progress + " / $" + fullAch.achievement.quantity;
			}
			else
			{
				progressLabel.text = fullAch.userAchievement.progress + " / " + fullAch.achievement.quantity;
			}
		}

	}

	/// <summary>
	/// Assigned to the COLLECT button in the editor
	/// </summary>
	public void Redeem()
	{
		StartCoroutine(DoRedeem());
	}

	IEnumerator DoRedeem()
	{
		MSFullAchievement successor = null;

		yield return StartCoroutine(MSAchievementManager.instance.RedeemAchievement(fullAchievement, successor, loadLock));
		
		Vector3 offsetVector = new Vector3(GetComponent<UIWidget>().width, 0, 0);
		
		if (successor != null)
		{
			MSAchievementEntry newEntry = MSPopupManager.instance.popups.questPopup.AddAchievement(successor);
			newEntry.transform.localPosition = transform.localPosition + offsetVector;
			TweenPosition.Begin(newEntry.gameObject, .3f, transform.localPosition);
		}
		else
		{
			MSPopupManager.instance.popups.questPopup.achievementGrid.animateSmoothly = true;
			MSPopupManager.instance.popups.questPopup.achievementGrid.Reposition();
		}
		
		transform.parent = transform.parent.parent;
		TweenPosition.Begin(gameObject, .3f, transform.localPosition - offsetVector);
	}
}
