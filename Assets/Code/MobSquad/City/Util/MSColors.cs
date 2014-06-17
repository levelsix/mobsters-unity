using UnityEngine;
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

	public static readonly Color activeTabTextColor = new Color(0, .5f, .9f);
	public static readonly Color inactiveTabTextColor = new Color(0, .41f, .61f);

	public static readonly Dictionary<Quality, Color> qualityColors = new Dictionary<Quality, Color>()
	{
		{Quality.COMMON, new Color(.607f, .592f, .407f)},
		{Quality.RARE, new Color(.043f, .584f, .929f)},
		{Quality.ULTRA, new Color(1, .752f, .004f)},
		{Quality.EPIC, new Color(.352f, .008f, 1)},
		{Quality.LEGENDARY, new Color(.725f, .082f, .091f)},
		{Quality.NO_QUALITY, Color.black},
		{Quality.EVO, Color.gray}
	};
	
	public static readonly Dictionary<Element, Color> elementColors = new Dictionary<Element, Color>()
	{
		{Element.DARK, Color.magenta},
		{Element.EARTH, Color.green},
		{Element.FIRE, Color.red},
		{Element.LIGHT, Color.yellow},
		{Element.WATER, Color.blue},
		{Element.ROCK, Color.grey},
		{Element.NO_ELEMENT, Color.black}
	};
}
