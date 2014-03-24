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
	Stack<GameObject> _currPops;

	/// <summary>
	/// Gets the popup that's one behind the current popup.
	/// Used by a Back Button to get the popup that it should slide to
	/// </summary>
	/// <value>The back pop.</value>
	public GameObject backPop
	{
		get
		{
			if (_currPops.Count > 1)
			{
				GameObject temp = _currPops.Pop();
				GameObject back = _currPops.Peek();
				_currPops.Push(temp);
				return back;
			}
			else
			{
				return null;
			}
		}
	}

	public GameObject top
	{
		get
		{
			return _currPops.Peek();
		}
	}
	
	/// <summary>
	/// Awake this instance.
	/// Set up the stack for popups
	/// </summary>
	void Awake()
	{
		_currPops = new Stack<GameObject>();
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
	
	void InitPopup (CBKGenericPopup pop, bool townMode)
	{
		Transform popT = pop.transform;

		if (townMode)
		{
			popT.parent = townPopupParent;
		}
		else
		{
			popT.parent = puzzlePopupParent;
		}
		popT.localScale = Vector3.one;
		popT.localPosition = Vector3.zero;
		
		OnPopup(pop.gameObject);
	}
	
	void CreatePopup(string text)
	{
		popup.Init(text);

		InitPopup (popup, MSSceneManager.instance.cityState);
	}
	
	void PopWithButtons(string text, string[] buttonLabels, Action[] buttonActions)
	{
		popup.Init(text, buttonLabels, buttonActions);
		
		InitPopup (popup, MSSceneManager.instance.cityState);
	}
	
	/// <summary>
	/// Raises the popup event.
	/// Adds a popup to the popup stack.
	/// </summary>
	/// <param name='popup'>
	/// Popup.
	/// </param>
	void OnPopup(GameObject popup)
	{
		_currPops.Push(popup);
		popup.SetActive(true);
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
		if (_currPops.Count > 0)
		{
			_currPops.Pop().SetActive(false);
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
