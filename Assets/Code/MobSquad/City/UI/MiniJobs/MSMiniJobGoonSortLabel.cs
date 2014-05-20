using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobGoonSortLabel
/// </summary>
public class MSMiniJobGoonSortLabel : MonoBehaviour {
	
	[SerializeField]
	UISprite arrow;

	[SerializeField]
	MSMiniJobGoonSortLabel otherLabel;

	[SerializeField]
	MSMiniJobGoonGrid grid;

	[SerializeField]
	MSMiniJobGoonGrid.SortingMode mode;

	public void OnOtherClicked()
	{
		arrow.alpha = 0;
	}

	void OnClick()
	{
		arrow.alpha = 1;

		//If setting the mode returns true, then the arrow should point down (unflipped)
		arrow.flip = grid.SetMode(mode) ? UISprite.Flip.Nothing : UISprite.Flip.Vertically;
		otherLabel.OnOtherClicked();
	}

}
