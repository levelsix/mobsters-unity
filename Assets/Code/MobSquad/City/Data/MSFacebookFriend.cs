using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSFacebookFriend
/// </summary>
public class MSFacebookFriend {

	string name;

	string id;

	string pictureURL;

	bool installed = false;

	public MSFacebookFriend(string name, string id, string pictureURL, bool installed)
	{
		this.name = name;
		this.id = id;
		this.pictureURL = pictureURL;
		this.installed = installed;
	}

	public override string ToString ()
	{
		return name + " " + id + " " + installed;
	}

}
