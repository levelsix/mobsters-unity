using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// CBKHireEntry
/// @author Rob Giusti
/// </summary>
public class CBKHireEntry : MonoBehaviour {

	[SerializeField]
	UILabel occupationName;

	[SerializeField]
	UILabel slots;

	[SerializeField]
	UILabel requirement;

	[SerializeField]
	GameObject arrow;

	[SerializeField]
	GameObject claim;

	bool activatesRequest;

	int currBuilding;

	public void Init(ResidenceProto proto, bool claimed, int userBuildingId)
	{
		Init (proto);
		claim.SetActive(claimed);
		arrow.SetActive(!claimed);
		requirement.text = " ";
		activatesRequest = !claimed;
		currBuilding = userBuildingId;
	}

	public void Init(ResidenceProto proto, string needs)
	{
		Init (proto);
		claim.SetActive(false);
		arrow.SetActive(false);
		requirement.text = needs;
		activatesRequest = false;
	}

	public void Init(ResidenceProto proto)
	{
		occupationName.text = proto.occupationName;
		slots.text = proto.numBonusMonsterSlots + " Bonus Slots";
	}

	/// <summary>
	/// When this is clicked, if this is the level that's not claimed but has no needs,
	/// then it's the one that's going to trigger the request dialogue.
	/// </summary>
	void OnClick()
	{
		if (activatesRequest)
		{
			MSResidenceManager.instance.OpenRequestDialogue(currBuilding);
		}
	}
}
