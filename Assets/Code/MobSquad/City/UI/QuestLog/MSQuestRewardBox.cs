using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSQuestRewardBox : MonoBehaviour, MSPoolable {
	
	GameObject gameObj;
	Transform trans;
	MSQuestRewardBox _prefab;
	
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
	
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as MSQuestRewardBox;
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
	
	public MSPoolable Make (Vector3 origin)
	{
		MSQuestRewardBox reward = Instantiate(this, origin, Quaternion.identity) as MSQuestRewardBox;
		reward.prefab = this;
		return reward;
	}
	
	public void Pool ()
	{
		MSPoolManager.instance.Pool(this);
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
	
	public void Init(MonsterProto monsterReward)
	{
		//TODO: This!!!
	}
	
	
}
