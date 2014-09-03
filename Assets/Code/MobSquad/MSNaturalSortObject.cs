using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MSNaturalSortObject : IComparer<UnityEngine.Object> {
	//Grabbed from:
	//http://stackoverflow.com/questions/8568696/icomparer-for-natural-sorting
	public int Compare( UnityEngine.Object oX, UnityEngine.Object oY){

		Sprite sX = oX as Sprite;
		Sprite sY = oY as Sprite;

		string x = sX.name;
		string y = sY.name;

		Regex regex = new Regex(@"(?<NumPart>\d+)(?<StrPart>\D*)",RegexOptions.Compiled);
		var mx = regex.Match(x);
		var my = regex.Match(y);
		var ret = int.Parse(mx.Groups["NumPart"].Value).CompareTo(int.Parse(my.Groups["NumPart"].Value));
		if(ret != 0) return ret;
		return mx.Groups["StrPart"].Value.CompareTo(my.Groups["StrPart"].Value);
	}
}