using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRequestBadge
/// </summary>
[RequireComponent (typeof (MSBadge))]
public class MSRequestBadge : MonoBehaviour {

	MSBadge badge;

	void Awake()
	{
		badge = GetComponent<MSBadge>();
	}

	void OnEnable()
	{
		badge.notifications = 0;
		MSActionManager.UI.OnRequestsAcceptOrReject += OnRequestsChange;
	}

	void OnDisable()
	{
		MSActionManager.UI.OnRequestsAcceptOrReject -= OnRequestsChange;
	}

	void OnRequestsChange()
	{
		badge.notifications = MSRequestManager.instance.invitesForMe.Count;
	}
}
