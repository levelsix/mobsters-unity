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
	UISprite bg;

	MSBuilding currBuilding;
	
	MSUnit currUnit;

	UIButton button;

	public enum Mode {SELL, UPGRADE, FINISH, ENGAGE, HEAL, ENHANCE, REMOVE, EVOLVE, HIRE, MINIJOB};
	
	Mode currMode;
	
	System.Collections.Generic.Dictionary<Mode, string> modeTexts = new System.Collections.Generic.Dictionary<Mode, string>() {
		{Mode.SELL, "SELL"},
		{Mode.UPGRADE, "UPGRADE"},
		{Mode.FINISH, "FINISH"},
		{Mode.ENGAGE, "(B) ENTER"},
		{Mode.HEAL, "HEAL"},
		{Mode.ENHANCE, "ENHANCE"},
		{Mode.REMOVE, "REMOVE"},
		{Mode.EVOLVE, "EVOLVE"},
		{Mode.HIRE, "HIRE"},
		{Mode.MINIJOB, "MINI JOBS"}
	};

	System.Collections.Generic.Dictionary<Mode, string> modeButtonSprites = new System.Collections.Generic.Dictionary<Mode, string>() {
		{Mode.UPGRADE, "greenmenuoption"},
		{Mode.FINISH, "purplemenuoption"},
		{Mode.ENGAGE, "orangemenuoption"},
		{Mode.REMOVE, "greenmenuoption"},
		{Mode.HEAL, "orangemenuoption"},
		{Mode.EVOLVE, "orangemenuoption"},
		{Mode.HIRE, "orangemenuoption"},
		{Mode.MINIJOB, "orangemenuoption"}
	};

	System.Collections.Generic.Dictionary<string, Color> buttonTextColors = new Dictionary<string, Color>(){
		{"greenmenuoption", new Color(.353f, .491f, .027f)},
		{"orangemenuoption", new Color(.639f, .353f, 0)},
		{"yellowmenuoption", new Color(.776f, .533f, 0)},
		{"purplemenuoption", Color.white}
	};
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		button = GetComponent<UIButton>();
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

		bg.spriteName = modeButtonSprites[mode];
		text.text = modeTexts[mode];
		text.effectColor = Color.white;
		switch(mode)
		{
		case Mode.UPGRADE:
			if (currBuilding.combinedProto.structInfo.buildResourceType == com.lvl6.proto.ResourceType.OIL)
			{
				bg.spriteName = "yellowmenuoption";
				text.text = "Upgrade\n(o) " + currBuilding.combinedProto.successor.structInfo.buildCost;
			}
			else
			{
				text.text = "Upgrade\n$" + currBuilding.combinedProto.successor.structInfo.buildCost;
			}
			break;
		case Mode.FINISH:
			if (currBuilding.obstacle != null)
			{
				text.text = "Finish\n(G) " + currBuilding.obstacle.gemsToFinish;
			}
			else
			{
				text.text = "Finish\n(G) " + currBuilding.upgrade.gemsToFinish;
			}
			text.effectColor = Color.black;
			break;
		case Mode.REMOVE:
			if (currBuilding.obstacle.obstacle.removalCostType == com.lvl6.proto.ResourceType.OIL)
			{
				bg.spriteName = "yellowmenuoption";
				text.text = "Remove\n(o) " + currBuilding.obstacle.obstacle.cost;
			}
			else
			{
				text.text = "Remove\n$" + currBuilding.obstacle.obstacle.cost;
			}
			break;
		default:
			break;
		}
		text.color = buttonTextColors[bg.spriteName];
		button.normalSprite = bg.spriteName;
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
		case Mode.HIRE:
			ClickHire();
			break;
		case Mode.MINIJOB:
			ClickMiniJob();
			break;
		default:
			break;
		}
	}

	void ClickHeal()
	{
		MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.GetComponent<MSPopup>());
		MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);

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

	void ClickHire()
	{
		base.OnClick();
		popup.GetComponent<MSHirePopup>().Init(currBuilding);
	}

	void ClickMiniJob()
	{
		base.OnClick();
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
