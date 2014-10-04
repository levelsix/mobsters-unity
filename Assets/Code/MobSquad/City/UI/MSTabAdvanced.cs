using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(UIButton))]
public class MSTabAdvanced : MSTab {

	[SerializeField]
	Color activeText;

	[SerializeField]
	Color inactiveText;

	[SerializeField]
	MSTabAdvanced[] tabList;

	[SerializeField]
	GameObject tabContents;
	
	override public void InitActive()
	{
		base.InitActive();
		label.color = activeText;
		tabContents.SetActive(true);
	}

	override public void InitInactive()
	{
		base.InitInactive();
		label.color = inactiveText;
		tabContents.SetActive(false);
	}

	public void OnClick()
	{
		InitActive();
		foreach(MSTabAdvanced tab in tabList)
		{
			tab.InitInactive();
		}
	}
}
