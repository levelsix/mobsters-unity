using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// CBK task button, the button on the right-side of the task bar.
/// When in BUILDING mode, this is the upgrade/finish upgrade button.
/// </summary>
public class CBKTaskButton : CBKTriggerPopupButton {
	
	[HideInInspector]
	public GameObject gameObj;
	
	[SerializeField]
	UILabel text;
	
	[SerializeField]
	UISprite icon;
	
	CBKBuilding currBuilding;
	
	void Awake()
	{
		gameObj = gameObject;
	}
	
	public void SetupBuilding(CBKBuilding building)
	{
		currBuilding = building;
		if (building.userStructProto.isComplete)
		{
			text.text = "UPGRADE";
		}
		else
		{
			text.text = "FINISH";
		}
	}
	
	public override void OnClick ()
	{
		base.OnClick ();
		popup.GetComponent<CBKBuildingUpgradePopup>().Init(currBuilding);
	}
}
