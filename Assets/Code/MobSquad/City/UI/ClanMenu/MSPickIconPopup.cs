using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSPickIconPopup : MonoBehaviour {

	[SerializeField]
	MSPickIconEntry badgeIcon;

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	Transform selectionBg;

	List<MSPickIconEntry> badges = new List<MSPickIconEntry>();

	void Awake()
	{
		Init ();
	}

	void Init()
	{
		for (int i = 0; i < 18; i++) 
		{
			AddClanIcon(i+1);
		}
		grid.Reposition();
	}

	void AddClanIcon(int iconId)
	{
		MSPickIconEntry entry = MSPoolManager.instance.Get<MSPickIconEntry>(badgeIcon, grid.transform);
		entry.Init (iconId, selectionBg);
		entry.transform.localScale = Vector3.one;
		badges.Add(entry);
	}
}
