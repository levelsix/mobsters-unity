using UnityEngine;
using System.Collections;
/// <summary>
/// CBK hover bar.
/// </summary>
public class MSHoverBar : MonoBehaviour {

	[SerializeField]
	UISprite[] backArrows;

	[SerializeField]
	UISprite[] frontArrows;

	[SerializeField]
	GameObject[] arrows;
	
	MSBuilding currBuilding;
	
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
	
	public void AttachToUnit(MSUnit unit)
	{
		gameObj.SetActive(false);
	}
	
	public void AttachToPlayerStructure(MSBuilding building)
	{
		if (building != null)
		{
			
			currBuilding = building;
			
			if (building.locallyOwned)
			{
				gameObj.SetActive(true);
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
}
