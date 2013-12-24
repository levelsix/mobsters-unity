using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CBKGoonGrid : UIGrid {

	[SerializeField]
	List<CBKGridItem> items = new List<CBKGridItem>();

	[SerializeField]
	float spaceBetween;

	[SerializeField]
	float startingX;

	Transform trans;

	void Awake()
	{
		trans = transform;
	}

	public void AddItemToGrid(CBKGridItem item)
	{
		items.Add(item);
		Reposition();
	}

	public void ClearGrid()
	{
		items.Clear();
	}

	protected override void Position (ref int x, ref int y)
	{
		if (trans == null)
		{
			trans = transform;
		}

		trans.localPosition = Vector3.zero;
		float runningX = startingX + spaceBetween;
		foreach (CBKGridItem item in items) 
		{
			if (!NGUITools.GetActive(item.gObj) && hideInactive) continue;
			
			item.trans.localPosition = new Vector3(runningX + item.width/2, 0);
			
			runningX += item.width + spaceBetween;
		}
	}

}
