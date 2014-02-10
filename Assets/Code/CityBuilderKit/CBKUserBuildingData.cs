﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKBuildingData
/// </summary>
public class CBKUserBuildingData 
{

	public CBKCombinedBuildingProto combinedProto;
	
	public FullUserStructureProto userStructProto;

	public bool underConstruction {
		get
		{
			return !userStructProto.isComplete;
		}
		set
		{
			userStructProto.isComplete = !value;
		}
	}

}
