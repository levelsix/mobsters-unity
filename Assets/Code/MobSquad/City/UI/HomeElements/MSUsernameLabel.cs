using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSUsernameLabel
/// </summary>
public class MSUsernameLabel : MonoBehaviour {

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
	}

	void OnStartup(StartupResponseProto response)
	{
		GetComponent<UILabel>().text = response.sender.name;
	}
}
