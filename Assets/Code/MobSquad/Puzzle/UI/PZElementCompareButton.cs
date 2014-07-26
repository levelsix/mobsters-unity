using UnityEngine;
using System.Collections;
using System;

public class PZElementCompareButton : MonoBehaviour {

	/// <summary>
	/// The sprite that shows the element info, not the button sprite
	/// </summary>
	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	Camera camera;

	/// <summary>
	/// the game object of elementSprite
	/// </summary>
	GameObject element;

	UIButton button;

	UISprite buttonSprite;

	const float ANIMATION_LENGTH = 0.2f;

	void Awake(){
		element = elementSprite.gameObject;
		button = GetComponent<UIButton> ();
		buttonSprite = GetComponent<UISprite> ();
	}

	void OnEnable(){
		MSActionManager.Controls.OnAnyTap [0] += GlobalOnClick;
	}

	void OnDisable(){
		MSActionManager.Controls.OnAnyTap [0] -= GlobalOnClick;
	}

	void Update(){
		button.enabled = PZPuzzleManager.instance.swapLock <= 0 || PZDeployPopup.acting;
		buttonSprite.alpha = (button.enabled) ? 1 : 0;

	}
	
	void OnClick()
	{
		OpenImage ();
	}

	void GlobalOnClick(TCKTouchData data){
		Collider hit = MSMath.ClickRayCast(data.pos, camera);
		if(hit == null || hit.GetComponent<PZElementCompareButton> () == null) {
			CloseImage();
		}
	}

	void OpenImage(){
		elementSprite.transform.localScale = Vector3.zero;
		elementSprite.alpha = 0f;
		TweenScale.Begin (element, ANIMATION_LENGTH, Vector3.one);
		TweenAlpha.Begin (element, ANIMATION_LENGTH, 1f);
	}

	void CloseImage(){
		TweenScale.Begin (element, ANIMATION_LENGTH, Vector3.zero);
		TweenAlpha.Begin (element, ANIMATION_LENGTH, 0f);
	}
}
