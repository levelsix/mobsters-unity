using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKQuestRewardBox : MonoBehaviour, CBKIPoolable {
	
	GameObject gameObj;
	Transform trans;
	CBKQuestRewardBox _prefab;
	
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
	
	public CBKIPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKQuestRewardBox;
		}
	}
	
	public enum RewardType {MONEY, EXP, EQUIP}
	
	[SerializeField]
	UISprite rewardSprite;
	
	[SerializeField]
	UILabel rewardLabel;
	
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
	}
	
	public CBKIPoolable Make (Vector3 origin)
	{
		CBKQuestRewardBox reward = Instantiate(this, origin, Quaternion.identity) as CBKQuestRewardBox;
		reward.prefab = this;
		return reward;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
	
	public void Init(RewardType type, int amount)
	{
		switch (type) {
			case RewardType.MONEY:
				
				break;
			case RewardType.EXP:
				
				break;
			case RewardType.EQUIP:
				
				break;
			default:
				Debug.LogError("You have fucked up now");
				break;
		}
	}
	
	public void Init(FullUserEquipProto equip)
	{
		//TODO: This!!!
	}
	
	
}
