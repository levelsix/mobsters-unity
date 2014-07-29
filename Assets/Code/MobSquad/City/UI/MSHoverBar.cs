using UnityEngine;
using System.Collections;
/// <summary>
/// CBK hover bar.
/// </summary>
public class MSHoverBar : MonoBehaviour {

	[SerializeField]
	GameObject ArrowUp;

	[SerializeField]
	GameObject ArrowDown;

	[SerializeField]
	GameObject ArrowLeft;

	[SerializeField]
	GameObject ArrowRight;

	GameObject[] arrows = new GameObject[4];

	MSBuilding currBuilding;
	
	Transform trans;
	
	GameObject gameObj;

	const float HORIZONTAL_SCALE = 24.0f;
	const float VERTICAL_SCALE = 17.5f;

	//this is an estimation of the center of a building in the local transform
	static readonly Vector3 BUILDING_CENTER = new Vector3 ( 10.0f, -152.5f ,200.0f);

	static readonly Vector3 BUILDING_OFFSET = new Vector3(0, 4.5f, 0);
	
	void Awake()
	{
		MSActionManager.Town.OnBuildingSelect += AttachToPlayerStructure;
		trans = transform;
		gameObj = gameObject;

		arrows [0] = ArrowDown;
		arrows [1] = ArrowLeft;
		arrows [2] = ArrowRight;
		arrows [3] = ArrowUp;
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
			
			if (building.locallyOwned && building.combinedProto.structInfo.structType != com.lvl6.proto.StructureInfoProto.StructType.MINI_JOB)
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

			ArrowDown.transform.localPosition = new Vector3(BUILDING_CENTER.x + HORIZONTAL_SCALE * building.length, BUILDING_CENTER.y - VERTICAL_SCALE * building.width, BUILDING_CENTER.z);
			ArrowLeft.transform.localPosition = new Vector3(BUILDING_CENTER.x - HORIZONTAL_SCALE * building.width, BUILDING_CENTER.y - VERTICAL_SCALE * building.width, BUILDING_CENTER.z);
			ArrowRight.transform.localPosition = new Vector3(BUILDING_CENTER.x + HORIZONTAL_SCALE * building.width, BUILDING_CENTER.y + VERTICAL_SCALE * building.width, BUILDING_CENTER.z);
			ArrowUp.transform.localPosition = new Vector3(BUILDING_CENTER.x - HORIZONTAL_SCALE * building.length, BUILDING_CENTER.y + VERTICAL_SCALE * building.width, BUILDING_CENTER.z);

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
