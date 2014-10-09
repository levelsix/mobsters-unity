using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MSPopupManager : MonoBehaviour {

	public static MSPopupManager instance;

	[SerializeField] MSGenericPopup genericPopupPrefab;

	[SerializeField] Transform genericParent;

	public TweenScale defaultScaleIn;
	public TweenAlpha defaultAlphaIn;
	public TweenScale defaultScaleOut;
	public TweenAlpha defaultAlphaOut;

	[System.Serializable]
	public class Popups
	{
		public MSGoonScreen goonScreen;
		public MSClanPopup clanPopup;
		public MSRaidScreen raidScreen;
		public MSRaidTeamPopup raidTeamPopup;
		public MSPopup loadingScreenBlocker;
		public MSHirePopup hirePopup;
		public MSChatPopup chatPopup;
		public MSQuestLog questPopup;
		public MSGenericPopup cityGeneric;
		public MSGenericPopup puzzleGeneric;
		public MSGoonInfoPopup goonInfoPopup;
		public MSProfilePopup profilePopup;
		public MSBuildingMenu buildingMenu;
	}

	public Popups popups;
	
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

	public int topLayer
	{
		get
		{
			return _currPops.Count;
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

	void Start()
	{
		MSPoolManager.instance.Warm(genericPopupPrefab, 2);
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
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !MSTutorialManager.instance.inTutorial)
		{
			CloseTopLayer();
		}
	}

	MSGenericPopup GrabGeneric()
	{
		MSGenericPopup pop = MSPoolManager.instance.Get<MSGenericPopup>(genericPopupPrefab, genericParent);
		pop.transform.localScale = Vector3.one;
		pop.transform.localPosition = Vector3.zero;
		pop.transform.localRotation = Quaternion.identity;
		return pop;
	}
	
	void InitPopup (MSGenericPopup pop)
	{
		OnPopup(pop.GetComponent<MSPopup>());
	}

	public void CreatePopup(string text)
	{
		MSGenericPopup popup = GrabGeneric();

		popup.Init(text);
		popup.gameObject.SetActive(false);

		InitPopup (popup);
	}

	public void CreatePopup(string title, string text)
	{
		MSGenericPopup popup = GrabGeneric();

		popup.Init(title, text);
		popup.gameObject.SetActive(false);

		InitPopup(popup);
	}
	
	public void CreatePopup(string text, string[] buttonLabels, string[] buttonSprites, Action[] buttonActions)
	{
		MSGenericPopup popup = GrabGeneric();

		popup.Init(text, buttonLabels, buttonSprites, buttonActions);
		popup.gameObject.SetActive(false);
		
		InitPopup (popup);
	}

	public void CreatePopup(string title, string text, string[] buttonLabels, string[] buttonSprites, Action[] buttonActions, string topColor = "green")
	{
		MSGenericPopup popup = GrabGeneric();

		popup.Init(title, text, buttonLabels, buttonSprites, buttonActions, topColor);
		popup.gameObject.SetActive(false);
		
		InitPopup (popup);
	}

	public void CreatePopup(string title, string text, string[] buttonLabels, string[] buttonSprites, WaitFunction[] waitFunctions, string topColor = "green")
	{
		MSGenericPopup popup = GrabGeneric();

		popup.Init(title, text, buttonLabels, buttonSprites, waitFunctions, topColor);
		popup.gameObject.SetActive(false);

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
		Debug.Log("Pop it, like a wheelie should");
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
			closing.Close(false);
			if (MSActionManager.Popup.OnPopupLayerClosed != null)
			{
				MSActionManager.Popup.OnPopupLayerClosed(_currPops.Count+1); //Needs to send int for layer that *was* closed
			}
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
			MSPopup closing = top;
			_currPops.RemoveAt(_currPops.Count-1);
			closing.Close(true);
		}
	}

	#region Default Tweens

	public float DefaultTweenIn(GameObject go)
	{
		TweenScale scale = TweenScale.Begin(go, defaultScaleIn.duration, defaultScaleIn.to);
		scale.from = defaultScaleIn.from;
		scale.animationCurve = defaultScaleIn.animationCurve;

		TweenAlpha alph = TweenAlpha.Begin(go, defaultAlphaIn.duration, defaultAlphaIn.to);
		alph.from = defaultAlphaIn.from;
		alph.animationCurve = defaultAlphaIn.animationCurve;

		return Mathf.Max(scale.duration, alph.duration);
	}

	public float DefaultTweenOut(GameObject go)
	{
		TweenScale scale = TweenScale.Begin(go, defaultScaleOut.duration, defaultScaleOut.to);
		scale.from = defaultScaleOut.from;
		scale.animationCurve = defaultScaleOut.animationCurve;
		
		TweenAlpha alph = TweenAlpha.Begin(go, defaultAlphaOut.duration, defaultAlphaOut.to);
		alph.from = defaultAlphaOut.from;
		alph.animationCurve = defaultAlphaOut.animationCurve;

		return Mathf.Max(scale.duration, alph.duration);
	}

	#endregion

}
