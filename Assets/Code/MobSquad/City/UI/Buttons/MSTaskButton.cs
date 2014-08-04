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
	UILabel topLabel;

	[SerializeField]
	UILabel middleLabel;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UISprite bg;

	TweenPosition tweenPos;

	TweenAlpha tweenAlpha;

	MSBuilding currBuilding;
	
	MSUnit currUnit;

	UIButton button;

	public enum Mode {SELL_MOBSTERS, UPGRADE, FINISH, HEAL, ENHANCE, REMOVE_OBSTACLE, EVOLVE, HIRE, MINIJOB, TEAM, FIX};
	
	public Mode currMode;

	public bool tweeningOut
	{
		get
		{
			return tweenPos.tweenFactor > 0 || tweenAlpha.tweenFactor > 0;
		}
	}
	
	static readonly Dictionary<Mode, string> modeTexts = new Dictionary<Mode, string>() 
	{
		{Mode.SELL_MOBSTERS, "Sell Mobsters"},
		{Mode.UPGRADE, "Upgrade"},
		{Mode.FINISH, "Finish Now"},
		{Mode.HEAL, "Heal Mobsters"},
		{Mode.ENHANCE, "Enhance"},
		{Mode.REMOVE_OBSTACLE, "Remove"},
		{Mode.EVOLVE, "Evolve"},
		{Mode.HIRE, "Bonus Slots"},
		{Mode.MINIJOB, "MiniJobs"},
		{Mode.TEAM, "Manage Team"},
		{Mode.FIX, "Fix"}
	};

	static readonly Dictionary<Mode, string> modeIcons = new Dictionary<Mode, string>()
	{
		{Mode.SELL_MOBSTERS, "buildingsell"},
		{Mode.UPGRADE, "buildingupgrade"},
		{Mode.FINISH, ""},
		{Mode.HEAL, "buildingheal"},
		{Mode.ENHANCE, "buildingenhance"},
		{Mode.REMOVE_OBSTACLE, "buildingremove"},
		{Mode.EVOLVE, "buildingevolve"},
		{Mode.HIRE, "buildingbonusslots"},
		{Mode.MINIJOB, "buildingminijobs"},
		{Mode.TEAM, "buildingmanage"},
		{Mode.FIX, "buildingfix"}
	};

	Dictionary<string, Color> buttonTextColors = new Dictionary<string, Color>(){
		{NORMAL_SPRITE, new Color(.639f, .353f, 0)},
		{FINISH_SPRITE, Color.white}
	};

	const string NORMAL_SPRITE = "buildingoptionbutton";
	const string FINISH_SPRITE = "buildingfinishnow";


	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		button = GetComponent<UIButton>();
		tweenPos = GetComponent<TweenPosition>();
		tweenAlpha = GetComponent<TweenAlpha>();
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

		bg.spriteName = mode == Mode.FINISH ? FINISH_SPRITE : NORMAL_SPRITE;

		bottomLabel.text = modeTexts[mode];
		bottomLabel.effectColor = Color.white;
		icon.spriteName = modeIcons[mode];
		icon.MakePixelPerfect();
		icon.alpha = 1;
		middleLabel.text = " ";
		topLabel.text = " ";

		switch(mode)
		{
		case Mode.FIX:
			if (currBuilding.combinedProto.structInfo.buildResourceType == com.lvl6.proto.ResourceType.OIL)
			{
				topLabel.text = "(o) " + currBuilding.combinedProto.successor.structInfo.buildCost;
				topLabel.color = MSColors.oilTextColor;
			}
			else
			{
				topLabel.text = "(c) " + currBuilding.combinedProto.successor.structInfo.buildCost;
				topLabel.color = MSColors.cashTextColor;
			}
			break;
		case Mode.UPGRADE:
			if (currBuilding.combinedProto.structInfo.buildResourceType == com.lvl6.proto.ResourceType.OIL)
			{
				topLabel.text = "(o) " + currBuilding.combinedProto.successor.structInfo.buildCost;
				topLabel.color = MSColors.oilTextColor;
			}
			else
			{
				topLabel.text = "(c) " + currBuilding.combinedProto.successor.structInfo.buildCost;
				topLabel.color = MSColors.cashTextColor;
			}
			break;
		case Mode.FINISH:
			if (currBuilding.obstacle != null)
			{
				middleLabel.text = "(g) " + currBuilding.obstacle.gemsToFinish;
			}
			else
			{
				middleLabel.text = "(g) " + currBuilding.upgrade.gemsToFinish;
			}
			icon.alpha = 0;
			bottomLabel.effectColor = new Color(3/255f, 3/255f, 3/255f);
			break;
		case Mode.REMOVE_OBSTACLE:
			if (currBuilding.obstacle.obstacle.removalCostType == com.lvl6.proto.ResourceType.OIL)
			{
				topLabel.text = "(o) " + currBuilding.obstacle.obstacle.cost;
				topLabel.color = MSColors.oilTextColor;
			}
			else
			{
				topLabel.text = "(c) " + currBuilding.obstacle.obstacle.cost;
				topLabel.color = MSColors.cashTextColor;
			}
			break;
		default:
			break;
		}
		bottomLabel.color = buttonTextColors[bg.spriteName];
		button.normalSprite = bg.spriteName;
		button.pressedSprite = bg.spriteName + "pressed";
	}
	
	public override void OnClick ()
	{
		switch(currMode)
		{
		case Mode.FINISH:
			ClickFinish();
			break;
		case Mode.FIX:
		case Mode.UPGRADE:
			ClickUpgrade();
			break;
		case Mode.REMOVE_OBSTACLE:
			ClickRemove();
			break;
		case Mode.HEAL:
			OpenFunctionalMenu(GoonScreenMode.HEAL);
			break;
		case Mode.TEAM:
			OpenFunctionalMenu(GoonScreenMode.TEAM);
			break;
		case Mode.SELL_MOBSTERS:
			OpenFunctionalMenu(GoonScreenMode.SELL);
			break;
		case Mode.ENHANCE:
			OpenFunctionalMenu(GoonScreenMode.PICK_ENHANCE);
			break;
		case Mode.EVOLVE:
			OpenFunctionalMenu(GoonScreenMode.PICK_EVOLVE);
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

	void OpenFunctionalMenu(GoonScreenMode mode)
	{
		MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.GetComponent<MSPopup>());
		MSPopupManager.instance.popups.goonScreen.Init(mode);
		MSBuildingManager.instance.FullDeselect();
	}
	
	void ClickFinish()
	{
		MSBuildingManager.instance.FullDeselect();
		if (currBuilding.obstacle == null)
		{
			currBuilding.upgrade.FinishWithPremium();
		}
		else
		{
			currBuilding.obstacle.FinishWithGems();
			currBuilding.Select();
		}
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
		StartCoroutine(Exit());
	}

	void ClickRemove()
	{
		currBuilding.GetComponent<MSObstacle>().StartRemove();
		MSBuildingManager.instance.FullDeselect();
	}

	public void Enter(int delays)
	{
		tweenPos.delay = delays * .1f;
		tweenAlpha.delay = delays * .1f;

		tweenPos.to = trans.localPosition;
		tweenPos.from = trans.localPosition + new Vector3(0, -50, 0);

		tweenPos.Sample(0, true);
		tweenPos.PlayForward();
		tweenAlpha.Sample(0, true);
		tweenAlpha.PlayForward();
	}

	IEnumerator Exit()
	{
		tweenPos.PlayReverse();
		tweenAlpha.PlayReverse();
		while (tweeningOut)
		{
			yield return null;
		}
		MSPoolManager.instance.Pool(this);
		yield return null;
	}
}
