using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// ResourceCard
/// Used for the Funds menu, for players to click on and buy currency
/// </summary>
public class MSResourceCard : MonoBehaviour {

	[SerializeField]
	UISprite icon;

	[SerializeField]
	ResourceType resourceToFill;

	[SerializeField]
	bool fill;

	[SerializeField]
	float percent;

	[SerializeField]
	UIButton button;

	[SerializeField]
	UILabel costLabel;

	[SerializeField]
	UILabel amountLabel;

	[SerializeField]
	GameObject front;
	
	[SerializeField]
	GameObject back;

	int cost;

	int amount;

	bool on{
		set{
			if(value){
				state = State.ACTIVE;
			}else{
				state = State.INACTIVE;
			}
		}

		get{
			return state == State.ACTIVE;
		}
	}
	
	public enum State{
		ACTIVE,
		INACTIVE
	}
	
	State _state;
	
	public State state{
		get{
			return _state;
		}
		set{
			_state = value;
			
			if(value == State.ACTIVE){
//				Debug.Log("On" + percent + " : " + amountLabel.text);
				button.pressed = button.disabledColor;
				button.normalSprite = FRONT_IMAGE;

				icon.color = Color.white;
				
				front.SetActive(true);
				back.SetActive(false);
			}else{
//				Debug.Log("Off" + percent + " : " + amountLabel.text + transform.parent.gameObject.name.ToString());
				button.pressed = button.disabledColor;
				button.normalSprite = BACK_IMAGE;
				button.SetState(UIButtonColor.State.Normal, true);
				if(button.normalSprite != BACK_IMAGE){
					StartCoroutine(Wat());
				}

				icon.color = Color.black;

				front.SetActive(false);
				back.SetActive(true);
			}
		}
	}

	static int oilMaxLevel = -1;
	static int cashMaxLevel = -1;

	static string oilSprite;
	static string cashSprite;

	const string FRONT_IMAGE = "menusquareactive";
	const string BACK_IMAGE = "menusquareinactive";

	const int PREFIX_LENGTH = 6; // "1Funds".Length

	IEnumerator Wat(){
		while(button.normalSprite != BACK_IMAGE){
			button.normalSprite = BACK_IMAGE;
			Debug.Log(button.normalSprite + " = " + BACK_IMAGE);
			yield return null;
		}
	}

	void OnEnable()
	{
		if(oilMaxLevel == -1){
			Debug.Log("Checking for max level storage");
			List<ResourceStorageProto> allStorages = MSBuildingManager.instance.GetAllStorages();
			foreach(ResourceStorageProto storage in allStorages){
				if(storage.resourceType == ResourceType.OIL && storage.structInfo.level > oilMaxLevel){
					oilMaxLevel = storage.structInfo.level;
					oilSprite = storage.structInfo.imgName;
					oilSprite = oilSprite.Substring(0, oilSprite.Length - ".png".Length); //remove .png
				}
				else if (storage.structInfo.level > cashMaxLevel)
				{
					cashMaxLevel = storage.structInfo.level;
					cashSprite = storage.structInfo.imgName;
					cashSprite = cashSprite.Substring(0, cashSprite.Length - ".png".Length); //remove .png
				}
			}
		}

		Init ();
		MSActionManager.UI.OnChangeResource[(int)resourceToFill-1] += Init;
	}

	void OnDisable()
	{
		oilMaxLevel = -1;
		cashMaxLevel = -1;
		MSActionManager.UI.OnChangeResource[(int)resourceToFill-1] -= Init;
	}

	void Init(int currAmount)
	{
		Init();
	}

	void Init ()
	{
		if (fill) 
		{
			amount = Mathf.Max(0, MSResourceManager.maxes [(int)resourceToFill - 1] - MSResourceManager.resources [(int)resourceToFill - 1]);
			on = amount > 0;
		}
		else 
		{
			amount = Mathf.CeilToInt (MSResourceManager.maxes [(int)resourceToFill - 1] * percent);
			on = MSResourceManager.resources [(int)resourceToFill - 1] + amount < MSResourceManager.maxes [(int)resourceToFill - 1];
		}
		cost = Mathf.CeilToInt (amount * MSWhiteboard.constants.gemsPerResource);
		costLabel.text = "(G)" + cost;
		amountLabel.text = ((resourceToFill == ResourceType.CASH) ? "$" : "(O)") + amount;

		switch(resourceToFill){
		case ResourceType.CASH:
			icon.spriteName = icon.spriteName.Substring(0, PREFIX_LENGTH) + cashSprite;
			break;
		case ResourceType.OIL:
			icon.spriteName = icon.spriteName.Substring(0, PREFIX_LENGTH) + oilSprite;
			break;
		default:
			break;
		}
	}

	void OnClick()
	{
		if (on)
		{
			if (MSResourceManager.resources[(int)ResourceType.GEMS-1] >= cost)
			{
				MSResourceManager.instance.SpendGemsForOtherResource(resourceToFill, amount);
			}
			else
			{
				MSActionManager.Popup.DisplayError("Not enough Gems");
			}
		}
		else
		{
			MSActionManager.Popup.DisplayError("Storage is too full");
		}
	}
}
