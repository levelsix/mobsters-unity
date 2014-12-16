using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSHealScreen : MSFunctionalScreen 
{
	public static MSHealScreen instance;

	public MSMobsterGrid grid;

	[SerializeField]
	Transform healQueueParent;

	[SerializeField]
	MSHospitalQueue hospitalQueuePrefab;

	MSHospitalQueue _currQueue;
	public MSHospitalQueue currQueue
	{
		get
		{
			return _currQueue;
		}
		set
		{
			_currQueue = value;
			leftArrow.SetActive(MSHospitalManager.instance.PreviousHospital(value.hospital) != null);
			rightArrow.SetActive(MSHospitalManager.instance.NextHospital(value.hospital) != null);
			currHospital = value.hospital;
//			Debug.Log ("Changing queue and setting hospital to: " + currHospital.building.userStructProto.userStructUuid);
		}
	}
	
	public UIButton button;

	public static MSHospital currHospital;

	MSLoadLock loadLock;
	
	[SerializeField]
	int tweenDistance;

	[SerializeField]
	GameObject leftArrow;

	[SerializeField]
	GameObject rightArrow;

	long _totalFinishTime;
	public long totalFinishTime
	{
		get
		{
			return _totalFinishTime;
		}
	}

	public long timeLeft;

	public List<MSGoonCard> currHeals = new List<MSGoonCard>();

	bool canCallForHelp = false;

	void Awake()
	{
		instance = this;
	}

	public override void Init ()
	{
		if (currHospital == null)
		{
			currHospital = MSHospitalManager.instance.hospitals[0];
//			Debug.Log("Hospital was null, setting to: " + currHospital.userBuildingData.userStructUuid);
		}

//		Debug.Log("Initting heal with hospital: " + currHospital.userBuildingData.userStructUuid);

		if (currQueue == null)
		{
			_currQueue = MSPoolManager.instance.Get<MSHospitalQueue>(hospitalQueuePrefab, healQueueParent);
			_currQueue.transform.localScale = Vector3.one;
		}

		grid.Init(GoonScreenMode.HEAL);

		_currQueue.transform.localPosition = Vector3.zero;
		_currQueue.Init(currHospital, grid);

		currQueue = _currQueue; //Almost redundant...
	}

	public void Add(MSGoonCard card)
	{
		currQueue.Add(card);
	}

	public void Remove(MSGoonCard card)
	{
		currQueue.Remove(card);

		ClanHelpProto clanHelp = MSClanManager.instance.GetClanHelp(GameActionType.HEAL, card.monster.userMonster.monsterId, card.monster.userMonster.userMonsterUuid);
		if(clanHelp != null)
		{
			MSClanManager.instance.DoEndClanHelp(new List<string>{ clanHelp.clanHelpUuid });
		}
	}

	public override bool IsAvailable ()
	{
		return MSHospitalManager.instance.hospitals.Count > 0;
	}

	void OnDisable()
	{
		if (MSHospitalManager.instance != null)
		{
			MSHospitalManager.instance.DoSendHealRequest();
		}
		rightArrow.SetActive(false);
		leftArrow.SetActive(false);
	}

	public bool AddToonToCurrentQueue(PZMonster toon)
	{
		return MSHospitalManager.instance.AddToHealQueue(toon, currHospital);
	}

	public void NextHospitalQueue()
	{
		currQueue.Slide(false, -tweenDistance);
		MSHospitalQueue newQueue = MSPoolManager.instance.Get<MSHospitalQueue>(hospitalQueuePrefab, healQueueParent);
		newQueue.transform.localScale = Vector3.one;
		newQueue.transform.localPosition = new Vector3(tweenDistance, 0, 0);
		newQueue.Init(MSHospitalManager.instance.NextHospital(currHospital), grid);
		newQueue.Slide (true, 0);
		currQueue = newQueue;
	}

	public void PreviousHospitalQueue()
	{
		currQueue.Slide(false, tweenDistance);
		MSHospitalQueue newQueue = MSPoolManager.instance.Get<MSHospitalQueue>(hospitalQueuePrefab, healQueueParent);
		newQueue.transform.localScale = Vector3.one;
		newQueue.transform.localPosition = new Vector3(-tweenDistance, 0, 0);
		newQueue.Init(MSHospitalManager.instance.PreviousHospital(currHospital), grid);
		newQueue.Slide(true, 0);
		currQueue = newQueue;
	}
}
