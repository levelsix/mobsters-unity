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

	string _name;

	public string name
	{
		get
		{
			return _name;
		}
	}

	string _id;

	public string id
	{
		get
		{
			return _id;
		}
	}

	string pictureURL;

	bool _installed = false;

	public bool installed
	{
		get
		{
			return _installed;
		}
	}

	public MSFacebookFriend(string name, string id, string pictureURL, bool installed)
	{
		this._name = name;
		this._id = id;
		this.pictureURL = pictureURL;
		this._installed = installed;
	}

	public override string ToString ()
	{
		return _name + " " + _id + " " + _installed;
	}

}
