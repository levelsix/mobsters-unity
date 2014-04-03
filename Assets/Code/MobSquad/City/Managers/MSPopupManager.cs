using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MSPopupManager : MonoBehaviour {

	public static MSPopupManager instance;

	[System.Serializable]
	public class Popups
	{
		public CBKGoonScreen goonScreen;
		public CBKClanPopup clanPopup;
		public MSRaidScreen raidScreen;
		public MSRaidTeamPopup raidTeamPopup;
		public GameObject loadingScreenBlocker;
	}

	public Popups popups;

	/// <summary>
	/// The popup
	/// </summary>
	[SerializeField]
	CBKGenericPopup popup;
	
	[SerializeField]
	Transform townPopupParent;

	[SerializeField]
	Transform puzzlePopupParent;
	
	/// <summary>
	/// The stack of current popup menus.
	/// </summary>
	List<MSPopup> _currPops = new List<MSPopup>();

	/// <summary>
	/// Gets the popup that's one behind the current popup.
	/// Used by a Back Button to get the popup that it should slide to
	/// </summary>
	/// <value>The back pop.</value>
	public MSPopup backPop
	{
		get
		{
			if (_currPops.Count > 1)
			{
				return _currPops[_currPops.Count-2];
			}
			else
			{
				return null;
			}
		}
	}

	public MSPopup top
	{
		get
		{
			if (_currPops.Count == 0)
			{
				return null;
			}
			return _currPops[_currPops.Count-1];
		}
	}
	
	/// <summary>
	/// Awake this instance.
	/// Set up the stack for popups
	/// </summary>
	void Awake()
	{
		instance = this;
	}
	
	/// <summary>
	/// Raises the enable event.
	/// Assigns delegates.
	/// </summary>
	void OnEnable()
	{
		MSActionManager.Popup.OnPopup += OnPopup;
		MSActionManager.Popup.CloseAllPopups += CloseAllPopups;
		MSActionManager.Popup.ClosePopupLayer += ClosePopupLayer;
		MSActionManager.Popup.CloseTopPopupLayer += CloseTopLayer;
		MSActionManager.Popup.CreatePopup += CreatePopup;
		MSActionManager.Popup.CreateButtonPopup += PopWithButtons;
	}
	
	/// <summary>
	/// Raises the disable event.
	/// Deassigns deletages.
	/// </summary>
	void OnDisable()
	{
		MSActionManager.Popup.OnPopup -= OnPopup;
		MSActionManager.Popup.CloseAllPopups -= CloseAllPopups;
		MSActionManager.Popup.ClosePopupLayer -= ClosePopupLayer;
		MSActionManager.Popup.CloseTopPopupLayer -= CloseTopLayer;
		MSActionManager.Popup.CreatePopup -= CreatePopup;
		MSActionManager.Popup.CreateButtonPopup -= PopWithButtons;
	}
	
	void InitPopup (CBKGenericPopup pop)
	{
		Transform popT = pop.transform;

		if (MSSceneManager.instance.cityState)
		{
			popT.parent = townPopupParent;
		}
		else
		{
			popT.parent = puzzlePopupParent;
		}
		popT.localScale = Vector3.one;
		popT.localPosition = Vector3.zero;
		
		OnPopup(pop.GetComponent<MSPopup>());
	}
	
	void CreatePopup(string text)
	{
		popup.Init(text);

		InitPopup (popup);
	}
	
	void PopWithButtons(string text, string[] buttonLabels, Action[] buttonActions)
	{
		popup.Init(text, buttonLabels, buttonActions);
		
		InitPopup (popup);
	}
	
	/// <summary>
	/// Raises the popup event.
	/// Adds a popup to the popup stack.
	/// </summary>
	/// <param name='popup'>
	/// Popup.
	/// </param>
	void OnPopup(MSPopup popup)
	{
		if (popup is MSMenuPopup && top is MSMenuPopup)
		{
			(top as MSMenuPopup).SlideOut();
		}
		_currPops.Add(popup);
		popup.Popup();
	}
	
	/// <summary>
	/// Closes all popups.
	/// </summary>
	void CloseAllPopups()
	{
		ClosePopupLayer(0);
	}
	
	void CloseTopLayer()
	{
		if (_currPops.Count > 1)
		{
			if (top is MSMenuPopup && backPop is MSMenuPopup)
			{
				(backPop as MSMenuPopup).SlideBackIn();
			}
		}
		if (_currPops.Count > 0)
		{
			MSPopup closing = top;
			_currPops.RemoveAt(_currPops.Count-1);
			closing.Close();
		}
	}
	
	/// <summary>
	/// Closes the popup layer and all layers above it, but not below it
	/// </summary>
	/// <param name='stackLayer'>
	/// Stack layer.
	/// </param>
	void ClosePopupLayer(int stackLayer)
	{
		while(_currPops.Count > stackLayer)
		{
			CloseTopLayer();
		}
	}
	
}
