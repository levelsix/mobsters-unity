using UnityEngine;
using System.Collections;

public class CBKQuestEntry : MonoBehaviour, CBKPoolable {
	
	#region Poolable Members & Properties
	
	[HideInInspector]
	public CBKQuestEntry _prefab;
	
	public CBKPoolable prefab {
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
	
	CBKFullQuest fullQuest;
	
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
	}
	
	public void Init(CBKFullQuest quest)
	{


		questName.text = quest.quest.name;
		
		questProgress.text = quest.GetProgressString();
		
		trans.localScale = Vector3.one;
		
		fullQuest = quest;
	}
	
	void OnClick()
	{
		CBKEventManager.UI.OnQuestEntryClicked(fullQuest);
	}
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKQuestEntry entry = Instantiate(this, origin, Quaternion.identity) as CBKQuestEntry;
		entry.prefab = this;
		return entry;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
}
