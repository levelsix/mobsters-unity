using UnityEngine;
using System.Collections;

public class MSBuildingPrereqEntry : MonoBehaviour {

	[SerializeField] UISprite completionSymbol;

	[SerializeField] UILabel label;

	[SerializeField] GameObject goButton;

	[SerializeField] Color greenLabelColor;
	[SerializeField] Color redLabelColor;

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
		//TODO
	}
}
