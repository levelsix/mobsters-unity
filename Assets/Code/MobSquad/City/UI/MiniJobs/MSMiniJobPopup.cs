using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobPopup
/// </summary>
public class MSMiniJobPopup : MonoBehaviour {

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
	MSUIHelper tapHint;

	[SerializeField]
	MSMiniJobGoonSlot[] goonSlots;

	[SerializeField]
	GameObject noJobStuff;

	[SerializeField]
	GameObject hasJobStuff;
	
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

	public List<MSMiniJobEntry> jobEntries = new List<MSMiniJobEntry>();

	public List<MSMiniJobGoonie> goonEntries = new List<MSMiniJobGoonie>();

	public List<PZMonster> currTeam = new List<PZMonster>();

	UserMiniJobProto currJob;

	bool ready = false;

	const string greyEngage = "engagedisabled";
	const string greenEngage = "greenmenuoption";
	const string greyArrow = "engagegrayarrow";
	const string greenArrow = "engagearrow";
	static readonly Color greyTextColor = new Color(.5f, .5f, .5f);
	static readonly Color whiteTextColor = Color.white;


	void OnEnable()
	{
		mover.Sample(0, true);

		Init ();
	}

	public void Init()
	{
		SetupJobGrid();

		StartCoroutine(RunTimerUntilJobsReset());
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
			long timeUntilRefresh = MSMiniJobManager.instance.timeUntilRefresh;

			newJobSpawnTimer.text = MSUtil.TimeStringShort(timeUntilRefresh);

			//If it's more than an hour, refresh every minute. Otherwise, every second.
			if (timeUntilRefresh > 60 * 60 * 1000)
			{
				yield return new WaitForSeconds(60);
			}
			else
			{
				yield return new WaitForSeconds(1);
			}
		}
	}

	public void OnJobClicked(UserMiniJobProto job)
	{
		backButton.TurnOn();
		backButton.FadeIn();

		currJob = job;

		mover.PlayForward();

		foreach (var item in goonSlots) 
		{
			item.Reset();
		}

		currTeam.Clear();

		foreach (var item in goonEntries) 
		{
			item.Pool();
		}

		goonEntries.Clear();

		if (MSMiniJobManager.instance.currActiveJob != null
		    && MSMiniJobManager.instance.currActiveJob.userMiniJobId > 0)
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
		reqAtkLabel.text = "REQUIRED ATTACK: " + totalAttack + "/" + currJob.miniJob.atkRequired;

		reqHpBar.fill = totalHp / currJob.miniJob.hpRequired;
		reqHpLabel.text = "REQUIRED HP: " + totalHp + "/" + currJob.miniJob.hpRequired;

		if (totalAttack > currJob.miniJob.atkRequired && totalHp > currJob.miniJob.hpRequired)
		{
			engageButton.spriteName = greenEngage;
			engageArrow.spriteName = greenArrow;
			engageText.color = whiteTextColor;
			ready = true;
		}
		else
		{
			engageButton.spriteName = greyEngage;
			engageArrow.spriteName = greyArrow;
			engageText.color = greyTextColor;
			ready = false;
		}
	}

	public void EngageButton()
	{
		if (ready)
		{
			MSMiniJobManager.instance.BeginJob(currJob, currTeam);
			Init();
			mover.PlayReverse();
		}
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
		tapHint.ResetAlpha(true);
	}

	void InitRightAlreadyOnJob()
	{
		currJobName.text = MSMiniJobManager.instance.currActiveJob.miniJob.name;
		StartCoroutine(UpdateStupidFuckingButton());
	}

	public bool TryPickMonster(PZMonster monster, MSMiniJobGoonPortrait portrait)
	{
		//Debug.LogWarning("Trying picking!");

		//Find open slot
		foreach (var item in goonSlots) 
		{
			if (item.gameObject.activeSelf && item.isOpen)
			{
				item.InsertMonster(monster, portrait);
				currTeam.Add(monster);
				CalculateBars ();
				tapHint.FadeOut();
				return true;
			}
		}

		//If no open slots, we didn't actually move the monster
		return false;
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
			foreach (var item in goonSlots) 
			{
				if (!item.isOpen)
				{
					return;
				}
			}
			tapHint.FadeIn();
		}
	}

	public void ClickBack()
	{
		mover.PlayReverse();
		backButton.FadeOutAndOff();
	}

	public void CheckNoJobs()
	{
		noJobsLabel.Fade(jobEntries.Count == 0);
	}

	/// <summary>
	/// Fuck this button and fuck this code
	/// </summary>
	IEnumerator UpdateStupidFuckingButton()
	{
		while (MSMiniJobManager.instance.currActiveJob.timeCompleted == 0)
		{
			buttonSprite.normalSprite = "purplemenuoption";
			buttonLabel.text = "Finish\n(g) " + MSMath.GemsForTime(MSMiniJobManager.instance.timeLeft);
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
		if (MSMiniJobManager.instance.currActiveJob.timeCompleted == 0)
		{
			MSMiniJobManager.instance.CompleteCurrentJobWithGems();
		}
		else
		{
			MSMiniJobManager.instance.RedeemCurrJob();
			mover.PlayReverse();
		}

	}
}
