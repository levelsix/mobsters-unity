using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UISprite))]
public class MSSplashBg : MonoBehaviour {

	const string splashSpriteSuffix = "splashbg";

	[SerializeField]
	int numBgs = 6;

	UISprite sprite;

	void OnEnable()
	{
		if (sprite == null)
		{
			sprite = GetComponent<UISprite>();
		}

		sprite.spriteName = (Random.Range(0,numBgs)+1) + splashSpriteSuffix;
	}
}
