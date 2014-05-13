using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// CBKHireEntry
/// @author Rob Giusti
/// </summary>
public class MSHireEntry : MonoBehaviour {

	[SerializeField]
	UILabel occupationName;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UISprite arrow;

	[SerializeField]
	GameObject hiredIcon;

	[SerializeField]
	Color blueColor;

	[SerializeField]
	Color greyColor;

	[SerializeField]
	TweenPosition mover;

	bool activatesRequest;

	int currBuilding;

	public void Init(ResidenceProto proto, bool claimed, int userBuildingId)
	{
		mover.Sample(0, true);

		occupationName.color = blueColor;
		bottomLabel.color = Color.black;

		icon.spriteName = "onjobicon" + proto.occupationName.ToLower();

		Init (proto);

		hiredIcon.SetActive(claimed);
		arrow.gameObject.SetActive(!claimed);

		activatesRequest = !claimed;
		currBuilding = userBuildingId;
	}

	public void Init(ResidenceProto proto, string needs)
	{
		Init (proto);
		hiredIcon.SetActive(false);
		arrow.gameObject.SetActive(false); //TODO: Set arrow sprite to lock
		bottomLabel.text = needs;
		activatesRequest = false;
	}

	public void Init(ResidenceProto proto)
	{
		occupationName.text = proto.occupationName;
		bottomLabel.text = "Adds " + proto.numBonusMonsterSlots + " slots to your residence";
	}

	/// <summary>
	/// When this is clicked, if this is the level that's not claimed but has no needs,
	/// then it's the one that's going to trigger the request dialogue.
	/// </summary>
	void OnClick()
	{
		if (activatesRequest)
		{
			Debug.Log("Clicked Button");
			//MSResidenceManager.instance.OpenRequestDialogue(currBuilding);
		}
	}
}
