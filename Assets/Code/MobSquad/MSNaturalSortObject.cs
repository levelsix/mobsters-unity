using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MSNaturalSortObject : IComparer<UnityEngine.Object> {
	//Grabbed from:
	//http://stackoverflow.com/questions/8568696/icomparer-for-natural-sorting
	public int Compare( UnityEngine.Object oX, UnityEngine.Object oY){
		return Compare(oX.name, oY.name);
	}

	public static int Compare( string x, string y){
		Regex regex = new Regex(@"(?<NumPart>\d+)(?<StrPart>\D*)",RegexOptions.IgnoreCase);
		var mx = regex.Match(x);
		var my = regex.Match(y);
		var ret = int.Parse(mx.Groups["NumPart"].Value).CompareTo(int.Parse(my.Groups["NumPart"].Value));
		if(ret != 0) return ret;
		return mx.Groups["StrPart"].Value.CompareTo(my.Groups["StrPart"].Value);
	}

	/// <summary>
	/// Sort by name of the object
	/// </summary>
	public static int CompareTrans(Transform x, Transform y)
	{
		return Compare(x.name, y.name);
	}


}