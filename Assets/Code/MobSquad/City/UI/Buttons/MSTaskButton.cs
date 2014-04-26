using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// CBK task button, the button on the right-side of the task bar.
/// When in BUILDING mode, this is the upgrade/finish upgrade button.
/// </summary>
public class MSTaskButton : MSTriggerPopupButton, MSPoolable {
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	MSTaskButton _prefab;
	
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as MSTaskButton;
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
	
	MSBuilding currBuilding;
	
	MSUnit currUnit;
	
	public enum Mode {SELL, UPGRADE, FINISH, ENGAGE, HEAL, ENHANCE, REMOVE};
	
	Mode currMode;
	
	System.Collections.Generic.Dictionary<Mode, string> modeTexts = new System.Collections.Generic.Dictionary<Mode, string>() {
		{Mode.SELL, "SELL"},
		{Mode.UPGRADE, "UPGRADE"},
		{Mode.FINISH, "FINISH"},
		{Mode.ENGAGE, "ENGAGE"},
		{Mode.HEAL, "HEAL"},
		{Mode.ENHANCE, "ENHANCE"},
		{Mode.REMOVE, "REMOVE"}
	};
	
	System.Collections.Generic.Dictionary<Mode, string> modeSprites = new System.Collections.Generic.Dictionary<Mode, string>() {
		{Mode.UPGRADE, "upgradeicon"},
		{Mode.SELL, "sellicon"},
		{Mode.FINISH, "diamond"},
		{Mode.ENGAGE, "moneystack"},
		{Mode.REMOVE, "sellicon"},
		{Mode.HEAL, "moneystack"}
	};
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
	}
	
	public MSPoolable Make (Vector3 origin)
	{
		MSTaskButton button = Instantiate(this, origin, Quaternion.identity) as MSTaskButton;
		button.prefab = this;
		return button;
	}
	
	public void Setup(Mode mode, MSBuilding building, GameObject popup = null)
	{
		currBuilding = building;
		currUnit = null;
		this.popup = popup;
		
		SetMode (mode);
	}
	
	public void Setup(Mode mode, MSUnit unit, GameObject popup = null)
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
		case Mode.REMOVE:
			ClickRemove();
			break;
		case Mode.HEAL:
			ClickHeal();
			break;
		default:
			break;
		}
	}

	void ClickHeal()
	{
		MSPopupManager.instance.popups.goonScreen.InitHeal();
		MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.GetComponent<MSPopup>());

		MSBuildingManager.instance.FullDeselect();
	}
	
	void ClickFinish()
	{
		if (currBuilding.obstacle == null)
		{
			currBuilding.upgrade.FinishWithPremium();
		}
		else
		{
			currBuilding.obstacle.FinishWithGems();
		}

		MSBuildingManager.instance.FullDeselect();
	}
	
	void ClickEngage()
	{
		if (currBuilding != null)
		{
			currBuilding.GetComponent<MSTaskable>().EngageTask();
		}
		else
		{
			currUnit.GetComponent<MSTaskable>().EngageTask();
		}

		MSBuildingManager.instance.FullDeselect();
	}
	
	void ClickUpgrade()
	{
		base.OnClick();
		popup.GetComponent<MSBuildingUpgradePopup>().Init(currBuilding);
	}
	
	public void Pool ()
	{
		MSPoolManager.instance.Pool(this);
	}

	void ClickRemove()
	{
		currBuilding.GetComponent<MSObstacle>().StartRemove();
		MSBuildingManager.instance.FullDeselect();
	}
}
