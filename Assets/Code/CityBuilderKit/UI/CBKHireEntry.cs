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

	public void Init(ResidenceProto proto, bool claimed)
	{
		Init (proto);
		claim.SetActive(claimed);
		arrow.SetActive(!claimed);
		requirement.text = " ";
	}

	public void Init(ResidenceProto proto, string needs)
	{
		Init (proto);
		claim.SetActive(false);
		arrow.SetActive(false);
		requirement.text = needs;
	}

	public void Init(ResidenceProto proto)
	{
		occupationName.text = proto.occupationName;
		slots.text = proto.numBonusMonsterSlots + " Bonus Slots";
	}
}
