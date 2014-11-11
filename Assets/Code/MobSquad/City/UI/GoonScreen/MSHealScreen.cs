using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSHealScreen : MSFunctionalScreen 
{
	public static MSHealScreen instance;

	public MSMobsterGrid grid;

	public UIGrid healQueue;

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
	
	public UIButton button;

	MSLoadLock loadLock;

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

	bool canCallForHelp = false;

	void Awake()
	{
		instance = this;
		loadLock = button.GetComponent<MSLoadLock>();
	}

	public override void Init ()
	{
		currHeals.Clear();

		healQueue.animateSmoothly = false;
		grid.Init(GoonScreenMode.HEAL);
		healQueue.Reposition();
		healQueue.animateSmoothly = true;

		emptyQueueRoot.ResetAlpha(MSHospitalManager.instance.healingMonsters.Count == 0);
		queueRoot.ResetAlpha(MSHospitalManager.instance.healingMonsters.Count > 0);
		
		RefreshSlots();
	}

	public void Add(MSGoonCard card)
	{
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeOut();
			queueRoot.FadeIn();
		}
		
		currHeals.Add(card);
		RefreshSlots();

		CheckHelp();
	}

	public void Remove(MSGoonCard card)
	{
		currHeals.Remove(card);
		
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeIn();
			queueRoot.FadeOut();
		}
		RefreshSlots();

		ClanHelpProto clanHelp = MSClanManager.instance.GetClanHelp(GameActionType.HEAL, card.monster.userMonster.monsterId, card.monster.userMonster.userMonsterId);
		if(clanHelp != null)
		{
			MSClanManager.instance.DoEndClanHelp(new List<long>{ clanHelp.clanHelpId });
		}

		CheckHelp();
	}

	void RefreshSlots()
	{
		int slotsRemaining = MSHospitalManager.instance.queueSize - currHeals.Count;
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

	void Update()
	{
		timeLeft = 0;
		foreach (var item in currHeals) 
		{
			timeLeft = Math.Max(item.monster.healTimeLeftMillis, timeLeft);
		}
	
		timeLeftLabel.text = MSUtil.TimeStringShort(timeLeft);
		_totalFinishTime = timeLeft;
		int finishAmount = MSMath.GemsForTime(timeLeft, true);


		if(finishAmount == 0)
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
			finishNowLabel.text = "Finish\n(g) " + finishAmount;
			finishNowLabel.effectColor = BLACK_SHADOW;
			finishNowLabel.color = Color.white;
		}
	}

	public void CheckHelp()
	{
		foreach(MSGoonCard card in currHeals)
		{
			if(!MSClanManager.instance.HelpAlreadyRequested(GameActionType.HEAL, card.monster.userMonster.monsterId, card.monster.userMonster.userMonsterId))
			{
				canCallForHelp = true;
				return;
			}
		}

		canCallForHelp = false;
	}

	public void OnClick()
	{
		CheckHelp();
		int finishAmount = MSMath.GemsForTime(timeLeft, true);
		if(canCallForHelp && finishAmount != 0)
		{
			List<ClanHelpNoticeProto> notices = new List<ClanHelpNoticeProto>();

			foreach(MSGoonCard card in currHeals)
			{
				if(!MSClanManager.instance.HelpAlreadyRequested(GameActionType.HEAL, card.monster.userMonster.monsterId, card.monster.userMonster.userMonsterId))
				{
					ClanHelpNoticeProto notice = new ClanHelpNoticeProto();
					notice.helpType = GameActionType.HEAL;
					notice.staticDataId = card.monster.userMonster.monsterId;
					notice.userDataId = card.monster.userMonster.userMonsterId;
					notices.Add(notice);
				}
			}
			loadLock.Lock();
			MSClanManager.instance.DoSolicitClanHelp(notices,
			                                         MSBuildingManager.clanHouse.combinedProto.clanHouse.maxHelpersPerSolicitation,
			                                         loadLock.Unlock);
			canCallForHelp = false;
		}
		else
		{
			MSHospitalManager.instance.TrySpeedUpHeal(loadLock);
		}
	}

	public override bool IsAvailable ()
	{
		return MSHospitalManager.instance.hospitals.Count > 0;
	}

	void OnDisable()
	{
		MSHospitalManager.instance.DoSendHealRequest();
	}
}
