using UnityEngine;
using System.Collections;
/// <summary>
/// CBK hover bar.
/// </summary>
public class CBKHoverBar : MonoBehaviour {

	[SerializeField]
	UISprite[] backArrows;

	[SerializeField]
	UISprite[] frontArrows;

	[SerializeField]
	GameObject[] arrows;

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
		MSActionManager.Town.OnBuildingSelect += AttachToPlayerStructure;
		trans = transform;
		gameObj = gameObject;
	}
	
	void OnDestroy()
	{
		MSActionManager.Town.OnBuildingSelect -= AttachToPlayerStructure;
	}
	
	public void AttachToUnit(CBKUnit unit)
	{
		gameObj.SetActive(false);
	}
	
	public void AttachToPlayerStructure(CBKBuilding building)
	{
		if (building != null)
		{
			
			currBuilding = building;
			
			if (building.locallyOwned)
			{
				gameObj.SetActive(true);
				building.OnUpdateValues += OnUpdateBuildingValues;
				barBackground.MarkAsChanged();
				label.MarkAsChanged();
				bar.MarkAsChanged();
				OnUpdateBuildingValues();
			}
			else
			{
				gameObj.SetActive(false);
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
		
	void EnableArrows()
	{
		foreach (var item in arrows) 
		{
			item.SetActive(true);
		}
	}

	void DisableArrows()
	{
		foreach (var item in arrows) 
		{
			item.SetActive(false);
		}
	}

	void OnUpdateBuildingValues()
	{
		if (currBuilding != null && currBuilding.userStructProto != null)
		{
			if (!currBuilding.userStructProto.isComplete)
			{
				bar.fillAmount = 1 - ((float)currBuilding.upgrade.timeRemaining) / currBuilding.upgrade.TimeToUpgrade(1);//currBuilding.userStructProto.level - 1);
				label.text = MSUtil.TimeStringShort(currBuilding.upgrade.timeRemaining);
			}
			else
			{
				//bar.fillAmount = 1 - ((float)currBuilding.collector.secondsUntilComplete) / currBuilding.collector.timeToGenerate;
				//label.text = currBuilding.collector.timeLeftString;
			}
		}
	}
}
