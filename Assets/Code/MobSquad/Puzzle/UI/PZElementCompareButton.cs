using UnityEngine;
using System.Collections;
using System;

public class PZElementCompareButton : MonoBehaviour {

	[SerializeField]
	UISprite elementSprite;

	/// <summary>
	/// the game object of elementSprite
	/// </summary>
	GameObject element;

	const float ANIMATION_LENGTH = 0.5f;

	void Awake(){
		element = elementSprite.gameObject;
	}

	void OnEnable(){
		MSActionManager.Controls.OnAnyTap [0] += GlobalOnClick;
	}

	void OnDisable(){
		MSActionManager.Controls.OnAnyTap [0] -= GlobalOnClick;
	}
	
	void OnClick()
	{
		OpenImage ();
	}

	void GlobalOnClick(TCKTouchData data){
		Collider hit = MSMath.ClickRayCast(data.pos, PZPuzzleManager.instance.puzzleCamera);
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
