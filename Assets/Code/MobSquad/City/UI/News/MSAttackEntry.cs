using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSAttackEntry
/// @author Rob Giusti
/// </summary>
public class MSAttackEntry : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	UILabel attackerNameLabel;

	[SerializeField]
	UILabel timeAgoLabel;

	[SerializeField]
	MSMobsterIcon[] team;

	[SerializeField]
	UISprite rankIcon;

	[SerializeField]
	UILabel rankChangeLabel;

	[SerializeField]
	UILabel cashLostLabel;

	[SerializeField]
	UILabel oilLostLabel;

	[SerializeField]
	UILabel revengeLabel;

	[SerializeField]
	GameObject revengeButton;

	[SerializeField]
	Color hasRevengeLabelColor;

	[SerializeField]
	Color noRevengeLabelColor;

	#endregion


}
