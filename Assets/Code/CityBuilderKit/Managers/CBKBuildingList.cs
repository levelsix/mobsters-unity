using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Singleton list of StructProtos for each building, until we have
/// the server stuff up and running.
/// </summary>
public class CBKBuildingList {
	
	public FullStructureProto chineseRestaurant = new FullStructureProto();
	public FullStructureProto sevenEleven = new FullStructureProto();
	public FullStructureProto starBucks = new FullStructureProto();
	public FullStructureProto apartmentComplex = new FullStructureProto();
	
	private static CBKBuildingList _buildingList;
	
	public static CBKBuildingList instance
	{
		get
		{
			if (_buildingList == null)
			{
				_buildingList = new CBKBuildingList();
			}
			return _buildingList;
		}
	}
	
	private CBKBuildingList()
	{
		sevenEleven.name = "Convenience Store";
		sevenEleven.structId = 0;
		sevenEleven.income = 1000;
		sevenEleven.minutesToGain = 1;
		sevenEleven.minutesToBuild = 1;
		sevenEleven.minutesToUpgradeBase = 15;
		sevenEleven.coinPrice = 100;
		sevenEleven.xLength = 3;
		sevenEleven.yLength = 3;
		
		chineseRestaurant.name = "Chinese Restaurant";
		chineseRestaurant.structId = 1;
		chineseRestaurant.income = 100;
		chineseRestaurant.minutesToGain = 5;
		chineseRestaurant.minutesToBuild = 5;
		chineseRestaurant.minutesToUpgradeBase = 30;
		chineseRestaurant.coinPrice = 10;
		chineseRestaurant.xLength = 3;
		chineseRestaurant.yLength = 3;
		
		starBucks.name = "Coffee Shop";
		starBucks.structId = 2;
		starBucks.income = 5000;
		starBucks.minutesToGain = 60;
		starBucks.minutesToBuild = 30;
		starBucks.minutesToUpgradeBase = 90;
		starBucks.coinPrice = 100000;
		starBucks.xLength = 3;
		starBucks.yLength = 3;
		
		apartmentComplex.name = "Lofts";
		apartmentComplex.structId = 3;
		apartmentComplex.income = 50;
		apartmentComplex.minutesToGain = 5;
		apartmentComplex.minutesToBuild = 720;
		apartmentComplex.minutesToUpgradeBase = 6000;
		apartmentComplex.diamondPrice = 5000;
		apartmentComplex.xLength = 3;
		apartmentComplex.yLength = 3;
		
		CBKDataManager.instance.Load(sevenEleven, sevenEleven.structId);
	}
	
}
