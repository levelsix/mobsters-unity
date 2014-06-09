using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK quest task entry.
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
public class MSQuestTaskEntry : MonoBehaviour
{
	[SerializeField]
	UILabel taskNameLabel;
	
	[SerializeField]
	UILabel progressLabel;
	
	[SerializeField]
	UISprite visitOrDoneSprite;
	
	[SerializeField]
	UILabel visitOrDoneLabel;

	[SerializeField]
	UILabel taskNumberLabel;

	int num = 0;

	int city = 0;

	MSFullQuest quest;

	QuestJobProto job;

	UserQuestJobProto userJob;
	
	const string DONE_STRING = "Done!";
	
	const string VISIT_STRING = "GO";

	const string DONATE_STRING = "DONATE";
	
	public void Pool ()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}

	public void Init(MSFullQuest quest, UserQuestJobProto userJob, int num)
	{
		QuestJobProto job = quest.quest.jobs.Find(x=>x.questJobId == userJob.questJobId);

		this.quest = quest;
		this.job = job;
		this.userJob = userJob;
		this.num = num;

		taskNameLabel.text = job.description;

		progressLabel.text = userJob.progress + "/" + job.quantity;

		SetComplete(userJob.isComplete);

		taskNumberLabel.text = num.ToString();

		city = job.cityId;
	}

	public void DoneMoving()
	{
		foreach (var item in GetComponentsInChildren<UIWidget>()) 
		{
			item.MarkAsChanged();
		}
	}
	
	public void SetComplete(bool complete)
	{
		if (complete)
		{
			visitOrDoneSprite.alpha = 0;
			visitOrDoneLabel.text = DONE_STRING;
		}
		else if (job.questJobType == QuestJobProto.QuestJobType.DONATE_MONSTER)
		{
			visitOrDoneSprite.alpha = 1;
			visitOrDoneLabel.text = DONATE_STRING;
		}
		else
		{
			visitOrDoneSprite.alpha = 1;
			visitOrDoneLabel.text = VISIT_STRING;
		}
	}

	/// <summary>
	/// Called when the button is clicked.
	/// If the task is a donate task, attempt to donate the monsters.
	/// Otherwise, go to the proper city
	/// </summary>
	public void ButtonClick()
	{
		if (job.questJobType == QuestJobProto.QuestJobType.DONATE_MONSTER)
		{
			if (MSQuestManager.instance.AttemptDonation(quest, job, userJob))
			{
				Init(quest, userJob, num);
			}
		}
		else
		{
			MSWhiteboard.currCityType = city > 0 ? MSWhiteboard.CityType.NEUTRAL : MSWhiteboard.CityType.PLAYER;
			MSWhiteboard.cityID = city > 0 ? city : MSWhiteboard.localMup.userId;

			//TODO: Lock screen!

			StartCoroutine(city > 0 ? MSBuildingManager.instance.LoadNeutralCity(city) : MSBuildingManager.instance.LoadPlayerCity());
		}
	}
	
}
