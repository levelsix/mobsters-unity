using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSBuildingPrereqEntry : MonoBehaviour {

	[SerializeField] UISprite completionSymbol;

	[SerializeField] UILabel label;

	[SerializeField] GameObject goButton;

	[SerializeField] Color greenLabelColor;
	[SerializeField] Color redLabelColor;

	[SerializeField] MSTabButton buildButton;

	MSFullBuildingProto prereq;

	const string CHECK_MARK = "requirementmet";
	const string X_MARK = "requirementfailed";

	public void Init(int buildingId, int quantity, bool met)
	{
		gameObject.SetActive(true);

		MSFullBuildingProto building = MSDataManager.instance.Get<MSFullBuildingProto>(buildingId);

		prereq = building;

		if (met)
		{
			completionSymbol.spriteName = CHECK_MARK;
			label.color = greenLabelColor;
			goButton.SetActive(false);
		}
		else
		{
			completionSymbol.spriteName = X_MARK;
			label.color = redLabelColor;
			goButton.SetActive(true);
		}

		label.text = "LVL " + building.structInfo.level + " " + building.structInfo.name;
		if (quantity > 1) label.text = quantity + " " + label.text;
	}

	public void SetEmpty()
	{
		gameObject.SetActive(false);
	}

	public void Go()
	{
		MSActionManager.Popup.CloseAllPopups();

		//Find building that's the best match
		MSBuilding bestMatch = null;
		StructureInfoProto stip;
		foreach (var item in MSBuildingManager.instance.buildings.Values) 
		{
			stip = item.combinedProto.structInfo;
			if (stip.structType == prereq.structInfo.structType
			    && stip.buildResourceType == prereq.structInfo.buildResourceType
			    && stip.level < prereq.structInfo.level
			    && (bestMatch == null || stip.level > bestMatch.combinedProto.structInfo.level))
			{
				bestMatch = item;
			}
		}

		if (bestMatch)
		{
			MSTownCamera.instance.DoCenterOnGroundPos(bestMatch.trans.position);
			MSBuildingManager.instance.SetSelectedBuilding(bestMatch);
		}
		else
		{
			buildButton.OnClick();
		}

	}
}
