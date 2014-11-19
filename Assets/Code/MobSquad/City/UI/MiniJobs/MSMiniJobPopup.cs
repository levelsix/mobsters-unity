using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobPopup
/// </summary>
public class MSMiniJobPopup : MSFunctionalScreen {

	public static MSMiniJobPopup instance;

	#region Prefabs

	[SerializeField]
	MSMiniJobEntry jobEntryPrefab;

	[SerializeField]
	MSMiniJobGoonie goonEntryPrefab;

	#endregion

	#region Job Screen

	[SerializeField]
	UIGrid miniJobGrid;

	[SerializeField]
	MSUIHelper noJobsLabel;

	[SerializeField]
	UILabel newJobSpawnTimer;

	#endregion

	#region Details Screen

	public UIGrid goonGrid;

	[SerializeField]
	MSFillBar reqHpBar;

	[SerializeField]
	UILabel reqHpLabel;

	[SerializeField]
	MSFillBar reqAtkBar;

	[SerializeField]
	UILabel reqAtkLabel;

	[SerializeField]
	UISprite engageButton;

	[SerializeField]
	UISprite engageArrow;

	[SerializeField]
	UILabel engageText;

	[SerializeField]
	UILabel time;

	[SerializeField]
	GameObject noJobStuff;

	[SerializeField]
	GameObject hasJobStuff;

	[SerializeField]
	UIGrid teamGrid;

	[SerializeField]
	UILabel allSlotsLabel;

	[SerializeField]
	UILabel currSlotsLabel;

	[SerializeField]
	MSUIHelper currSlotsLabelBg;

	[SerializeField]
	UISprite arrowSprite;

	[SerializeField]
	Color greenTextColor;

	[SerializeField]
	Color redTextColor;

	#endregion

	#region Busy On Job Stuff

	[SerializeField]
	UILabel currJobName;

	[SerializeField]
	UILabel currJobTime;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UIButton buttonSprite;

	#endregion

	[SerializeField]
	MSUIHelper backButton;

	[SerializeField]
	TweenPosition mover;

	#region Collection Screen
	[SerializeField]
	MSSimplePoolable prizePrefab;

	[SerializeField]
	GameObject collectionObject;

	[SerializeField]
	GameObject leftObject;

	[SerializeField]
	List<MSMiniJobHurtGoon> hurtGoons = new List<MSMiniJobHurtGoon>();

	[SerializeField]
	Transform prizeParent;

	List<PZPrize> prizes = new List<PZPrize>();

	public MSMiniJobEntry curJobEntry;

	[SerializeField]
	int PRIZE_SIZE = 110;
	
	[SerializeField]
	int PRIZE_BUFFER = 5;

	[SerializeField]
	MSLoadLock collectLoadLock;
	#endregion

	public List<MSMiniJobEntry> jobEntries = new List<MSMiniJobEntry>();

	public List<MSMiniJobGoonie> goonEntries = new List<MSMiniJobGoonie>();

	public List<PZMonster> currTeam = new List<PZMonster>();

	UserMiniJobProto currJob;

	bool ready = false;

	const string greyEngage = "engagedisabled";
	const string greenEngage = "greenmenuoption";
	const string greyEngageArrow = "engagegrayarrow";
	const string greenEngageArrow = "engagearrow";
	static readonly Color greyTextColor = new Color(.5f, .5f, .5f);
	static readonly Color whiteTextColor = Color.white;

	void Awake()
	{
		instance = this;
		MSActionManager.MiniJob.OnMiniJobComplete += CheckPageChange;
		MSActionManager.MiniJob.OnMiniJobBeginResponse += EngageResponse;
	}

	void OnDestroy()
	{
		MSActionManager.MiniJob.OnMiniJobComplete -= CheckPageChange;
		MSActionManager.MiniJob.OnMiniJobBeginResponse -= EngageResponse;
	}

	void OnEnable()
	{
		mover.Sample(0, true);

		Init ();

	}
	
	/// <summary>
	/// Check if the minijob menu is open.  If it is, change to the collection screen
	/// </summary>
	public void CheckPageChange()
	{
		if (gameObject.activeSelf)
		{
			Init();
		}
	}

	public override bool IsAvailable ()
	{
		return MSMiniJobManager.instance.currJobCenter != null && MSMiniJobManager.instance.currJobCenter.structInfo.level > 0;
	}

	public override void Init()
	{
		backButton.TurnOff();
		SetupJobGrid();
		InitCollectionScreen();
		StartCoroutine(RunTimerUntilJobsReset());
	}

	void InitCollectionScreen()
	{
		if(MSMiniJobManager.instance.isCompleted)
		{
			MSMiniJobManager.instance.SetTeamAndDamage();
			foreach(var item in hurtGoons)
			{
				item.gameObject.SetActive(false);
			}

			foreach(PZPrize prize in prizes)
			{
				prize.GetComponent<MSSimplePoolable>().Pool();
			}
			prizes.Clear();

			if(MSMiniJobManager.instance.currActiveJob.miniJob.cashReward > 0)
			{
				PZPrize prize;
				prize = MSPoolManager.instance.Get<PZPrize>(prizePrefab, prizeParent);
				prize.InitCash(MSMiniJobManager.instance.currActiveJob.miniJob.cashReward);
				prizes.Add(prize);
			}

			if(MSMiniJobManager.instance.currActiveJob.miniJob.oilReward > 0)
			{
				PZPrize prize;
				prize = MSPoolManager.instance.Get<PZPrize>(prizePrefab, prizeParent);
				prize.InitOil(MSMiniJobManager.instance.currActiveJob.miniJob.cashReward);
				prizes.Add(prize);
			}

			if(MSMiniJobManager.instance.currActiveJob.miniJob.gemReward > 0)
			{
				PZPrize prize;
				prize = MSPoolManager.instance.Get<PZPrize>(prizePrefab, prizeParent);
				prize.InitDiamond(MSMiniJobManager.instance.currActiveJob.miniJob.gemReward);
				prizes.Add(prize);
			}

			if(MSMiniJobManager.instance.currActiveJob.miniJob.monsterIdReward > 0)
			{
				PZPrize prize;
				prize = MSPoolManager.instance.Get<PZPrize>(prizePrefab, prizeParent);
				prize.InitEnemy(MSMiniJobManager.instance.currActiveJob.miniJob.monsterIdReward);
				prizes.Add(prize);
			}

			float spaceNeeded = prizes.Count * PRIZE_SIZE + (prizes.Count-1) * PRIZE_BUFFER;
			for (int i = 0; i < prizes.Count; i++) 
			{
				Vector3 endPosition = new Vector3(i * (PRIZE_SIZE + PRIZE_BUFFER) - spaceNeeded/2, 0, 0);
				prizes[i].transform.localPosition = endPosition;
				prizes[i].label.alpha = 1f;
			}

			for(int i = 0; i < MSMiniJobManager.instance.teamToDamage.Count; i++)
			{
				hurtGoons[i].Init(MSMiniJobManager.instance.teamToDamage[i] ,MSMiniJobManager.instance.damageDelt[i]);
			}
			StartCoroutine(FadeFromListingsToCollection());

		}
		else
		{
			StartCoroutine(FadeFromCollectionToListings());
		}
	}

	void SetupJobGrid()
	{
		foreach (var item in jobEntries) 
		{
			item.Pool();
		}

		jobEntries.Clear();

		foreach (var item in MSMiniJobManager.instance.userMiniJobs) 
		{
			InsertJobEntry(item);
		}
	
		miniJobGrid.Reposition();

		CheckNoJobs();
	}

	void InsertJobEntry(UserMiniJobProto job)
	{
		MSMiniJobEntry entry = (MSPoolManager.instance.Get(jobEntryPrefab.GetComponent<MSSimplePoolable>(),
		                                                  Vector3.zero,
		                                                   miniJobGrid.transform) as MSSimplePoolable).GetComponent<MSMiniJobEntry>();
		entry.transform.localScale = Vector3.one;
		entry.Init(job, this);
		jobEntries.Add(entry);
	}

	IEnumerator RunTimerUntilJobsReset()
	{
		while(true)
		{
			newJobSpawnTimer.text = MSUtil.TimeStringShort(MSMiniJobManager.instance.timeUntilRefresh);
			yield return null;
		}
	}

	public void OnJobClicked(UserMiniJobProto job)
	{
		backButton.TurnOn();
		backButton.FadeIn();

		currJob = job;

		mover.PlayForward();

		foreach (var item in teamGrid.GetComponentsInChildren<MSMiniJobGoonPortrait>()) 
		{
			item.Pool();
		}

		currTeam.Clear();

		foreach (var item in goonEntries) 
		{
			item.Pool();
		}

		goonEntries.Clear();

		if (MSMiniJobManager.instance.currActiveJob != null
		    && !MSMiniJobManager.instance.currActiveJob.userMiniJobUuid.Equals(""))
		{
			hasJobStuff.SetActive(true);
			noJobStuff.SetActive(false);
			InitRightAlreadyOnJob();
		}
		else
		{
			hasJobStuff.SetActive(false);
			noJobStuff.SetActive(true);
			InitRightPickGoons();
		}

		CalculateBars();
	}

	public void CalculateBars()
	{
		float totalAttack = 0;
		float totalHp = 0;

		foreach (var mobster in currTeam) 
		{
			totalAttack += Mathf.FloorToInt(mobster.totalDamage);
			totalHp += mobster.currHP;
		}

		reqAtkBar.fill = totalAttack / currJob.miniJob.atkRequired;
		reqAtkLabel.text = "REQ. ATK: " + totalAttack + "/" + currJob.miniJob.atkRequired;

		reqHpBar.fill = totalHp / currJob.miniJob.hpRequired;
		reqHpLabel.text = "REQ. HP: " + totalHp + "/" + currJob.miniJob.hpRequired;

		if (totalAttack >= currJob.miniJob.atkRequired && totalHp >= currJob.miniJob.hpRequired)
		{
			engageButton.spriteName = greenEngage;
			engageArrow.spriteName = greenEngageArrow;
			engageText.color = whiteTextColor;
			ready = true;
		}
		else
		{
			engageButton.spriteName = greyEngage;
			engageArrow.spriteName = greyEngageArrow;
			engageText.color = greyTextColor;
			ready = false;
		}
	}

	public void EngageButton()
	{
		if (ready)
		{
			MSMiniJobManager.instance.BeginJob(currJob, currTeam);
			engageButton.GetComponent<MSLoadLock>().Lock();
		}
	}

	/// <summary>
	/// Unlock the engage button
	/// </summary>
	void EngageResponse()
	{
		engageButton.GetComponent<MSLoadLock>().Unlock();
		Init();
		mover.PlayReverse();
	}

	void InitRightPickGoons()
	{
		foreach (var item in MSMonsterManager.instance.userMonsters) 
		{
			if (item.monsterStatus == MonsterStatus.HEALTHY || item.monsterStatus == MonsterStatus.INJURED)
			{
				InsertGoonEntry(item);
			}
		}
		goonGrid.Reposition();
		time.text = MSUtil.TimeStringShort(currJob.durationSeconds * 1000);
		UpdateArrow(true);
	}

	void InitRightAlreadyOnJob()
	{
		currJobName.text = MSMiniJobManager.instance.currActiveJob.miniJob.name;
		StartCoroutine(UpdateJobNotDoneButton());
	}

	public bool TryPickMonster(PZMonster monster, MSMiniJobGoonPortrait portrait)
	{
		//Debug.LogWarning("Trying picking!");
		if (currTeam.Count >= currJob.miniJob.maxNumMonstersAllowed)
		{
			MSActionManager.Popup.DisplayRedError("You can't send any more Toons on this mini job.");
			return false;
		}

		//item.InsertMonster(monster, portrait);
		portrait.transform.parent = teamGrid.transform;
		teamGrid.Reposition();
		currTeam.Add(monster);
		CalculateBars ();
		UpdateArrow(false);
		return true;
	}

	public void RemoveFromTeam(MSMiniJobGoonPortrait portrait)
	{
		currTeam.Remove(portrait.monster);
		MSMiniJobGoonie entry = (MSPoolManager.instance.Get(goonEntryPrefab.GetComponent<MSSimplePoolable>(),
		                                                    Vector3.zero,
		                                                    goonGrid.transform) as MSSimplePoolable).GetComponent<MSMiniJobGoonie>();
		entry.transform.localPosition = Vector3.zero;
		entry.transform.localScale = Vector3.one;
		entry.Init(portrait, currJob.miniJob.hpRequired, currJob.miniJob.atkRequired, this);
		goonEntries.Add (entry);

		goonGrid.Reposition();
		teamGrid.Reposition();
		UpdateArrow(false);

		CalculateBars();
	}

	public void InsertGoonEntry(PZMonster monster, bool once = false)
	{
		MSMiniJobGoonie entry = (MSPoolManager.instance.Get(goonEntryPrefab.GetComponent<MSSimplePoolable>(),
		                                                   Vector3.zero,
		                                                   goonGrid.transform) as MSSimplePoolable).GetComponent<MSMiniJobGoonie>();
		entry.transform.localPosition = Vector3.zero;
		entry.transform.localScale = Vector3.one;

		entry.Init(monster, currJob.miniJob.hpRequired, currJob.miniJob.atkRequired, this);
		goonEntries.Add(entry);



		if (once)
		{
			goonGrid.Reposition();
		}
	}

	void UpdateArrow(bool immediate)
	{
		allSlotsLabel.text = currJob.miniJob.maxNumMonstersAllowed + " Slot" + (currJob.miniJob.maxNumMonstersAllowed > 1 ? "s" : "") + " Available";
		arrowSprite.spriteName = MSHealScreen.GREEN_ARROW;

		if (currTeam.Count > 0)
		{
			teamGrid.GetComponent<MSUIHelper>().FadeIn();
			allSlotsLabel.GetComponent<MSUIHelper>().FadeOut();
			int slotsLeft = currJob.miniJob.maxNumMonstersAllowed - currTeam.Count;
			if (slotsLeft > 0)
			{
				currSlotsLabel.text = slotsLeft + "SLOT" + (slotsLeft>1?"S":"") + "\nOPEN";
				currSlotsLabel.color = greenTextColor;
				currSlotsLabel.effectColor = Color.white;
				currSlotsLabelBg.TurnOn();
			}
			else
			{
				arrowSprite.spriteName = MSHealScreen.RED_ARROW;
				currSlotsLabel.text = "SLOTS\nFULL";
				currSlotsLabel.color = redTextColor;
				currSlotsLabel.effectColor = Color.black;
				currSlotsLabelBg.TurnOff();
			}
		}
		else
		{
			if (immediate)
			{
				teamGrid.GetComponent<MSUIHelper>().TurnOff();
				allSlotsLabel.GetComponent<MSUIHelper>().TurnOn();
			}
			else
			{
				teamGrid.GetComponent<MSUIHelper>().FadeOutAndOff();
				allSlotsLabel.GetComponent<MSUIHelper>().FadeIn();
			}
		}
	}

	public void CheckNoJobs()
	{
		noJobsLabel.Fade(jobEntries.Count == 0);
	}

	/// <summary>	
	/// If the player clicks a job while they have one already running,
	/// it will bring them to a side-screen where there's a button that should function
	/// the same as the button on the job entry. However, since that functionality was coded
	/// into MiniJobEntry, we've got to replicate (somewhat) it here.
	/// </summary>
	IEnumerator UpdateJobNotDoneButton()
	{
		while (MSMiniJobManager.instance.currActiveJob.timeCompleted == 0)
		{
			buttonSprite.normalSprite = "purplemenuoption";
			buttonLabel.text = "Finish\n(g) " + MSMath.GemsForTime(MSMiniJobManager.instance.timeLeft, false);
			currJobTime.text = MSUtil.TimeStringShort(MSMiniJobManager.instance.timeLeft);
			currJobTime.color = Color.black;
			yield return new WaitForSeconds(1);
		}

		currJobTime.text = "Complete!";
		currJobTime.color = MSColors.cashTextColor;
		buttonLabel.text = "Collect!";
		buttonSprite.normalSprite = "greenmenuoption";
	}

	/// <summary>
	/// If the player clicks a job while they have one already running,
	/// it will bring them to a side-screen where there's a button that should function
	/// the same as the button on the job entry. However, since that functionality was coded
	/// into MiniJobEntry, we've got to replicate (somewhat) it here.
	/// </summary>
	public void ClickJobNotDoneButton()
	{
		Finish();
	}

	void Finish()
	{
		if (MSMiniJobManager.instance.currActiveJob.timeCompleted == 0)
		{
			buttonSprite.GetComponent<MSLoadLock>().Lock();
			MSMiniJobManager.instance.DoCompleteCurrentJobWithGems(delegate {
				buttonSprite.GetComponent<MSLoadLock>().Unlock();
				mover.PlayReverse();
			});

		}
	}

	public void ClickCollectButton()
	{
		StartCoroutine(Collect());
	}

	IEnumerator Collect()
	{
		collectLoadLock.Lock();
		yield return MSMiniJobManager.instance.RedeemCurrJob();
		collectLoadLock.Unlock();

		SetupJobGrid();
		StartCoroutine(RunTimerUntilJobsReset());
		
		StartCoroutine(FadeFromCollectionToListings());
	}

	public IEnumerator FadeFromCollectionToListings()
	{
		float duration = 0.3f;

		leftObject.SetActive(true);
		leftObject.GetComponent<UIWidget>().alpha = 0f;
		SetupJobGrid();

		TweenAlpha.Begin(leftObject, duration, 1f);
		TweenAlpha.Begin(collectionObject, duration, 0f);

		yield return new WaitForSeconds(duration);

		collectionObject.SetActive(false);
	}

	public IEnumerator FadeFromListingsToCollection()
	{
		float duration = 0.3f;

		collectionObject.SetActive(true);
		collectionObject.GetComponent<UIWidget>().alpha = 0f;

		TweenAlpha.Begin(collectionObject, duration, 1f);
		TweenAlpha.Begin(leftObject, duration, 0f);

		yield return new WaitForSeconds(duration);

		leftObject.SetActive(false);
	}
}
