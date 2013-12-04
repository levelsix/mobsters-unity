using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKCombinedBuildingProto {

	public int id;
	public StructureInfoProto structInfo;
	public ResourceGeneratorProto generator;
	public ResourceStorageProto storage;
	public HospitalProto hospital;
	public ResidenceProto residence;
	public TownHallProto townHall;

	public CBKCombinedBuildingProto(ResourceGeneratorProto generator)
	{
		this.generator = generator;
		structInfo = generator.structInfo;
		id = structInfo.structId;
	}
	public CBKCombinedBuildingProto(ResourceStorageProto storage)
	{
		this.storage = storage;
		structInfo = storage.structInfo;
		id = structInfo.structId;
	}
	public CBKCombinedBuildingProto(HospitalProto hospital)
	{
		this.hospital = hospital;
		structInfo = hospital.structInfo;
		id = structInfo.structId;
	}
	public CBKCombinedBuildingProto(ResidenceProto residence)
	{
		this.residence = residence;
		structInfo = residence.structInfo;
		id = structInfo.structId;
	}
	public CBKCombinedBuildingProto(TownHallProto townHall)
	{
		this.townHall = townHall;
		structInfo = townHall.structInfo;
		id = structInfo.structId;
	}


}
