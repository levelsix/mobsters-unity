using UnityEngine;
using System.Collections;

public class MSResourceCollectLabel : MonoBehaviour {

	//fonts
	[SerializeField]
	UIFont cashCollectionFont;

	[SerializeField]
	UIFont oilCollectionFont;

	[SerializeField]
	UIFont gemCollectionFont;

	[HideInInspector]
	public UILabel label;

	TweenPosition tweenPosition;

	TweenAlpha tweenAlpha;

	Transform trans;

	static readonly Vector3 movement = new Vector3(0,5,0);

	void Awake(){
		label = GetComponent<UILabel> ();
		tweenPosition = GetComponent<TweenPosition> ();
		tweenAlpha = GetComponent<TweenAlpha> ();
		trans = transform;
	}

	void OnEnable(){
		tweenPosition.ResetToBeginning ();
		tweenPosition.PlayForward ();
		tweenAlpha.ResetToBeginning ();
		tweenAlpha.PlayForward ();

	}

	void OnDisable(){
		trans.position = tweenPosition.from;
	}

	public void setStartPosition(){
		setStartPosition (trans.position);
	}

	public void setStartPosition(Vector3 position){
		tweenPosition.from = position;
		tweenPosition.to = new Vector3 (position.x + movement.x, position.y + movement.y, position.z + movement.z);
	}

	public void setFontCash(){
		label.bitmapFont = cashCollectionFont;
	}

	public void setFontOil(){
		label.bitmapFont = oilCollectionFont;
	}

	public void setFontGem(){
		label.bitmapFont = gemCollectionFont;
	}
}
