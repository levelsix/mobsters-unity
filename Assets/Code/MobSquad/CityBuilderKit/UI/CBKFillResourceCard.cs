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
			amount = CBKResourceManager.maxes[(int)resourceToFill-1] - CBKResourceManager.resources[(int)resourceToFill-1];
			button.enabled = amount < 0;
		}
		else
		{
			amount = Mathf.CeilToInt(CBKResourceManager.maxes[(int)resourceToFill-1] * percent);
			button.enabled = CBKResourceManager.resources[(int)resourceToFill-1] + amount < CBKResourceManager.maxes[(int)resourceToFill-1];
		}

		
		cost = Mathf.CeilToInt(amount * MSWhiteboard.constants.gemsPerResource);
		costLabel.text = "(G)" + cost;
		amountLabel.text = ((resourceToFill==ResourceType.CASH) ? "$" : "(O)") + amount;
	}

	void OnClick()
	{
		if (button.enabled)
		{
			if (CBKResourceManager.instance.Spend(ResourceType.GEMS, cost))
			{
				CBKResourceManager.instance.Collect(resourceToFill, amount);
			}
		}
	}
}
