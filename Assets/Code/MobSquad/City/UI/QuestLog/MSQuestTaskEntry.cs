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

	int city = 0;
	
	const string DONE_STRING = "Done!";
	
	const string VISIT_STRING = "GO";
	
	public void Pool ()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}

	public void Init(MSFullQuest quest, UserQuestJobProto userJob, int num)
	{
		QuestJobProto job = quest.quest.jobs.Find(x=>x.questJobId == userJob.questJobId);

		taskNameLabel.text = job.description;

		progressLabel.text = userJob.progress + "/" + job.quantity;

		SetComplete(userJob.isComplete);

		taskNumberLabel.text = num.ToString();

		city = job.cityId;
	}
	
	public void SetComplete(bool complete)
	{
		if (complete)
		{
			visitOrDoneSprite.alpha = 0;
			visitOrDoneLabel.text = DONE_STRING;
		}
		else
		{
			visitOrDoneSprite.alpha = 1;
			visitOrDoneLabel.text = VISIT_STRING;
		}
	}

	public void GoToCity()
	{
		MSWhiteboard.currCityType = city > 0 ? MSWhiteboard.CityType.NEUTRAL : MSWhiteboard.CityType.PLAYER;
		MSWhiteboard.cityID = city > 0 ? city : MSWhiteboard.localMup.userId;

		//TODO: Lock screen!

		StartCoroutine(city > 0 ? MSBuildingManager.instance.LoadNeutralCity(city) : MSBuildingManager.instance.LoadPlayerCity());
	}
	
}
