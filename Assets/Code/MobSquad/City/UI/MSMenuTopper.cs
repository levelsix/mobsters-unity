using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMenuTopper
/// </summary>
public class MSMenuTopper : MSPopup {

	public static MSMenuTopper instance;

	void Awake()
	{
		instance = this;
		gameObject.SetActive(false);
	}

}
