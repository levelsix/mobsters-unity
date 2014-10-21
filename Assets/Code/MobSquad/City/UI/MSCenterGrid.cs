using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSUiCenterGroup
/// </summary>
public class MSCenterGrid : UIGrid {

	[ContextMenu("Execute")]
	override public void Reposition ()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		
		if (!mInitDone) Init();
		
		mReposition = false;
		Transform myTrans = transform;
		
		int x = 0;
		int y = 0;
		
		if (sorted)
		{
			List<Transform> list = new List<Transform>();
			
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				if (t && (!hideInactive || NGUITools.GetActive(t.gameObject))) list.Add(t);
			}
			Sort(list);
			
			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				Transform t = list[i];
				
				if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;
				
				float depth = t.localPosition.z;
				Vector3 pos = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x - (list.Count-1) * cellWidth / 2, -cellHeight * y, depth) :
						new Vector3(cellWidth * y, -cellHeight * x, depth);
				
				if (animateSmoothly && Application.isPlaying)
				{
					SpringPosition.Begin(t.gameObject, pos, 15f);
				}
				else t.localPosition = pos;
				
				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		else
		{
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				
				if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;
				
				float depth = t.localPosition.z;
				Vector3 pos = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x - (myTrans.childCount-1) * cellWidth / 2, -cellHeight * y, depth) :
						new Vector3(cellWidth * y, -cellHeight * x + (myTrans.childCount-1) * cellHeight/2, depth);
				
				if (animateSmoothly && Application.isPlaying)
				{
					SpringPosition.Begin(t.gameObject, pos, 15f);
				}
				else t.localPosition = pos;
				
				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		
		if (keepWithinPanel && mPanel != null)
			mPanel.ConstrainTargetToBounds(myTrans, true);
		
		if (onReposition != null)
			onReposition();
	}
}
