using UnityEngine;
using System.Collections;


public class SoomlaNullable
{
	//Extend this class if you want to use the syntax
	//	if(myObject)
	//to check if it is not null
	public static implicit operator bool(SoomlaNullable o) {
		return (object)o != null;
	}
}


