﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSColors
/// </summary>
public static class MSColors {

	public static readonly Color cashTextColor = new Color(.294f, .627f, 0);

	public static readonly Color oilTextColor = new Color(.901f, .667f, .105f);

	public static readonly Color gemTextColor = new Color(.537f, 0, .949f);

	public static readonly Dictionary<Quality, Color> qualityColors = new Dictionary<Quality, Color>()
	{
		{Quality.COMMON, new Color(.607f, .592f, .407f)},
		{Quality.RARE, new Color(.043f, .584f, .929f)},
		{Quality.ULTRA, new Color(1, .752f, .004f)},
		{Quality.EPIC, new Color(.352f, .008f, 1)},
		{Quality.LEGENDARY, new Color(.725f, .082f, .091f)}
	};
}
