using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK quest task entry.
/// </summary>
public class CBKQuestTaskEntry : MonoBehaviour, CBKIPoolable
{
	
	GameObject gameObj;
	Transform trans;
	CBKQuestTaskEntry _prefab;
	
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
			_prefab = value as CBKQuestTaskEntry;
		}
	}
	
	[SerializeField]
	UILabel taskNameLabel;
	
	[SerializeField]
	UILabel numLeftLabel;
	
	[SerializeField]
	UISprite visitOrDoneSprite;
	
	[SerializeField]
	UILabel visitOrDoneLabel;
	
	const string DONE_STRING = "Done!";
	
	const string VISIT_STRING = "Visit";
	
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
	}
	
	public CBKIPoolable Make (Vector3 origin)
	{
		CBKQuestTaskEntry entry = Instantiate(this, origin, Quaternion.identity) as CBKQuestTaskEntry;
		entry.prefab = this;
		return entry;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
	
	public void InitMoneyCollect(int amountCollected, int amountToCollect)
	{
		taskNameLabel.text = "Collect [" + CBKValues.Colors.moneyText +"]$" + amountToCollect + "[-] from buildings";
		
		numLeftLabel.text = amountCollected + "/" + amountToCollect;
		
		SetComplete(amountCollected >= amountToCollect);
	}
	
	public void Init(MinimumUserQuestTaskProto task)
	{
		FullTaskProto fullTask = CBKDataManager.instance.Get(typeof(FullTaskProto), task.taskId) as FullTaskProto;
		
		taskNameLabel.text = fullTask.name;
		
		numLeftLabel.text = task.numTimesActed + "/" + fullTask.numRequiredForCompletion;
		
		SetComplete(task.numTimesActed >= fullTask.numRequiredForCompletion);
	}
	
	public void Init(MinimumUserBuildStructJobProto job)
	{
		BuildStructJobProto buildJob = CBKDataManager.instance.Get (typeof(BuildStructJobProto), job.buildStructJobId) as BuildStructJobProto;
		
		FullStructureProto structure = CBKDataManager.instance.Get(typeof(FullStructureProto), buildJob.structId) as FullStructureProto;
		
		taskNameLabel.text = "Build " + buildJob.quantityRequired + " " + structure.name + "s";
		
		numLeftLabel.text = job.numOfStructUserHas + "/" + buildJob.quantityRequired;
		
		SetComplete(job.numOfStructUserHas >= buildJob.quantityRequired);
	}
	
	public void Init(MinimumUserUpgradeStructJobProto job)
	{
		UpgradeStructJobProto upgradeJob = CBKDataManager.instance.Get(typeof(UpgradeStructJobProto), job.upgradeStructJobId) as UpgradeStructJobProto;
		
		FullStructureProto structure = CBKDataManager.instance.Get(typeof(FullStructureProto), upgradeJob.structId) as FullStructureProto;
		
		taskNameLabel.text = "Upgrade " + structure.name + " to level " + upgradeJob.levelReq;
		
		numLeftLabel.text = job.currentLevel + "/" + upgradeJob.levelReq;
		
		SetComplete(job.currentLevel >= upgradeJob.levelReq);
	}
	
	/// <summary>
	/// Init from the specified job.
	/// DOES NOT WORK
	/// ROB: TODO: Finish this when we have a more clear equip system---------------------------------------------------------------------------------
	/// </summary>
	/// <param name='job'>
	/// Job.
	/// </param>
	public void Init(MinimumUserPossessEquipJobProto job)
	{
		PossessEquipJobProto possJob = CBKDataManager.instance.Get (typeof(PossessEquipJobProto), job.possessEquipJobId) as PossessEquipJobProto;
		
		//FullEquipProto equip = CBKDataManager.instance.Get(typeof(FullEquipProto), possJob.equipId) as FullEquipProto;
		
		//taskNameLabel.text = "Possess " + possJob.quantityReq + " " + equip.name + "s";
		
		//numLeftLabel.text = job.numEquipUserHas + "/" + possJob.quantityReq;
		
		//SetComplete(job.numEquipUserHas >= possJob.quantityReq);
	}
	
	public void Init(MinimumUserDefeatTypeJobProto job)
	{	
		DefeatTypeJobProto defeatJob = CBKDataManager.instance.Get(typeof(DefeatTypeJobProto), job.defeatTypeJobId) as DefeatTypeJobProto;
		
		taskNameLabel.text = "Defeat " + defeatJob.numEnemiesToDefeat + " ";
		
		switch (defeatJob.typeOfEnemy) 
		{
			default:
				taskNameLabel.text += "FINISH THIS ROB";
				break;
		}
		
		numLeftLabel.text = job.numDefeated + "/" + defeatJob.numEnemiesToDefeat;
		
		SetComplete(job.numDefeated >= defeatJob.numEnemiesToDefeat);
	}
	
	public void SetComplete(bool complete)
	{
		if (complete)
		{
			visitOrDoneSprite.alpha = 0;
			visitOrDoneLabel.text = DONE_STRING;
		}
		else
		{
			visitOrDoneSprite.alpha = 1;
			visitOrDoneLabel.text = VISIT_STRING;
		}
	}
	
}
