using UnityEngine;
using System.Collections;
/// <summary>
/// CBK hover bar.
/// </summary>
public class CBKHoverBar : MonoBehaviour {
	
	[SerializeField]
	UILabel label;
	
	[SerializeField]
	UISprite bar;
	
	[SerializeField]
	UISprite barBackground;
	
	[SerializeField]
	UISprite engageBackground;
	
	CBKBuilding currBuilding;
	
	Transform trans;
	
	GameObject gameObj;
	
	static readonly Vector3 BUILDING_OFFSET = new Vector3(0, 4.5f, 0);
	
	void Awake()
	{
		CBKEventManager.Town.OnBuildingSelect += AttachToPlayerStructure;
		trans = transform;
		gameObj = gameObject;
	}
	
	void OnDestroy()
	{
		CBKEventManager.Town.OnBuildingSelect -= AttachToPlayerStructure;
	}
	
	public void AttachToUnit(CBKUnit unit)
	{
		
	}
	
	public void AttachToPlayerStructure(CBKBuilding building)
	{
		if (building != null)
		{
			gameObj.SetActive(true);
			
			currBuilding = building;
			
			if (building.locallyOwned)
			{
				building.OnUpdateValues += OnUpdateBuildingValues;
				barBackground.MarkAsChanged();
				label.MarkAsChanged();
				bar.MarkAsChanged();
				OnUpdateBuildingValues();
			}
			else
			{
				//
			}
			
			trans.parent = building.trans;
			trans.localPosition = BUILDING_OFFSET;
			trans.Translate(0,0,-2,Space.Self);
		}
		else
		{
			gameObj.SetActive(false);
		}
	}
		
	void OnUpdateBuildingValues()
	{
		if (currBuilding != null)
		{
			if (currBuilding.underConstruction)
			{
				bar.fillAmount = 1 - ((float)currBuilding.upgrade.timeRemaining) / currBuilding.upgrade.TimeToUpgrade(currBuilding.userStructProto.level - 1);
				label.text = CBKUtil.TimeStringShort(currBuilding.upgrade.timeRemaining);
			}
			else
			{
				bar.fillAmount = 1 - ((float)currBuilding.collector.secondsUntilComplete) / currBuilding.collector.timeToGenerate;
				label.text = currBuilding.collector.timeLeftString;
			}
		}
	}
}
