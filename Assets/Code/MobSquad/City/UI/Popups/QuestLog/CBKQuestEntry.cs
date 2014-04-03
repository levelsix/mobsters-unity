using UnityEngine;
using System.Collections;

public class CBKQuestEntry : MonoBehaviour, MSPoolable {
	
	#region Poolable Members & Properties
	
	[HideInInspector]
	public CBKQuestEntry _prefab;
	
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKQuestEntry;
		}
	}
	
	[HideInInspector]
	public GameObject gameObj;
	
	[HideInInspector]
	public Transform trans;
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	public Transform transf {
		get {
			return trans;
		}
	}
	
	#endregion
	
	[SerializeField]
	UISprite questGiver;
	
	[SerializeField]
	UILabel questName;
	
	[SerializeField]
	UILabel questProgress;
	
	[SerializeField]
	UISprite newQuestLabel;
	
	public MSFullQuest fullQuest;
	
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
	}
	
	public void Init(MSFullQuest quest)
	{


		questName.text = quest.quest.name;
		
		questProgress.text = quest.GetProgressString();
		
		trans.localScale = Vector3.one;
		
		fullQuest = quest;
	}
	
	void OnClick()
	{
		MSActionManager.UI.OnQuestEntryClicked(fullQuest);
	}
	
	public MSPoolable Make (Vector3 origin)
	{
		CBKQuestEntry entry = Instantiate(this, origin, Quaternion.identity) as CBKQuestEntry;
		entry.prefab = this;
		return entry;
	}
	
	public void Pool ()
	{
		MSPoolManager.instance.Pool(this);
	}
}
