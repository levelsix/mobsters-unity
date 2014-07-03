using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[System.Serializable]
public class MSFullBuildingProto {

	public int id;
	public StructureInfoProto structInfo;
	public ResourceGeneratorProto generator;
	public ResourceStorageProto storage;
	public HospitalProto hospital;
	public ResidenceProto residence;
	public TownHallProto townHall;
	public LabProto lab;
	public MiniJobCenterProto miniJobCenter;
	public EvoChamberProto evoChamber;
	public TeamCenterProto teamCenter;

	public MSFullBuildingProto predecessor
	{
		get
		{
			if (structInfo.predecessorStructId == 0)
			{
				return null;
			}
			return MSDataManager.instance.Get(typeof(MSFullBuildingProto), structInfo.predecessorStructId) as MSFullBuildingProto;
		}
	}

	public MSFullBuildingProto successor
	{
		get
		{
			if (structInfo.successorStructId == 0)
			{
				return null;
			}
			return MSDataManager.instance.Get(typeof(MSFullBuildingProto), structInfo.successorStructId) as MSFullBuildingProto;
		}
	}

	//I recurse now. Recursion is cool now. Isn't it?
	public MSFullBuildingProto maxLevel
	{
		get
		{
			if (structInfo.successorStructId == 0)
			{
				return this;
			}
			return successor.maxLevel;
		}
	}

	public MSFullBuildingProto baseLevel
	{
		get
		{
			if (structInfo.predecessorStructId == 0)
			{
				return this;
			}
			return predecessor.baseLevel;
		}
	}

	public MSFullBuildingProto(ResourceGeneratorProto generator)
	{
		this.generator = generator;
		structInfo = generator.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(ResourceStorageProto storage)
	{
		this.storage = storage;
		structInfo = storage.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(HospitalProto hospital)
	{
		this.hospital = hospital;
		structInfo = hospital.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(ResidenceProto residence)
	{
		this.residence = residence;
		structInfo = residence.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(TownHallProto townHall)
	{
		this.townHall = townHall;
		structInfo = townHall.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(LabProto lab)
	{
		this.lab = lab;
		structInfo = lab.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(MiniJobCenterProto jobCenter)
	{
		this.miniJobCenter = jobCenter;
		structInfo = jobCenter.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(EvoChamberProto evoChamber)
	{
		this.evoChamber = evoChamber;
		structInfo = evoChamber.structInfo;
		id = structInfo.structId;
	}
	public MSFullBuildingProto(TeamCenterProto teamCenter)
	{
		this.teamCenter = teamCenter;
		structInfo = teamCenter.structInfo;
		id = structInfo.structId;
	}


}
