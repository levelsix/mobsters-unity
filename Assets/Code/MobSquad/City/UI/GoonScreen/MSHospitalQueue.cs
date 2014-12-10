using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[RequireComponent (typeof(MSSimplePoolable))]
public class MSHospitalQueue : MonoBehaviour 
{
	[SerializeField] 
	MSLoadLock loadLock;

	public UIGrid grid;
	
	[SerializeField]
	MSUIHelper emptyQueueRoot;
	
	[SerializeField]
	MSUIHelper queueRoot;
	
	[SerializeField]
	UILabel slotsLeftLabel;
	
	[SerializeField]
	MSUIHelper slotsLeftLabelBg;
	
	[SerializeField]
	UILabel timeLeftLabel;
	
	[SerializeField]
	UILabel finishNowLabel;
	
	[SerializeField]
	UISprite arrow;

	[SerializeField] 
	UILabel levelLabel;

	[SerializeField]
	UIButton button;

	[SerializeField]
	TweenPosition tweenPos;

	List<MSGoonCard> currHeals = new List<MSGoonCard>();

	public MSHospital hospital;
	
	public const string GREEN_ARROW = "hospitalopenarrow";
	public const string RED_ARROW = "fullhospitalarrow";
	
	const string PURPLE_BUTTON = "purplemenuoption";
	const string ORANGE_BUTTON = "orangemenuoption";
	
	readonly Color WHITE_SHADOW = new Color(1f, 1f, 1f, 0.6f);
	readonly Color BLACK_SHADOW = new Color(0f, 0f ,0f, 0.6f);
	readonly Color ORANGLE_WORDS = new Color(195f/255f, 27f/255f, 0f, 1f);
	
	[SerializeField]
	Color slotsOpenLabelColor;
	
	[SerializeField]
	Color slotsFullLabelColor;

	bool canCallForHelp
	{
		get
		{
			return (hospital.healQueue.Count > 0 && MSClanManager.instance.isInClan
			 && !MSClanManager.instance.HelpAlreadyRequested(GameActionType.HEAL, hospital.healQueue[0].userMonster.monsterId, hospital.healQueue[0].userMonster.userMonsterUuid));
		}
	}

	public void Init(MSHospital hospital, MSMobsterGrid greaterGrid)
	{
		if (hospital == null)
		{
			Debug.LogError("Houston, we have a problem");
			return;
		}

		foreach (var item in currHeals) 
		{
			item.Pool();
		}
		currHeals.Clear();

		this.hospital = hospital;

		//levelLabel.text = "Level " + hospital.building.combinedProto.structInfo.level;

		grid.animateSmoothly = false;
		foreach (var item in hospital.healQueue) 
		{
			MSGoonCard card = greaterGrid.AddCard (item, GoonScreenMode.HEAL);
			card.transform.parent = grid.transform;
			Add (card);
		}
		grid.Reposition();
		grid.animateSmoothly = true;
		
		emptyQueueRoot.ResetAlpha(hospital.healQueue.Count == 0);
		queueRoot.ResetAlpha(hospital.healQueue.Count > 0);
	}

	void Update()
	{
		timeLeftLabel.text = MSUtil.TimeStringShort(hospital.timeLeft);
		
		
		if(hospital.gemsToFinish == 0)
		{
			button.normalSprite = PURPLE_BUTTON;
			finishNowLabel.text = "Finish\n FREE";
			finishNowLabel.effectColor = BLACK_SHADOW;
			finishNowLabel.color = Color.white;
		}
		else if(canCallForHelp)
		{
			finishNowLabel.text = "Get Help!";
			finishNowLabel.effectColor = WHITE_SHADOW;
			finishNowLabel.color = ORANGLE_WORDS;
			button.normalSprite = ORANGE_BUTTON;
		}
		else
		{
			button.normalSprite = PURPLE_BUTTON;
			finishNowLabel.text = "Finish\n(g) " + hospital.gemsToFinish;
			finishNowLabel.effectColor = BLACK_SHADOW;
			finishNowLabel.color = Color.white;
		}
	}

	public void Add(MSGoonCard card)
	{
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeOut();
			queueRoot.FadeIn();
		}

		currHeals.Add (card);

		
		card.transform.parent = grid.transform;
		grid.Reposition();

		RefreshSlots();
	}

	public void Remove(MSGoonCard card)
	{
		
		currHeals.Remove(card);
		
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeIn();
			queueRoot.FadeOut();
		}

		grid.Reposition();

		RefreshSlots();

	}

	void RefreshSlots()
	{
		int slotsRemaining = hospital.queueSize - currHeals.Count;
		if (slotsRemaining <= 0)
		{
			slotsLeftLabel.text = "HOSPITAL\nFULL";
			slotsLeftLabel.color = slotsFullLabelColor;
			slotsLeftLabelBg.TurnOff();
			arrow.spriteName = RED_ARROW;
		}
		else
		{
			slotsLeftLabel.text = slotsRemaining + " SLOT" + (slotsRemaining>1?"S":"") + "\nOPEN";
			slotsLeftLabel.color = slotsOpenLabelColor;
			slotsLeftLabelBg.TurnOn();
			arrow.spriteName = GREEN_ARROW;
		}
	}
	
	public void Button()
	{
		int finishAmount = hospital.gemsToFinish;
		if(canCallForHelp && finishAmount != 0)
		{
			List<ClanHelpNoticeProto> notices = new List<ClanHelpNoticeProto>();
			
			ClanHelpNoticeProto notice = new ClanHelpNoticeProto();
			notice.helpType = GameActionType.HEAL;
			notice.staticDataId = currHeals[0].monster.userMonster.monsterId;
			notice.userDataUuid = currHeals[0].monster.userMonster.userMonsterUuid;
			notices.Add(notice);

			loadLock.Lock();
			MSClanManager.instance.DoSolicitClanHelp(notices,
			                                         MSBuildingManager.currClanHouse.maxHelpersPerSolicitation,
			                                         loadLock.Unlock);
		}
		else
		{
			MSHospitalManager.instance.TrySpeedUpHeal(loadLock, hospital.healQueue, hospital.gemsToFinish);
		}
	}

	public void Slide(bool comingIn, int targetX)
	{
		tweenPos.from = transform.localPosition;
		tweenPos.to = new Vector3(targetX, 0, 0);
		tweenPos.Sample(0, true);
		tweenPos.PlayForward();
		if (!comingIn) tweenPos.onFinished.Add(new EventDelegate(Pool));
		else tweenPos.onFinished.Clear();
	}

	void Pool()
	{
		foreach (var item in currHeals) 
		{
			item.Pool();
		}
		currHeals.Clear();
		GetComponent<MSSimplePoolable>().Pool();
	}
}
