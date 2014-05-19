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

	[SerializeField]
	MSUIHelper backButton;

	[SerializeField]
	TweenPosition mover;

	List<MSMiniJobEntry> jobEntries = new List<MSMiniJobEntry>();

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
		Init ();
	}

	public void Init()
	{
		mover.Sample(0, true);

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
	}

	void InitRightAlreadyOnJob()
	{

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
		entry.Init(monster, currJob.miniJob.atkRequired, currJob.miniJob.hpRequired, this);
		goonEntries.Add(entry);

		if (once)
		{
			goonGrid.Reposition();
		}
	}

	public void ClickBack()
	{
		mover.PlayReverse();
		backButton.FadeOutAndOff();
	}

}
