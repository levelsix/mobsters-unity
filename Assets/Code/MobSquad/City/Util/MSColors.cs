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

	public static readonly Color blackAndWhiteTint = new Color(0.222f, 0.707f, 0.071f);

	public static readonly Dictionary<Quality, Color> qualityColors = new Dictionary<Quality, Color>()
	{
//		{Quality.COMMON, new Color(.607f, .592f, .407f)}, old tan colored common
		{Quality.COMMON, new Color(.412f, .412f, .412f)},
		{Quality.RARE, Color.blue},
		{Quality.SUPER, new Color(.043f, .584f, .929f)},
		{Quality.ULTRA, new Color(1, .752f, .004f)},
		{Quality.EPIC, new Color(.352f, .008f, 1)},
		{Quality.LEGENDARY, new Color(.725f, .082f, .091f)},
		{Quality.NO_QUALITY, Color.black},
		{Quality.EVO, Color.gray}
	};
	
	public static readonly Dictionary<Element, Color> elementColors = new Dictionary<Element, Color>()
	{
		{Element.DARK, new Color(0.647f, 0.247f, 0.921f)},//magenta
		{Element.EARTH, new Color(0.25f, 0.713f, 0f)},//green
		{Element.FIRE, new Color(0.976f, 0.207f, 0.019f)},//red
		{Element.LIGHT, new Color(1f, 0.819f, 0.322f)},//yellow
		{Element.WATER, new Color(0.172f, 0.702f, 0.85f)},//blue
		{Element.ROCK, new Color(0.537f, 0.612f, 0.655f)},//grey
		{Element.NO_ELEMENT, Color.black}
	};

	public static string colorHexString(Color color)
	{
		string red = ((int)(color.r * 255)).ToString("X2");
		string green = ((int)(color.g * 255)).ToString("X2");
		string blue = ((int)(color.b * 255)).ToString("X2");
		return red + green + blue;
	}

	public static string nguiColorHexString(Color color)
	{
		return "[" + colorHexString(color) + "]";
	}
}
