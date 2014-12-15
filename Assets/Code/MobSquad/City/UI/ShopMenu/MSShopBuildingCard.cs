using UnityEngine;
using System.Collections;

public class MSShopBuildingCard : MonoBehaviour {

	[SerializeField]
	UISprite buildingIcon;

	[SerializeField]
	UILabel title;
	
	[SerializeField]
	UILabel description;

	[SerializeField]
	UISprite currency;
	
	[SerializeField]
	UILabel cost;
	
	[SerializeField]
	UILabel quantity;
	
	[SerializeField]
	UILabel time;

	[SerializeField]
	UILabel requirement;

	[SerializeField]
	UISprite divider;

	[SerializeField]
	UISprite clock;

	[SerializeField]
	UILabel builtLabel;

	UIButton button;

	const string ACTIVE_BACKGROUND = "menusqaureactive";

	const string INACTIVE_BACKGROUND = "menusqareinactive";

	const string ACTIVE_DIVIDER = "activedividerline";

	const string INACTIVE_DIVIDER = "inactivedividerline";

	public enum State{
		ACTIVE,
		INACTIVE
	}
	
	public State _state;
	
	public State state{
		get{
			return _state;
		}
		set{
			_state = value;

			if(value == State.ACTIVE){
//				Debug.Log("Active");
				//button.normalSprite = ACTIVE_BACKGROUND;
				divider.spriteName = ACTIVE_DIVIDER;

				clock.enabled = true;
				builtLabel.enabled = true;
				time.enabled = true;
				quantity.enabled = true;
				cost.enabled = true;
				currency.enabled = true;


				requirement.enabled = false;
			}else{

//				Debug.Log("Inactive");
				//button.normalSprite = ACTIVE_BACKGROUND;
				divider.spriteName = ACTIVE_DIVIDER;
				
				clock.enabled = false;
				builtLabel.enabled = false;
				time.enabled = false;
				quantity.enabled = false;
				cost.enabled = false;
				currency.enabled = false;
				
				
				requirement.enabled = true;
			}
		}
	}

	void awake(){
		button = GetComponent<UIButton>();
		_state = State.ACTIVE;
	}

	void Update(){
		state = _state;
	}
}
