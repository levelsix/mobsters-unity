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
		
		questProgress.text = quest.GetProgressString();
		
		fullQuest = quest;
	}
	
	void OnClick()
	{
		MSActionManager.UI.OnQuestEntryClicked(fullQuest);
	}
	
	public void Pool ()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
}
