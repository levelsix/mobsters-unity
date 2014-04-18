using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// ResourceCard
/// Used for the Funds menu, for players to click on and buy currency
/// </summary>
public class MSResourceCard : MonoBehaviour {

	[SerializeField]
	ResourceType resourceToFill;

	[SerializeField]
	bool fill;

	[SerializeField]
	float percent;

	[SerializeField]
	UIButton button;

	[SerializeField]
	UILabel costLabel;

	[SerializeField]
	UILabel amountLabel;

	int cost;

	int amount;

	void OnEnable()
	{
		Init ();
		MSActionManager.UI.OnChangeResource[(int)resourceToFill-1] += Init;
	}

	void OnDisable()
	{
		MSActionManager.UI.OnChangeResource[(int)resourceToFill-1] -= Init;
	}

	void Init(int currAmount)
	{
		Init();
	}

	void Init ()
	{
		if (fill) 
		{
			amount = Mathf.Max(0, MSResourceManager.maxes [(int)resourceToFill - 1] - MSResourceManager.resources [(int)resourceToFill - 1]);
			button.isEnabled = amount > 0;
		}
		else 
		{
			amount = Mathf.CeilToInt (MSResourceManager.maxes [(int)resourceToFill - 1] * percent);
			button.isEnabled = MSResourceManager.resources [(int)resourceToFill - 1] + amount < MSResourceManager.maxes [(int)resourceToFill - 1];
		}
		cost = Mathf.CeilToInt (amount * MSWhiteboard.constants.gemsPerResource);
		costLabel.text = "(G)" + cost;
		amountLabel.text = ((resourceToFill == ResourceType.CASH) ? "$" : "(O)") + amount;
	}

	void OnClick()
	{
		if (button.enabled)
		{
			if (MSResourceManager.resources[(int)ResourceType.GEMS-1] >= cost)
			{
				MSResourceManager.instance.SpendGemsForOtherResource(resourceToFill, amount);
			}
		}
	}
}
