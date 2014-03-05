using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKFillResourceCard
/// </summary>
public class CBKFillResourceCard : MonoBehaviour {

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
		if (fill)
		{
			amount = MSResourceManager.maxes[(int)resourceToFill-1] - MSResourceManager.resources[(int)resourceToFill-1];
			button.enabled = amount < 0;
		}
		else
		{
			amount = Mathf.CeilToInt(MSResourceManager.maxes[(int)resourceToFill-1] * percent);
			button.enabled = MSResourceManager.resources[(int)resourceToFill-1] + amount < MSResourceManager.maxes[(int)resourceToFill-1];
		}

		
		cost = Mathf.CeilToInt(amount * MSWhiteboard.constants.gemsPerResource);
		costLabel.text = "(G)" + cost;
		amountLabel.text = ((resourceToFill==ResourceType.CASH) ? "$" : "(O)") + amount;
	}

	void OnClick()
	{
		if (button.enabled)
		{
			if (MSResourceManager.instance.Spend(ResourceType.GEMS, cost))
			{
				MSResourceManager.instance.Collect(resourceToFill, amount);
			}
		}
	}
}
