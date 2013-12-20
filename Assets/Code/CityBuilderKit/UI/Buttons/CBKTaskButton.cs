using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// CBK task button, the button on the right-side of the task bar.
/// When in BUILDING mode, this is the upgrade/finish upgrade button.
/// </summary>
public class CBKTaskButton : CBKTriggerPopupButton, CBKPoolable {
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	CBKTaskButton _prefab;
	
	public CBKPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKTaskButton;
		}
	}
	
	[HideInInspector]
	public Transform trans;
	
	public Transform transf {
		get {
			return trans;
		}
	}
	
	[HideInInspector]
	public GameObject gameObj;
	
	[SerializeField]
	UILabel text;
	
	[SerializeField]
	UISprite icon;
	
	CBKBuilding currBuilding;
	
	CBKUnit currUnit;
	
	public enum Mode {SELL, UPGRADE, FINISH, ENGAGE, HEAL, ENHANCE};
	
	Mode currMode;
	
	System.Collections.Generic.Dictionary<Mode, string> modeTexts = new System.Collections.Generic.Dictionary<Mode, string>() {
		{Mode.SELL, "SELL"},
		{Mode.UPGRADE, "UPGRADE"},
		{Mode.FINISH, "FINISH"},
		{Mode.ENGAGE, "ENGAGE"},
		{Mode.HEAL, "HEAL"},
		{Mode.ENHANCE, "ENHANCE"}
	};
	
	System.Collections.Generic.Dictionary<Mode, string> modeSprites = new System.Collections.Generic.Dictionary<Mode, string>() {
		{Mode.UPGRADE, "upgradeicon"},
		{Mode.SELL, "sellicon"},
		{Mode.FINISH, "diamond"},
		{Mode.ENGAGE, "moneystack"}
	};
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
	}
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKTaskButton button = Instantiate(this, origin, Quaternion.identity) as CBKTaskButton;
		button.prefab = this;
		return button;
	}
	
	public void Setup(Mode mode, CBKBuilding building, GameObject popup = null)
	{
		currBuilding = building;
		currUnit = null;
		this.popup = popup;
		
		SetMode (mode);
	}
	
	public void Setup(Mode mode, CBKUnit unit, GameObject popup = null)
	{
		currUnit = unit;
		currBuilding = null;
		this.popup = popup;
		
		SetMode(mode);
	}

	void SetMode (Mode mode)
	{
		currMode = mode; 
		
		text.text = modeTexts[currMode];

		icon.spriteName = modeSprites[currMode];
		//UISpriteData data = icon.GetAtlasSprite();
		//icon.height = data.height;
		//icon.width = data.width;
	}
	
	public override void OnClick ()
	{
		switch(currMode)
		{
		case Mode.FINISH:
			ClickFinish();
			break;
		case Mode.ENGAGE:
			ClickEngage();
			break;
		case Mode.UPGRADE:
			ClickUpgrade();
			break;
		case Mode.SELL:
			ClickSell();
			break;
		default:
			break;
		}
	}
	
	void ClickFinish()
	{
		
		base.OnClick();
		popup.GetComponent<CBKBuildingUpgradePopup>().Init(currBuilding);

		//currBuilding.upgrade.FinishWithPremium();
	}
	
	void ClickEngage()
	{
		if (currBuilding != null)
		{
			currBuilding.GetComponent<CBKTaskable>().EngageTask();
		}
		else
		{
			currUnit.GetComponent<CBKTaskable>().EngageTask();
		}
	}
	
	void ClickUpgrade()
	{
		base.OnClick();
		popup.GetComponent<CBKBuildingUpgradePopup>().Init(currBuilding);
	}
	
	void ClickSell()
	{
		currBuilding.Sell();
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
}
