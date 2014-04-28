using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRoadSpace
/// </summary>
public class MSRoadSpace : MSITakesGridSpace {

	public bool walkable
	{
		get
		{
			return true;
		}
	}

	public MSRoadSpace(){}
}
