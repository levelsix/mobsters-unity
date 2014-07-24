using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSSimplePoolable))]
public class MSQuestEntry : MonoBehaviour {
	
	[SerializeField]
	UISprite questGiverBG;

	[SerializeField]
	UI2DSprite questGiverThumb;

	[SerializeField]
	UILabel questName;
	
	[SerializeField]
	UILabel questProgress;
	
	[SerializeField]
	UISprite newQuestLabel;
	
	public MSFullQuest fullQuest;
	
	public void Init(MSFullQuest quest)
	{
		questName.text = quest.quest.name;
		
		questProgress.text = quest.progressString;
		
		fullQuest = quest;

		newQuestLabel.enabled = quest.newQuest;
	}
	
	void OnClick()
	{
		if (fullQuest.complete)
		{
			MSActionManager.Popup.CloseAllPopups();
			MSQuestManager.instance.CompleteQuest(fullQuest);
		}
		else
		{
			MSActionManager.UI.OnQuestEntryClicked(fullQuest);
			newQuestLabel.enabled = false;
		}
	}
	
	public void Pool ()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
}
